using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.SourceGenerators.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

internal static class IpcMemberAttributeHelper
{
    public static IEnumerable<(AttributeSyntax? attribute, IpcProxyMemberNamedValues namedValues)> TryFindAttributes(
        SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationSyntax)
    {
        if (TryFindIpcPublicType(semanticModel, classDeclarationSyntax, out var compilation))
        {
            foreach (var (contractType, realType, member, implementationMember) in compilation.EnumerateMembersByContractType())
            {
                var namedValues = implementationMember switch
                {
                    IPropertySymbol propertySymbol => propertySymbol.GetIpcNamedValues(realType),
                    IMethodSymbol methodSymbol => methodSymbol.GetIpcNamedValues(null, realType),
                    _ => null,
                };
                if (namedValues is null)
                {
                    continue;
                }

                var memberNode = classDeclarationSyntax.DescendantNodes()
                    .Where(x => x is PropertyDeclarationSyntax or MethodDeclarationSyntax)
                    .FirstOrDefault(x => x switch
                    {
                        PropertyDeclarationSyntax propertyDeclarationSyntax => SymbolEqualityComparer.Default.Equals(implementationMember, semanticModel.GetDeclaredSymbol(propertyDeclarationSyntax)),
                        MethodDeclarationSyntax methodDeclarationSyntax => SymbolEqualityComparer.Default.Equals(implementationMember, semanticModel.GetDeclaredSymbol(methodDeclarationSyntax)),
                        _ => false,
                    }) as MemberDeclarationSyntax;
                if (memberNode is not null && memberNode.AttributeLists.SelectMany(x => x.Attributes).FirstOrDefault(x =>
                {
                    string? attributeName = x.Name switch
                    {
                        IdentifierNameSyntax identifierName => identifierName.ToString(),
                        QualifiedNameSyntax qualifiedName => qualifiedName.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.ToString(),
                        _ => null,
                    };
                    return attributeName is not null &&
                        (attributeName.Equals(nameof(IpcPropertyAttribute), StringComparison.Ordinal)
                        || attributeName.Equals(GetAttributeName(nameof(IpcPropertyAttribute)), StringComparison.Ordinal)
                        || attributeName.Equals(nameof(IpcMethodAttribute), StringComparison.Ordinal)
                        || attributeName.Equals(GetAttributeName(nameof(IpcMethodAttribute)), StringComparison.Ordinal));
                }) is { } attribute)
                {
                    yield return (attribute, namedValues);
                }
                else
                {
                    yield return (null, namedValues);
                }
            }
        }
    }

    private static bool TryFindIpcPublicType(SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationNode,
        [NotNullWhen(true)] out PublicIpcObjectCompilation? publicIpcObjectCompilation)
    {
        if (semanticModel.GetDeclaredSymbol(classDeclarationNode) is { } classDeclarationSymbol
            && classDeclarationSymbol.GetAttributes().FirstOrDefault(x => string.Equals(
                x.AttributeClass?.ToString(),
                typeof(IpcPublicAttribute).FullName,
                StringComparison.Ordinal)) is { } ipcPublicAttribute)
        {
            if (ipcPublicAttribute.ConstructorArguments.Length == 1
                && ipcPublicAttribute.ConstructorArguments[0] is TypedConstant typedConstant
                && typedConstant.Value is INamedTypeSymbol contractType)
            {
                publicIpcObjectCompilation = new PublicIpcObjectCompilation(classDeclarationNode.SyntaxTree, classDeclarationSymbol, contractType);
                return true;
            }
        }
        publicIpcObjectCompilation = null;
        return false;
    }
}
