using dotnetCampus.Ipc.Properties;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace dotnetCampus.Ipc.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoresIpcExceptionIsRecommendedCodeFixProvider)), Shared]
public class EmptyIpcMemberAttributeIsUnnecessaryCodeFixProvider : CodeFixProvider
{
    public EmptyIpcMemberAttributeIsUnnecessaryCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(IPC201_IpcMember_EmptyIpcMemberAttributeIsUnnecessary.Id);
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
            var node = root.FindNode(diagnosticSpan);
            if (node is AttributeSyntax attributeNode)
            {
                var fix = string.Format(Resources.IPC201_Fix, attributeNode.Name);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: fix,
                        createChangedDocument: c => RemoveAttribute(context.Document, attributeNode, c),
                        equivalenceKey: fix),
                    diagnostic);
            }
            else if (node is AttributeListSyntax attributeListNode)
            {
                var fix = string.Format(Resources.IPC201_Fix, attributeListNode.Attributes[0].Name);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: fix,
                        createChangedDocument: c => RemoveAttribute(context.Document, attributeListNode, c),
                        equivalenceKey: fix),
                    diagnostic);
            }
        }
    }

    private async Task<Document> RemoveAttribute(Document document, SyntaxNode syntax, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || syntax.Parent is null)
        {
            return document;
        }

        var newAttributeNode = syntax.Parent.RemoveNode(syntax, SyntaxRemoveOptions.KeepUnbalancedDirectives);

        if (newAttributeNode is not null)
        {
            var newRoot = root.ReplaceNode(syntax.Parent, newAttributeNode);
            return document.WithSyntaxRoot(newRoot);
        }

        return document;
    }
}
