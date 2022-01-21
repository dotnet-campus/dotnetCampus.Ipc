using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AddIpcShapeAnalyzer : DiagnosticAnalyzer
{
    public AddIpcShapeAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(IPC302_CreateIpcProxy_AddIpcShape);
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
                    Diagnostic.Create(IPC302_CreateIpcProxy_AddIpcShape,
                    memberAccessNode.GetLocation()));
            }
        }
    }
}
