using dotnetCampus.Ipc.Analyzers.Compiling;

namespace dotnetCampus.Ipc.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IgnoresIpcExceptionIsRecommendedAnalyzer : DiagnosticAnalyzer
{
    public IgnoresIpcExceptionIsRecommendedAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(IPC131_IpcMembers_IgnoresIpcExceptionIsRecommended);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeIpcPublicAttributes, SyntaxKind.InterfaceDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeIpcShapeAttributes, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeIpcPublicAttributes(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is InterfaceDeclarationSyntax interfaceDeclarationNode)
        {
            foreach (var (attributeNode, namedValues) in IpcAttributeHelper.TryFindIpcPublicAttributes(context.SemanticModel, interfaceDeclarationNode))
            {
                if (namedValues.IgnoresIpcException is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(IPC131_IpcMembers_IgnoresIpcExceptionIsRecommended, attributeNode.GetLocation()));
                }
            }
        }
    }

    private void AnalyzeIpcShapeAttributes(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is ClassDeclarationSyntax classDeclarationNode)
        {
            foreach (var (attributeNode, namedValues) in IpcAttributeHelper.TryFindIpcShapeAttributes(context.SemanticModel, classDeclarationNode))
            {
                if (namedValues.IgnoresIpcException is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(IPC131_IpcMembers_IgnoresIpcExceptionIsRecommended, attributeNode.GetLocation()));
                }
            }
        }
    }
}
