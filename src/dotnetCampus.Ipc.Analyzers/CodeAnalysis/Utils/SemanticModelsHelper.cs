using Microsoft.CodeAnalysis.CSharp.Syntax;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

internal static class SemanticModelsHelper
{
    public static string ToFullyQualifiedName(this INamedTypeSymbol typeSymbol)
    {
        var symbolDisplayFormat = new SymbolDisplayFormat(
            typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        return typeSymbol.ToDisplayString(symbolDisplayFormat);
    }

    public static SyntaxNode ReplaceNodeWithUsings(this SyntaxNode rootSyntaxNode,
        SyntaxNode oldSyntaxNode, SyntaxNode newSyntaxNode,
        params INamedTypeSymbol[] namespaceProviders)
    {
        // 追踪多个语法节点的变更。
        var annotation = new SyntaxAnnotation();

        // 追踪即将被替换的语法节点。
        var newRoot = rootSyntaxNode.ReplaceNode(
             oldSyntaxNode,
             oldSyntaxNode.WithAdditionalAnnotations(annotation));

        // 增加文件级命名空间。
        var fileNamespace = rootSyntaxNode.ChildNodes().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString()
            ?? rootSyntaxNode.ChildNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
        if (newRoot is CompilationUnitSyntax compilation)
        {
            foreach (var namespaceProvider in namespaceProviders)
            {
                var newNamespace = namespaceProvider.ContainingNamespace.ToString();
                if (!string.Equals(fileNamespace, newNamespace)
                    && compilation.Usings.All(u => !string.Equals(newNamespace, u.Name.ToString(), StringComparison.Ordinal)))
                {
                    var @namespace = SF.UsingDirective(SF.ParseName(newNamespace));
                    newRoot = newRoot.InsertNodesAfter(
                        compilation.Usings.Last(),
                        new[] { @namespace });
                }
            }
        }

        // 替换节点。
        var annotatedOldSyntaxNode = newRoot.GetAnnotatedNodes(annotation).First();
        newRoot = newRoot.ReplaceNode(annotatedOldSyntaxNode, newSyntaxNode);

        // 返回替换后的节点。
        return newRoot;
    }
}
