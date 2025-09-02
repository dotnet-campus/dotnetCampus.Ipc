using dotnetCampus.Ipc.CodeAnalysis.Models;
using dotnetCampus.Ipc.Generators.Compiling;

namespace dotnetCampus.Ipc.Analyzers.Compiling;

internal static class IpcAttributeHelper
{
    public static IEnumerable<(AttributeSyntax attributeNode, IpcPublicAttributeNamedValues namedValues)> TryFindIpcPublicAttributes(
        SemanticModel semanticModel, InterfaceDeclarationSyntax typeDeclarationNode)
    {
        var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclarationNode);
        if (typeSymbol is null)
        {
            yield break;
        }

        if (typeDeclarationNode.AttributeLists.SelectMany(x => x.Attributes).FirstOrDefault(x =>
        {
            string? attributeName = x.Name switch
            {
                IdentifierNameSyntax identifierName => identifierName.ToString(),
                QualifiedNameSyntax qualifiedName => qualifiedName.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.ToString(),
                _ => null,
            };
            return attributeName is not null &&
                (attributeName.Equals(nameof(IpcPublicAttribute), StringComparison.Ordinal)
                || attributeName.Equals(GetAttributeName(nameof(IpcPublicAttribute)), StringComparison.Ordinal));
        }) is { } attributeNode)
        {
            var namedValues = typeSymbol.GetIpcPublicNamedValues();
            yield return (attributeNode, namedValues);
        }
    }

    public static IEnumerable<(AttributeSyntax attributeNode, IpcShapeAttributeNamedValues namedValues)> TryFindIpcShapeAttributes(
        SemanticModel semanticModel, ClassDeclarationSyntax typeDeclarationNode)
    {
        var typeSymbol = semanticModel.GetDeclaredSymbol(typeDeclarationNode);
        if (typeSymbol is null)
        {
            yield break;
        }

        if (typeDeclarationNode.AttributeLists.SelectMany(x => x.Attributes).FirstOrDefault(x =>
        {
            string? attributeName = x.Name switch
            {
                IdentifierNameSyntax identifierName => identifierName.ToString(),
                QualifiedNameSyntax qualifiedName => qualifiedName.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.ToString(),
                _ => null,
            };
            return attributeName is not null &&
                (attributeName.Equals(nameof(IpcShapeAttribute), StringComparison.Ordinal)
                || attributeName.Equals(GetAttributeName(nameof(IpcShapeAttribute)), StringComparison.Ordinal));
        }) is { } attributeNode)
        {
            var namedValues = typeSymbol.GetIpcShapeNamedValues();
            yield return (attributeNode, namedValues);
        }
    }

    public static IEnumerable<(AttributeSyntax? attributeNode, IpcPublicAttributeNamedValues namedValues)> TryFindMemberAttributes(
        SemanticModel semanticModel, TypeDeclarationSyntax typeDeclarationNode)
    {
        if (TryFindIpcPublicType(semanticModel, typeDeclarationNode, out var compilation))
        {
            foreach (var (contractType, member) in compilation.EnumerateMembers())
            {
                var namedValues = member switch
                {
                    IPropertySymbol propertySymbol => propertySymbol.GetIpcNamedValues(contractType),
                    IMethodSymbol methodSymbol => methodSymbol.GetIpcNamedValues(null, contractType),
                    _ => null,
                };
                if (namedValues is null)
                {
                    continue;
                }

                var memberNode = typeDeclarationNode.DescendantNodes()
                    .Where(x => x is PropertyDeclarationSyntax or MethodDeclarationSyntax)
                    .FirstOrDefault(x => x switch
                    {
                        PropertyDeclarationSyntax propertyDeclarationSyntax => SymbolEqualityComparer.Default.Equals(member, semanticModel.GetDeclaredSymbol(propertyDeclarationSyntax)),
                        MethodDeclarationSyntax methodDeclarationSyntax => SymbolEqualityComparer.Default.Equals(member, semanticModel.GetDeclaredSymbol(methodDeclarationSyntax)),
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
                }) is { } attributeNode)
                {
                    yield return (attributeNode, namedValues);
                }
                else
                {
                    yield return (null, namedValues);
                }
            }
        }
    }

    private static bool TryFindIpcPublicType(SemanticModel semanticModel, TypeDeclarationSyntax typeDeclarationNode,
        [NotNullWhen(true)] out IpcPublicCompilation? publicIpcObjectCompilation)
    {
        if (semanticModel.GetDeclaredSymbol(typeDeclarationNode) is { } typeDeclarationSymbol
            && typeDeclarationSymbol.GetAttributes().FirstOrDefault(x => string.Equals(
                x.AttributeClass?.ToString(),
                typeof(IpcPublicAttribute).FullName,
                StringComparison.Ordinal)) is { } ipcPublicAttribute)
        {
            publicIpcObjectCompilation = new(typeDeclarationNode.SyntaxTree, semanticModel, typeDeclarationSymbol);
            return true;
        }
        publicIpcObjectCompilation = null;
        return false;
    }
}
