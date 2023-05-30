using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;
using dotnetCampus.Ipc.Properties;
using dotnetCampus.Ipc.SourceGenerators.Compiling;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Text;
using static dotnetCampus.Ipc.SourceGenerators.Utils.GeneratorHelper;
using SF = Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace dotnetCampus.Ipc.CodeFixeProviders;

[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(IgnoresIpcExceptionIsRecommendedCodeFixProvider)), Shared]
public class AddIpcShapeCodeFixProvider : CodeFixProvider
{
    public AddIpcShapeCodeFixProvider()
    {
        FixableDiagnosticIds = ImmutableArray.Create(IPC302_CreateIpcProxy_AddIpcShape.Id);
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
            var node = root.FindNode(diagnosticSpan);
            if (node is MemberAccessExpressionSyntax memberAccessNode
                && IpcProxyInvokingInfo.TryCreateIpcProxyInvokingInfo(
                    semanticModel,
                    memberAccessNode,
                    context.CancellationToken) is { } invokingInfo)
            {
                if (invokingInfo.ShapeType is null)
                {
                    // 在新文件中生成 IPC 代理壳。（在当前文件中生成的这一个，因为字符串拼接的代码很难和语法节点的代码保持格式统一，所以就不做了。）
                    var fix2 = string.Format(Resources.IPC302_Fix2, invokingInfo.ContractType.Name);
                    context.RegisterCodeFix(
                        CodeAction.Create(
                            title: fix2,
                            createChangedSolution: c => GenerateIpcShapeInNewFileAsync(context.Document, invokingInfo, root, c),
                            equivalenceKey: fix2),
                        diagnostic);
                }
            }
        }
    }

    private async Task<Solution> GenerateIpcShapeInNewFileAsync(Document document, IpcProxyInvokingInfo invokingInfo, SyntaxNode root, CancellationToken cancellationToken)
    {
        var newTypeName = GenerateClassNameByInterfaceName($"{invokingInfo.ContractType.Name}IpcShape");

        if (invokingInfo.MemberAccessNode is { } memberAccessNode
            && memberAccessNode.Name is GenericNameSyntax genericNameNode
            && genericNameNode.TypeArgumentList is { } typeArgumentListNode
            && typeArgumentListNode.Arguments.Count == 1
            && typeArgumentListNode.Arguments[0] is TypeSyntax contractTypeArgumentNode)
        {
            // 在调用处的泛型参数列表中传入新生成的类型。
            var newRoot = root.ReplaceNode(
                typeArgumentListNode,
                SF.TypeArgumentList(SF.SeparatedList(new[]
                {
                    contractTypeArgumentNode,
                    SF.ParseTypeName(newTypeName),
                })));
            document = document.WithSyntaxRoot(newRoot);

            var contractTypeDeclarationNode = invokingInfo.ContractType.TryGetTypeDeclaration();
            if (contractTypeDeclarationNode is not null)
            {
                // 新生成一个 IPC 代理壳类型。
                var ipcPublicCompilation = new IpcPublicCompilation(contractTypeDeclarationNode.SyntaxTree, invokingInfo.SemanticModel, invokingInfo.ContractType);
                var shapeSource = GenerateShapeSource(ipcPublicCompilation, newTypeName, GetNamespace(newRoot));
                var newDocumentRoot = SF.ParseSyntaxTree(shapeSource).GetRoot();

                var folders = new List<string>(document.Folders.Count + 1) { document.Project.Name };
                folders.AddRange(document.Folders);

                var newDocument = document.Project.AddDocument(
                    newTypeName,
                    newDocumentRoot,
                    ImmutableArray.CreateRange(folders));
                return newDocument.Project.Solution.WithDocumentSyntaxRoot(document.Id, newRoot);
            }
        }

        return document.Project.Solution;
    }

    private static string? GetNamespace(SyntaxNode root)
    {
        return root.ChildNodes().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString()
            ?? root.ChildNodes().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
    }
}
