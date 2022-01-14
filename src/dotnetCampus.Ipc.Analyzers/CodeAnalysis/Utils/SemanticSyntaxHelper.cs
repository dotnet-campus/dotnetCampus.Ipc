using System.Diagnostics;

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
}
