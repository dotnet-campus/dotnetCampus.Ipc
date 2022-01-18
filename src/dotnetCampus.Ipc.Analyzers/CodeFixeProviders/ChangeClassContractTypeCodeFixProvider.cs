using System.Collections.Immutable;
using System.Composition;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;
using dotnetCampus.Ipc.Properties;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace dotnetCampus.Ipc.CodeFixeProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoresIpcExceptionIsRecommendedCodeFixProvider)), Shared]
public class ChangeClassContractTypeCodeFixProvider : CodeFixProvider
{
    public ChangeClassContractTypeCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(
            DIPC003_ContractTypeMustBeAnInterface.Id,
            DIPC004_ContractTypeDismatchWithInterface.Id);
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
        var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken);
        if (semanticModel is null)
        {
            return;
        }

        foreach (var diagnostic in context.Diagnostics)
        {
            var diagnosticSpan = diagnostic.Location.SourceSpan;
            if (root.FindNode(diagnosticSpan) is TypeSyntax typeOfTypeSyntax
                && typeOfTypeSyntax.Parent is TypeOfExpressionSyntax typeOfExpressionSyntax
                && typeOfExpressionSyntax.Parent is AttributeArgumentSyntax attributeArgumentSyntax
                && attributeArgumentSyntax.Parent is AttributeArgumentListSyntax attributeArgumentListSyntax
                && attributeArgumentListSyntax.Parent is AttributeSyntax attributeSyntax
                && attributeSyntax.Parent is AttributeListSyntax attributeListSyntax
                && attributeListSyntax.Parent is ClassDeclarationSyntax classDeclarationSyntax)
            {
                var (_, namedValues) = IpcAttributeHelper.TryFindClassAttributes(semanticModel, classDeclarationSyntax).FirstOrDefault();
                if (namedValues.RealType is { } realType)
                {
                    if (realType.AllInterfaces.Length is 0)
                    {
                        // 没有实现任何接口，此修改器无法给出任何建议。
                        continue;
                    }

                    foreach (var @interface in realType.AllInterfaces)
                    {
                        var fix = string.Format(Resources.DIPC004_Fix1, @interface.Name);
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: fix,
                                createChangedDocument: c => ChangeContractType(context.Document, typeOfTypeSyntax, @interface, c),
                                equivalenceKey: fix),
                            diagnostic);
                    }
                }
            }
        }
    }

    private async Task<Document> ChangeContractType(Document document,
        TypeSyntax typeOfTypeSyntax, INamedTypeSymbol interfaceSymbol,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var newTypeSyntax = SF.ParseTypeName(interfaceSymbol.Name);
        var newRoot = root.ReplaceNodeWithUsings(
            typeOfTypeSyntax, newTypeSyntax,
            interfaceSymbol);

        return document.WithSyntaxRoot(newRoot);
    }
}
