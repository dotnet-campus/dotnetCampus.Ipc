using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Properties;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace dotnetCampus.Ipc.CodeFixeProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoresIpcExceptionIsRecommendedCodeFixProvider)), Shared]
public class DefaultReturnDependsOnIgnoresIpcExceptionCodeFixProvider : CodeFixProvider
{
    public DefaultReturnDependsOnIgnoresIpcExceptionCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(IPC242_IpcProperty_DefaultReturnDependsOnIgnoresIpcException.Id);
    }

    public override ImmutableArray<string> FixableDiagnosticIds { get; }

    public override FixAllProvider? GetFixAllProvider()
    {
        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        return WellKnownFixAllProviders.BatchFixer;
    }

    public override async Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            if (root.FindNode(diagnosticSpan) is AttributeArgumentSyntax argumentNode
                && argumentNode.Parent?.Parent is AttributeSyntax attributeNode)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.IPC242_Fix1,
                        createChangedDocument: c => RemoveDefaultReturn(context.Document, attributeNode, c),
                        equivalenceKey: Resources.IPC242_Fix1),
                    diagnostic);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.IPC242_Fix2,
                        createChangedDocument: c => SetIgnoresIpcException(context.Document, attributeNode, c),
                        equivalenceKey: Resources.IPC242_Fix2),
                    diagnostic);
            }
        }
    }

    private async Task<Document> RemoveDefaultReturn(Document document, AttributeSyntax attributeNode, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || attributeNode.ArgumentList is null)
        {
            return document;
        }

        if (attributeNode.ArgumentList.Arguments.Count <= 1)
        {
            // 只设了这一个属性。
            var newAttributeNode = attributeNode.RemoveNode(attributeNode.ArgumentList, SyntaxRemoveOptions.KeepNoTrivia);
            if (newAttributeNode is not null)
            {
                var newRoot = root.ReplaceNode(attributeNode, newAttributeNode);
                return document.WithSyntaxRoot(newRoot);
            }
        }
        else
        {
            // 还设了其他属性。
            var argumentNode = attributeNode.ArgumentList.Arguments.FirstOrDefault(x =>
                x.NameEquals?.Name.ToString() == nameof(IpcMethodAttribute.DefaultReturn));

            var newAttributeNode = argumentNode is null
                ? null
                : attributeNode.ArgumentList.RemoveNode(argumentNode, SyntaxRemoveOptions.KeepNoTrivia);

            if (newAttributeNode is not null)
            {
                var newRoot = root.ReplaceNode(attributeNode.ArgumentList, newAttributeNode);
                return document.WithSyntaxRoot(newRoot);
            }
        }

        return document;
    }

    private async Task<Document> SetIgnoresIpcException(Document document, AttributeSyntax syntax, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || syntax.ArgumentList is null)
        {
            return document;
        }

        var argumentNode = syntax.ArgumentList.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.ToString() == nameof(IpcMemberAttribute.IgnoresIpcException));

        var newAttributeNode = argumentNode is null
            ? syntax.ArgumentList.AddArguments(
                // IgnoresIpcException = true/false
                SF.AttributeArgument(
                    SF.NameEquals(
                        SF.IdentifierName(nameof(IpcPublicAttribute.IgnoresIpcException))),
                    null,
                    SF.LiteralExpression(SyntaxKind.TrueLiteralExpression)))
            : syntax.ArgumentList.RemoveNode(argumentNode, SyntaxRemoveOptions.KeepNoTrivia);

        if (newAttributeNode is not null)
        {
            var newRoot = root.ReplaceNode(syntax.ArgumentList, newAttributeNode);
            return document.WithSyntaxRoot(newRoot);
        }

        return document;
    }
}
