using dotnetCampus.Ipc.Analyzers.Compiling;
using dotnetCampus.Ipc.Properties;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace dotnetCampus.Ipc.CodeFixes;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoresIpcExceptionIsRecommendedCodeFixProvider)), Shared]
public class LetClassImplementInterfaceCodeFixProvider : CodeFixProvider
{
    public LetClassImplementInterfaceCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(IPC161_IpcShape_ContractTypeDismatchWithInterface.Id);
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
            if (FindClassDeclarationSyntaxFromDiagnostic(root, diagnostic) is { } classDeclarationNode)
            {
                var (_, namedValues) = IpcAttributeHelper.TryFindIpcShapeAttributes(semanticModel, classDeclarationNode).FirstOrDefault();
                if (namedValues.IpcType is { } realType && namedValues.ContractType is { } contractType)
                {
                    var fix = string.Format(Localizations.IPC161_Fix2, realType.Name, contractType.Name);
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: fix,
                            createChangedDocument: c => ImplementInterface(context.Document, classDeclarationNode, contractType, c),
                            equivalenceKey: fix),
                        diagnostic);
                }
            }
        }
    }

    private async Task<Document> ImplementInterface(Document document,
        ClassDeclarationSyntax classDeclarationNode, INamedTypeSymbol interfaceSymbol, CancellationToken cancellationToken)
    {
        var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
        if (root is null)
        {
            return document;
        }

        var newClassDeclarationNode = classDeclarationNode.AddBaseListTypes(SF.SimpleBaseType(SF.ParseTypeName(interfaceSymbol.Name)));
        var newRoot = root.ReplaceNodeWithUsings(
            classDeclarationNode, newClassDeclarationNode,
            interfaceSymbol);
        return document.WithSyntaxRoot(newRoot);
    }

    private ClassDeclarationSyntax? FindClassDeclarationSyntaxFromDiagnostic(SyntaxNode root, Diagnostic diagnostic)
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
            return classDeclarationNode1;
        }
        else if (root.FindNode(diagnosticSpan) is ClassDeclarationSyntax classDeclarationNode2)
        {
            return classDeclarationNode2;
        }
        return null;
    }
}
