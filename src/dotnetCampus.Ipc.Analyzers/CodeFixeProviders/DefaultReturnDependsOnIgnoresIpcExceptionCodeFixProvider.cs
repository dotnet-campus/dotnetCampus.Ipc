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
        FixableDiagnosticIds = ImmutableArray.Create(DIPC102_DefaultReturnDependsOnIgnoresIpcException.Id);
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
            if (root.FindNode(diagnosticSpan) is AttributeArgumentSyntax argument
                && argument.Parent?.Parent is AttributeSyntax attributeSyntax)
            {
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.DIPC102_Fix1,
                        createChangedSolution: c => RemoveDefaultReturn(context.Document, attributeSyntax, c),
                        equivalenceKey: Resources.DIPC102_Fix1),
                    diagnostic);
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: Resources.DIPC102_Fix2,
                        createChangedSolution: c => SetIgnoresIpcException(context.Document, attributeSyntax, c),
                        equivalenceKey: Resources.DIPC102_Fix2),
                    diagnostic);
            }
        }
    }

    private async Task<Solution> RemoveDefaultReturn(Document document, AttributeSyntax syntax, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || syntax.ArgumentList is null)
        {
            return document.Project.Solution;
        }

        var argument = syntax.ArgumentList.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.ToString() == nameof(IpcMemberAttribute.DefaultReturn));

        var newAttributeSyntax = argument is null
            ? null
            : syntax.ArgumentList.RemoveNode(argument, SyntaxRemoveOptions.KeepNoTrivia);

        if (newAttributeSyntax is not null)
        {
            var newRoot = root.ReplaceNode(syntax.ArgumentList, newAttributeSyntax);
            return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }

        return document.Project.Solution;
    }

    private async Task<Solution> SetIgnoresIpcException(Document document, AttributeSyntax syntax, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null || syntax.ArgumentList is null)
        {
            return document.Project.Solution;
        }

        var argument = syntax.ArgumentList.Arguments.FirstOrDefault(x =>
            x.NameEquals?.Name.ToString() == nameof(IpcMemberAttribute.IgnoresIpcException));

        var newAttributeSyntax = argument is null
            ? syntax.ArgumentList.AddArguments(
                // IgnoresIpcException = true/false
                SF.AttributeArgument(
                    SF.NameEquals(
                        SF.IdentifierName(nameof(IpcPublicAttribute.IgnoresIpcException))),
                    null,
                    SF.LiteralExpression(SyntaxKind.TrueLiteralExpression)))
            : syntax.ArgumentList.RemoveNode(argument, SyntaxRemoveOptions.KeepNoTrivia);

        if (newAttributeSyntax is not null)
        {
            var newRoot = root.ReplaceNode(syntax.ArgumentList, newAttributeSyntax);
            return document.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
        }

        return document.Project.Solution;
    }
}
