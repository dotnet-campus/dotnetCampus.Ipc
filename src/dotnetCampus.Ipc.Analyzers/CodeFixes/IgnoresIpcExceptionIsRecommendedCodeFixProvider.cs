using dotnetCampus.Ipc.Properties;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace dotnetCampus.Ipc.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoresIpcExceptionIsRecommendedCodeFixProvider)), Shared]
public class IgnoresIpcExceptionIsRecommendedCodeFixProvider : CodeFixProvider
{
    public IgnoresIpcExceptionIsRecommendedCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(IPC131_IpcMembers_IgnoresIpcExceptionIsRecommended.Id);
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
            if (root.FindNode(diagnosticSpan) is AttributeSyntax attributeNode)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Localizations.IPC131_Fix1,
                        createChangedDocument: c => SetIgnoresIpcException(context.Document, attributeNode, true, c),
                        equivalenceKey: Localizations.IPC131_Fix1),
                    diagnostic);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Localizations.IPC131_Fix2,
                        createChangedDocument: c => SetIgnoresIpcException(context.Document, attributeNode, false, c),
                        equivalenceKey: Localizations.IPC131_Fix2),
                    diagnostic);
            }
        }
    }

    private async Task<Document> SetIgnoresIpcException(Document document, AttributeSyntax attributeNode, bool value, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var newArgumentNode = SF.AttributeArgument(
            SF.NameEquals(
                SF.IdentifierName(nameof(IpcPublicAttribute.IgnoresIpcException))),
            null,
            SF.LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression));

        if (attributeNode.ArgumentList is { } argumentList)
        {
            var newArgumentListNode = argumentList.AddArguments(newArgumentNode);
            var newRoot = root.ReplaceNode(argumentList, newArgumentListNode);
            return document.WithSyntaxRoot(newRoot);
        }
        else
        {
            var newAttributeNode = attributeNode.AddArgumentListArguments(newArgumentNode);
            var newRoot = root.ReplaceNode(attributeNode, newAttributeNode);
            return document.WithSyntaxRoot(newRoot);
        }
    }
}
