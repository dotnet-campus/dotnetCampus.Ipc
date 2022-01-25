using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DefaultReturnDependsOnIgnoresIpcExceptionAnalyzer : DiagnosticAnalyzer
{
    public DefaultReturnDependsOnIgnoresIpcExceptionAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(IPC242_IpcProperty_DefaultReturnDependsOnIgnoresIpcException);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeIpcTypeAttributes, SyntaxKind.InterfaceDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeIpcTypeAttributes, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeIpcTypeAttributes(SyntaxNodeAnalysisContext context)
    {
        var typeDeclarationNode = (TypeDeclarationSyntax) context.Node;
        var memberSymbol = context.SemanticModel.GetDeclaredSymbol(typeDeclarationNode);
        if (memberSymbol is null)
        {
            return;
        }

        foreach (var (attributeNode, namedValues) in IpcAttributeHelper.TryFindMemberAttributes(context.SemanticModel, typeDeclarationNode))
        {
            // 设置了默认值却没有忽略异常。
            if (!namedValues.IgnoresIpcException && namedValues.DefaultReturn is not null)
            {
                if (attributeNode?.ArgumentList?.Arguments.FirstOrDefault(x =>
                    x.NameEquals?.Name.ToString() == nameof(IpcMethodAttribute.DefaultReturn)) is { } attributeArgumentNode)
                {
                    context.ReportDiagnostic(Diagnostic.Create(IPC242_IpcProperty_DefaultReturnDependsOnIgnoresIpcException, attributeArgumentNode.GetLocation()));
                }
            }
        }
    }
}
