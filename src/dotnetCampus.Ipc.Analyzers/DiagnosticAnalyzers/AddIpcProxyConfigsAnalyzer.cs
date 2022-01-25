using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AddIpcProxyConfigsAnalyzer : DiagnosticAnalyzer
{
    public AddIpcProxyConfigsAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(
            IPC301_CreateIpcProxy_AddIpcProxyConfigs,
            IPC303_CreateIpcProxy_AddIpcProxyConfigs);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeIpcPublicAttributes, SyntaxKind.SimpleMemberAccessExpression);
    }

    private void AnalyzeIpcPublicAttributes(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is MemberAccessExpressionSyntax memberAccessNode
            && IpcProxyInvokingInfo.TryCreateIpcProxyInvokingInfo(
                context.SemanticModel,
                memberAccessNode,
                context.CancellationToken) is { } invokingInfo)
        {
            if (invokingInfo.ShapeType is null)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(IPC301_CreateIpcProxy_AddIpcProxyConfigs,
                    memberAccessNode.GetLocation()));
                context.ReportDiagnostic(
                    Diagnostic.Create(IPC303_CreateIpcProxy_AddIpcProxyConfigs,
                    memberAccessNode.GetLocation()));
            }
        }
    }
}
