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
            IPC160_IpcShape_ContractTypeMustBeAnInterface.Id,
            IPC161_IpcShape_ContractTypeDismatchWithInterface.Id);
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
            var (classDeclarationNode, contractTypeNode) = FindClassDeclarationNodeFromDiagnostic(root, diagnostic);
            if (classDeclarationNode is not null && contractTypeNode is not null)
            {
                var (_, namedValues) = IpcAttributeHelper.TryFindIpcShapeAttributes(semanticModel, classDeclarationNode).FirstOrDefault();
                if (namedValues.IpcType is { } ipcType)
                {
                    if (ipcType.AllInterfaces.Length is 0)
                    {
                        // 没有实现任何接口，此修改器无法给出任何建议。
                        continue;
                    }

                    foreach (var @interface in ipcType.AllInterfaces)
                    {
                        var fix = string.Format(Resources.IPC161_Fix1, @interface.Name);
                        context.RegisterCodeFix(
                            CodeAction.Create(
                                title: fix,
                                createChangedDocument: c => ChangeContractType(context.Document, contractTypeNode, @interface, c),
                                equivalenceKey: fix),
                            diagnostic);
                    }
                }
            }
        }
    }

    private async Task<Document> ChangeContractType(Document document,
        TypeSyntax contractTypeNode, INamedTypeSymbol interfaceSymbol,
        CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var newContractTypeNode = SF.ParseTypeName(interfaceSymbol.Name);
        var newRoot = root.ReplaceNodeWithUsings(
            contractTypeNode, newContractTypeNode,
            interfaceSymbol);

        return document.WithSyntaxRoot(newRoot);
    }

    private (ClassDeclarationSyntax? classDeclarationNode, TypeSyntax? typeNode) FindClassDeclarationNodeFromDiagnostic(
        SyntaxNode root, Diagnostic diagnostic)
    {
        var diagnosticSpan = diagnostic.Location.SourceSpan;
        if (root.FindNode(diagnosticSpan) is TypeSyntax typeNode
            && typeNode.Parent is TypeOfExpressionSyntax typeOfExpressionNode
            && typeOfExpressionNode.Parent is AttributeArgumentSyntax attributeArgumentNode
            && attributeArgumentNode.Parent is AttributeArgumentListSyntax attributeArgumentListNode
            && attributeArgumentListNode.Parent is AttributeSyntax attributeNode
            && attributeNode.Parent is AttributeListSyntax attributeListNode
            && attributeListNode.Parent is ClassDeclarationSyntax classDeclarationNode1)
        {
            return (classDeclarationNode1, typeNode);
        }
        return (null, null);
    }
}
