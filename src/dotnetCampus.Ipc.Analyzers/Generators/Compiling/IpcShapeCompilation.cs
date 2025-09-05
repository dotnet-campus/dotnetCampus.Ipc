namespace dotnetCampus.Ipc.Generators.Compiling;
/// <summary>
/// 提供 IPC 对象（形状代理类）的语法和语义分析。
/// </summary>
[DebuggerDisplay("{ShapeType} : {ContractType.Name,nq}")]
internal class IpcShapeCompilation : IpcPublicCompilation, IEquatable<IpcShapeCompilation?>
{
    /// <summary>
    /// 创建 IPC 对象的语法和语义分析。
    /// </summary>
    /// <param name="syntaxTree">IPC 对象所在整个文件的语法树。</param>
    /// <param name="semanticModel">语义模型。</param>
    /// <param name="ipcType">IPC 形状代理的语义符号。</param>
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
    /// 试图解析一个类型定义语法节点并创建 IPC 对象（契约接口）。
    /// </summary>
    /// <param name="syntaxNode">类型定义语法节点。</param>
    /// <param name="semanticModel">此类型定义语法节点的语义模型。</param>
    /// <param name="ipcShapeCompilation">如果找到了 IPC 对象，则此参数为此语法树中的所有 IPC 对象；如果没有找到，则为空集合。</param>
    /// <returns>如果找到了 IPC 类型，则返回 true；如果没有找到，则返回 false。</returns>
    public static bool TryCreateIpcShapeCompilation(ClassDeclarationSyntax syntaxNode, SemanticModel semanticModel,
        [NotNullWhen(true)] out IpcShapeCompilation? ipcShapeCompilation)
    {
        var syntaxTree = syntaxNode.SyntaxTree;
        if (semanticModel.GetDeclaredSymbol(syntaxNode) is { } typeSymbol
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
                        ipcShapeCompilation = new IpcShapeCompilation(syntaxTree, semanticModel, typeSymbol, contractType);
                        return true;
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

        ipcShapeCompilation = null;
        return false;
    }

    /// <summary>
    /// 在一个语法树（单个文件）中查找所有的 IPC 对象（契约接口）。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <param name="syntaxTree">单个文件的语法树。</param>
    /// <param name="publicIpcObjectCompilations">如果找到了 IPC 对象，则此参数为此语法树中的所有 IPC 对象；如果没有找到，则为空集合。</param>
    /// <returns>如果找到了 IPC 类型，则返回 true；如果没有找到，则返回 false。</returns>
    public static bool TryFindIpcShapeCompilations(Compilation compilation, SyntaxTree syntaxTree,
        out IReadOnlyList<IpcShapeCompilation> publicIpcObjectCompilations)
    {
        var result = new List<IpcShapeCompilation>();
        var typeDeclarationSyntaxes = from node in syntaxTree.GetRoot().DescendantNodes()
                                      where node.IsKind(SyntaxKind.ClassDeclaration)
                                      select (ClassDeclarationSyntax) node;

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        foreach (var typeDeclarationSyntax in typeDeclarationSyntaxes)
        {
            if (TryCreateIpcShapeCompilation(typeDeclarationSyntax, semanticModel, out var publicIpcObjectCompilation))
            {
                result.Add(publicIpcObjectCompilation);
            }
        }

        publicIpcObjectCompilations = result;
        return publicIpcObjectCompilations.Count > 0;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as IpcShapeCompilation);
    }

    public bool Equals(IpcShapeCompilation? other)
    {
        return other is not null &&
               SymbolEqualityComparer.Default.Equals(IpcType, other.IpcType) &&
               SymbolEqualityComparer.Default.Equals(ContractType, other.ContractType);
    }

    public override int GetHashCode()
    {
        var hashCode = -1723556882;
        hashCode = hashCode * -1521134295 + base.GetHashCode();
        hashCode = hashCode * -1521134295 + EqualityComparer<INamedTypeSymbol>.Default.GetHashCode(IpcType);
        hashCode = hashCode * -1521134295 + EqualityComparer<INamedTypeSymbol>.Default.GetHashCode(ContractType);
        return hashCode;
    }
}
