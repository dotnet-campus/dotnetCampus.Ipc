namespace dotnetCampus.Ipc.SourceGenerators.Compiling;

/// <summary>
/// 提供 IPC 对象（代理壳类）的语法和语义分析。
/// </summary>
[DebuggerDisplay("{ShapeType} : {ContractType.Name,nq}")]
internal class IpcShapeCompilation : IpcPublicCompilation
{
    /// <summary>
    /// 创建 IPC 对象的语法和语义分析。
    /// </summary>
    /// <param name="syntaxTree">IPC 对象所在整个文件的语法树。</param>
    /// <param name="semanticModel">语义模型。</param>
    /// <param name="ipcType">IPC 代理壳的语义符号。</param>
    /// <param name="contractType">IPC 契约类型的语义符号。</param>
    public IpcShapeCompilation(SyntaxTree syntaxTree, SemanticModel semanticModel,
        INamedTypeSymbol ipcType, INamedTypeSymbol contractType)
        : base(syntaxTree, semanticModel, ipcType)
    {
        ContractType = contractType;
    }

    /// <summary>
    /// IPC 契约类型的语义符号。
    /// </summary>
    public INamedTypeSymbol ContractType { get; }

    /// <summary>
    /// 查找 IPC 对象的所有成员。
    /// </summary>
    /// <returns>所有成员信息。</returns>
    public IEnumerable<(INamedTypeSymbol contractType, INamedTypeSymbol shapeType, ISymbol member, ISymbol shapeMember)> EnumerateMembersByContractType()
    {
        var members = ContractType.AllInterfaces.SelectMany(x => x.GetMembers())
            .Concat(ContractType.GetMembers());
        foreach (var member in members)
        {
            if (member is IMethodSymbol method && method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet)
            {
                // 属性内的方法忽略。
                continue;
            }

            if (member is INamedTypeSymbol)
            {
                // 接口中如果定义有其他嵌套类型/枚举/接口/结构，也忽略。
                continue;
            }

            if (member is IEventSymbol eventSymbol)
            {
                // IPC 不支持事件。
                var eventSyntax = eventSymbol.TryGetMemberDeclaration();
                throw new DiagnosticException(
                    IPCTMP1_IpcMembers_EventIsNotSupported,
                    eventSyntax?.GetLocation(),
                    IpcType.Name,
                    ContractType.Name);
            }

            if (IpcType.FindImplementationForInterfaceMember(member) is ISymbol implementationMember)
            {
                yield return new(ContractType, IpcType, member, implementationMember);
            }
            else
            {
                var attribute = IpcType.TryGetClassDeclarationWithIpcAttribute(SemanticModel);
                throw new DiagnosticException(
                    IPC161_IpcShape_ContractTypeDismatchWithInterface,
                    attribute?.ArgumentList?.Arguments.FirstOrDefault()?.GetLocation(),
                    IpcType.Name,
                    ContractType.Name);
            }
        }
    }

    /// <summary>
    /// 在一个语法树（单个文件）中查找所有的 IPC 对象（契约接口）。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <param name="syntaxTree">单个文件的语法树。</param>
    /// <param name="publicIpcObjectCompilations">如果找到了 IPC 对象，则此参数为此语法树中的所有 IPC 对象；如果没有找到，则为空集合。</param>
    /// <returns>如果找到了 IPC 类型，则返回 true；如果没有找到，则返回 false。</returns>
    public static bool TryFindIpcShapeCpmpilations(Compilation compilation, SyntaxTree syntaxTree,
        out IReadOnlyList<IpcShapeCompilation> publicIpcObjectCompilations)
    {
        var result = new List<IpcShapeCompilation>();
        var typeDeclarationSyntaxes = from node in syntaxTree.GetRoot().DescendantNodes()
                                      where node.IsKind(SyntaxKind.ClassDeclaration)
                                      select (ClassDeclarationSyntax) node;

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        foreach (var typeDeclarationSyntax in typeDeclarationSyntaxes)
        {
            if (semanticModel.GetDeclaredSymbol(typeDeclarationSyntax) is { } typeSymbol
                && typeSymbol.GetAttributes().FirstOrDefault(x => string.Equals(
                     x.AttributeClass?.ToString(),
                     typeof(IpcShapeAttribute).FullName,
                     StringComparison.Ordinal)) is { } ipcPublicAttribute)
            {
                if (ipcPublicAttribute.ConstructorArguments.Length == 1)
                {
                    if (ipcPublicAttribute.ConstructorArguments[0] is TypedConstant typedConstant
                        && typedConstant.Value is INamedTypeSymbol contractType)
                    {
                        if (contractType.TypeKind == TypeKind.Interface)
                        {
                            result.Add(new IpcShapeCompilation(syntaxTree, semanticModel, typeSymbol, contractType));
                        }
                        else
                        {
                            throw new DiagnosticException(IPC002_KnownDiagnosticError);
                        }
                    }
                    else
                    {
                        throw new DiagnosticException(IPC001_KnownCompilerError);
                    }
                }
                else
                {
                    throw new DiagnosticException(IPC001_KnownCompilerError);
                }
            }
        }

        publicIpcObjectCompilations = result;
        return publicIpcObjectCompilations.Count > 0;
    }
}
