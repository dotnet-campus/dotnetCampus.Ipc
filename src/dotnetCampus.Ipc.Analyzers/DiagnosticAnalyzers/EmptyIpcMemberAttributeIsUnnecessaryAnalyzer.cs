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

        foreach (var (attribute, _) in IpcAttributeHelper.TryFindMemberAttributes(context.SemanticModel, classDeclarationNode))
        {
            // 没有设置任何属性。
            if (attribute?.ArgumentList?.Arguments is { } arguments && arguments.Count is 0
                && attribute.Parent is AttributeListSyntax attributeList)
            {
                context.ReportDiagnostic(
                    Diagnostic.Create(DIPC121_IpcMember_EmptyIpcMemberAttributeIsUnnecessary,
                    attributeList.Attributes.Count is 1
                        ? attributeList.GetLocation()
                        : attribute.GetLocation(),
                    attribute.Name));
            }
        }
    }
}
