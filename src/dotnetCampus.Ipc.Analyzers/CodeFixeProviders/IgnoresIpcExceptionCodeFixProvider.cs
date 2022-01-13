using System.Collections.Immutable;
using System.Composition;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Properties;

using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace dotnetCampus.Ipc.CodeFixeProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoresIpcExceptionCodeFixProvider)), Shared]
public class IgnoresIpcExceptionCodeFixProvider : CodeFixProvider
{
    public IgnoresIpcExceptionCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(DIPC101_IgnoresIpcExceptionIsRecommended.Id);
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
            if (root.FindNode(diagnosticSpan) is AttributeSyntax attributeSyntax)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.DIPC101_Fix1,
                        createChangedSolution: c => SetIgnoresIpcException(context.Document, attributeSyntax, true, c),
                        equivalenceKey: Resources.DIPC101_Fix1),
                    diagnostic);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.DIPC101_Fix2,
                        createChangedSolution: c => SetIgnoresIpcException(context.Document, attributeSyntax, false, c),
                        equivalenceKey: Resources.DIPC101_Fix2),
                    diagnostic);
            }
        }
    }

    private async Task<Solution> SetIgnoresIpcException(Document document, AttributeSyntax syntax, bool value, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || syntax.ArgumentList is null)
        {
            return document.Project.Solution;
        }

        var newAttributeSyntax = syntax.ArgumentList.AddArguments(
            // IgnoresIpcException = true/false
            SF.AttributeArgument(
                SF.NameEquals(
                    SF.IdentifierName(nameof(IpcPublicAttribute.IgnoresIpcException))),
                null,
                SF.LiteralExpression(value ? SyntaxKind.TrueLiteralExpression : SyntaxKind.FalseLiteralExpression)));

        var newRoot = root.ReplaceNode(syntax.ArgumentList, newAttributeSyntax);
        return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
    }
}
