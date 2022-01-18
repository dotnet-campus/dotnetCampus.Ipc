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
public class EmptyIpcMemberAttributeIsUnnecessaryCodeFixProvider : CodeFixProvider
{
    public EmptyIpcMemberAttributeIsUnnecessaryCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(DIPC121_IpcMember_EmptyIpcMemberAttributeIsUnnecessary.Id);
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
            if (node is AttributeSyntax attributeSyntax)
            {
                var fix = string.Format(Resources.DIPC121_Fix, attributeSyntax.Name);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: fix,
                        createChangedDocument: c => RemoveAttribute(context.Document, attributeSyntax, c),
                        equivalenceKey: fix),
                    diagnostic);
            }
            else if (node is AttributeListSyntax attributeListSyntax)
            {
                var fix = string.Format(Resources.DIPC121_Fix, attributeListSyntax.Attributes[0].Name);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: fix,
                        createChangedDocument: c => RemoveAttribute(context.Document, attributeListSyntax, c),
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

        var newAttributeSyntax = syntax.Parent.RemoveNode(syntax, SyntaxRemoveOptions.AddElasticMarker);

        if (newAttributeSyntax is not null)
        {
            var newRoot = root.ReplaceNode(syntax.Parent, newAttributeSyntax);
            return document.WithSyntaxRoot(newRoot);
        }

        return document;
    }
}
