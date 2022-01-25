using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EmptyIpcMemberAttributeIsUnnecessaryAnalyzer : DiagnosticAnalyzer
{
    public EmptyIpcMemberAttributeIsUnnecessaryAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(IPC201_IpcMember_EmptyIpcMemberAttributeIsUnnecessary);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeIpcPublicAttributes, SyntaxKind.InterfaceDeclaration);
    }

    private void AnalyzeIpcPublicAttributes(SyntaxNodeAnalysisContext context)
    {
        var interfaceDeclarationNode = (InterfaceDeclarationSyntax) context.Node;
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(interfaceDeclarationNode);
        if (typeSymbol is null)
        {
            return;
        }

        foreach (var (attributeNode, _) in IpcAttributeHelper.TryFindMemberAttributes(context.SemanticModel, interfaceDeclarationNode))
        {
            if (attributeNode?.Parent is AttributeListSyntax attributeList)
            {
                // 没有设置任何参数（即连括号都没打），或者设置了 0 个参数（即打了括号但括号里没内容）。
                if (attributeNode.ArgumentList is null || attributeNode.ArgumentList.Arguments.Count is 0)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(IPC201_IpcMember_EmptyIpcMemberAttributeIsUnnecessary,
                        attributeList.Attributes.Count is 1
                            ? attributeList.GetLocation()
                            : attributeNode.GetLocation(),
                        attributeNode.Name));
                }
            }
        }
    }
}
