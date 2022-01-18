using System.Collections.Immutable;

using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class EmptyIpcMemberAttributeIsUnnecessaryAnalyzer : DiagnosticAnalyzer
{
    public EmptyIpcMemberAttributeIsUnnecessaryAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(DIPC121_IpcMember_EmptyIpcMemberAttributeIsUnnecessary);
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
        var classDeclarationNode = (ClassDeclarationSyntax) context.Node;
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationNode);
        if (typeSymbol is null)
        {
            return;
        }

        foreach (var (attributeNode, _) in IpcAttributeHelper.TryFindMemberAttributes(context.SemanticModel, classDeclarationNode))
        {
            if (attributeNode?.Parent is AttributeListSyntax attributeList)
            {
                // 没有设置任何参数（即连括号都没打），或者设置了 0 个参数（即打了括号但括号里没内容）。
                if (attributeNode.ArgumentList is null || attributeNode.ArgumentList.Arguments.Count is 0)
                {
                    context.ReportDiagnostic(
                        Diagnostic.Create(DIPC121_IpcMember_EmptyIpcMemberAttributeIsUnnecessary,
                        attributeList.Attributes.Count is 1
                            ? attributeList.GetLocation()
                            : attributeNode.GetLocation(),
                        attributeNode.Name));
                }
            }
        }
    }
}
