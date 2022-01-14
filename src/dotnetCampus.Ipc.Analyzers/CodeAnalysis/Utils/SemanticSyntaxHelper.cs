using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

internal static class SemanticSyntaxHelper
{
    public static ClassDeclarationSyntax? TryGetClassDeclaration(this INamedTypeSymbol type)
    {
        return type.Locations.FirstOrDefault() is Location location
            ? location.SourceTree?.GetRoot()?.FindNode(location.SourceSpan) as ClassDeclarationSyntax
            : null;
    }

    public static EventDeclarationSyntax? TryGetMemberDeclaration(this IEventSymbol @event)
    {
        return @event.Locations.FirstOrDefault() is Location location
            ? location.SourceTree?.GetRoot()?.FindNode(location.SourceSpan) as EventDeclarationSyntax
            : null;
    }

    public static PropertyDeclarationSyntax? TryGetMemberDeclaration(this IPropertySymbol @event)
    {
        return @event.Locations.FirstOrDefault() is Location location
            ? location.SourceTree?.GetRoot()?.FindNode(location.SourceSpan) as PropertyDeclarationSyntax
            : null;
    }

    public static MethodDeclarationSyntax? TryGetMemberDeclaration(this IMethodSymbol @event)
    {
        return @event.Locations.FirstOrDefault() is Location location
            ? location.SourceTree?.GetRoot()?.FindNode(location.SourceSpan) as MethodDeclarationSyntax
            : null;
    }
}
