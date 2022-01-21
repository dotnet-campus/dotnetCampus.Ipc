using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class ContractTypeMustBeAnInterfaceAnalyzer : DiagnosticAnalyzer
{
    public ContractTypeMustBeAnInterfaceAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(IPC160_IpcShape_ContractTypeMustBeAnInterface);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeTypeIpcAttributes, SyntaxKind.ClassDeclaration);
    }

    private void AnalyzeTypeIpcAttributes(SyntaxNodeAnalysisContext context)
    {
        foreach (var (attribute, namedValues) in IpcAttributeHelper.TryFindIpcShapeAttributes(context.SemanticModel, (ClassDeclarationSyntax) context.Node))
        {
            var contractType = namedValues.ContractType;
            if (contractType == null)
            {
                // 无需报告诊断，因为缺少构造函数参数必然编译不通过。
                return;
            }

            var typeLocation = (attribute.ArgumentList?.Arguments.FirstOrDefault()?.Expression as TypeOfExpressionSyntax)?.Type.GetLocation();
            if (contractType.TypeKind != TypeKind.Interface)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(IPC160_IpcShape_ContractTypeMustBeAnInterface,
                    typeLocation,
                    contractType.Name));
            }
        }
    }
}
