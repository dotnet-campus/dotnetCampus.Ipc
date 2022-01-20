using System.Collections.Immutable;

using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
internal class ContractTypeDismatchWithInterfaceAnalyzer : DiagnosticAnalyzer
{
    public ContractTypeDismatchWithInterfaceAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(DIPC004_ContractTypeDismatchWithInterface);
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
        foreach (var (attributeNode, namedValues) in IpcAttributeHelper.TryFindIpcShapeAttributes(context.SemanticModel, (ClassDeclarationSyntax) context.Node))
        {
            var contractType = namedValues.ContractType;
            var realType = namedValues.IpcType;
            if (contractType is null || realType is null)
            {
                // 无需报告诊断，因为缺少构造函数参数必然编译不通过。
                return;
            }
            if (contractType.TypeKind != TypeKind.Interface)
            {
                // 由 ContractTypeMustBeAnInterfaceAnalyzer 报告。
                return;
            }

            if (realType.AllInterfaces.Length is 0
                || realType.AllInterfaces.All(x => !SymbolEqualityComparer.Default.Equals(x, contractType)))
            {
                // 在契约类型上报告。
                var typeLocation = (attributeNode.ArgumentList?.Arguments.FirstOrDefault()?.Expression as TypeOfExpressionSyntax)?.Type.GetLocation();
                context.ReportDiagnostic(
                    Diagnostic.Create(DIPC004_ContractTypeDismatchWithInterface,
                    typeLocation,
                    realType.Name, contractType.Name));

                // 在代理壳类型上报告。
                if (attributeNode.Parent is AttributeListSyntax attributeListNode
                    && attributeListNode.Parent is ClassDeclarationSyntax classDeclarationNode)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(DIPC004_ContractTypeDismatchWithInterface,
                        classDeclarationNode.Identifier.GetLocation(),
                        realType.Name, contractType.Name));
                }
            }
        }
    }
}
