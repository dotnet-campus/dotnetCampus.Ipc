using System.Diagnostics;

using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling;

/// <summary>
/// 提供 PublicIpcObject 的语法和语义分析。
/// </summary>
[DebuggerDisplay("{RealType.Name,nq} : {ContractType.Name,nq}")]
internal class PublicIpcObjectCompilation
{
    /// <summary>
    /// IpcPublicObject 文件的编译信息。
    /// </summary>
    private readonly CompilationUnitSyntax _compilationUnitSyntax;

    /// <summary>
    /// 整个项目的语义模型。
    /// </summary>
    private readonly SemanticModel _semanticModel;

    /// <summary>
    /// 创建 PublicIpcObject 的语法和语义分析。
    /// </summary>
    /// <param name="syntaxTree">IPC 真实类型整个文件的语法树。</param>
    /// <param name="semanticModel">语义模型。</param>
    /// <param name="realType">IPC 真实类型的语义符号。</param>
    /// <param name="contractType">IPC 契约接口的语义符号。</param>
    /// <exception cref="ArgumentNullException"></exception>
    public PublicIpcObjectCompilation(SyntaxTree syntaxTree, SemanticModel semanticModel,
        INamedTypeSymbol realType, INamedTypeSymbol contractType)
    {
        _compilationUnitSyntax = syntaxTree.GetCompilationUnitRoot();
        _semanticModel = semanticModel ?? throw new ArgumentNullException(nameof(semanticModel));
        RealType = realType ?? throw new ArgumentNullException(nameof(realType));
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
    }

    /// <summary>
    /// Ipc 真实类型的语义符号。
    /// </summary>
    public INamedTypeSymbol RealType { get; }

    /// <summary>
    /// IPC 契约接口的语义符号。
    /// </summary>
    public INamedTypeSymbol ContractType { get; }

    /// <summary>
    /// 获取 IPC 真实类型所在文件的全部 using。
    /// </summary>
    /// <returns>全部 using 组成的字符串。</returns>
    public string GetUsing()
    {
        var usingsSyntax = _compilationUnitSyntax.Usings;
        var usings = string.Join(Environment.NewLine, usingsSyntax.Select(x => x.ToString()));
        return usings;
    }

    /// <summary>
    /// 获取 IPC 真实类型的命名空间。
    /// </summary>
    /// <returns>IPC 真实类型的命名空间</returns>
    public string GetNamespace()
    {
        return RealType.ContainingNamespace.ToString();
    }

    /// <summary>
    /// 以契约接口类型为准，查找所有成员。
    /// </summary>
    /// <returns>所有成员信息。</returns>
    public IEnumerable<(INamedTypeSymbol contractType, INamedTypeSymbol realType, ISymbol member, ISymbol implementationMember)> EnumerateMembersByContractType()
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

            if (RealType.FindImplementationForInterfaceMember(member) is ISymbol implementationMember)
            {
                yield return new(ContractType, RealType, member, implementationMember);
            }
            else
            {
                var attribute = RealType.TryGetClassDeclarationWithIpcAttribute(_semanticModel);
                throw new DiagnosticException(
                    DIPC104_IpcContractTypeDismatchWithInterface,
                    attribute?.ArgumentList?.Arguments.FirstOrDefault()?.GetLocation(),
                    RealType.Name,
                    ContractType.Name);
            }
        }
    }

    /// <summary>
    /// 在一个语法树（单个文件）中查找所有的 IPC 类型。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <param name="syntaxTree">单个文件的语法树。</param>
    /// <param name="publicIpcObjectCompilations">如果找到了 IPC 类型，则此参数为此语法树中的所有 IPC 类型；如果没有找到，则为空集合。</param>
    /// <returns>如果找到了 IPC 类型，则返回 true；如果没有找到，则返回 false。</returns>
    public static bool TryFind(Compilation compilation, SyntaxTree syntaxTree,
        out IReadOnlyList<PublicIpcObjectCompilation> publicIpcObjectCompilations)
    {
        var result = new List<PublicIpcObjectCompilation>();
        var classDeclarationSyntaxes = from node in syntaxTree.GetRoot().DescendantNodes()
                                       where node.IsKind(SyntaxKind.ClassDeclaration)
                                       select (ClassDeclarationSyntax) node;

        var semanticModel = compilation.GetSemanticModel(syntaxTree);
        foreach (var classDeclarationSyntax in classDeclarationSyntaxes)
        {
            if (semanticModel.GetDeclaredSymbol(classDeclarationSyntax) is { } typeSymbol
                && typeSymbol.GetAttributes().FirstOrDefault(x => string.Equals(
                     x.AttributeClass?.ToString(),
                     typeof(IpcPublicAttribute).FullName,
                     StringComparison.Ordinal)) is { } ipcPublicAttribute)
            {
                if (ipcPublicAttribute.ConstructorArguments.Length == 1)
                {
                    if (ipcPublicAttribute.ConstructorArguments[0] is TypedConstant typedConstant
                        && typedConstant.Value is INamedTypeSymbol contractType)
                    {
                        if (contractType.TypeKind == TypeKind.Interface)
                        {
                            result.Add(new PublicIpcObjectCompilation(syntaxTree, semanticModel, typeSymbol, contractType));
                        }
                        else
                        {
                            var (attribute, _) = IpcAttributeHelper.TryFindClassAttributes(semanticModel, classDeclarationSyntax).FirstOrDefault();
                            if (attribute is not null)
                            {
                                throw new DiagnosticException(
                                    DIPC105_ContractTypeMustBeAnInterface,
                                    attribute.ArgumentList?.Arguments.FirstOrDefault()?.GetLocation(),
                                    contractType.Name);
                            }
                        }
                    }
                    else
                    {
                        throw new DiagnosticException(DIPC002_ContractTypeNotSpecified, typeSymbol.Locations.FirstOrDefault());
                    }
                }
                else
                {
                    throw new DiagnosticException(DIPC002_ContractTypeNotSpecified, typeSymbol.Locations.FirstOrDefault());
                }
            }
        }

        publicIpcObjectCompilations = result;
        return publicIpcObjectCompilations.Count > 0;
    }
}
