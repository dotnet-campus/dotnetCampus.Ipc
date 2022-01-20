using System.Collections.Immutable;

using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DefaultReturnDependsOnIgnoresIpcExceptionAnalyzer : DiagnosticAnalyzer
{
    public DefaultReturnDependsOnIgnoresIpcExceptionAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(DIPC120_IpcMember_DefaultReturnDependsOnIgnoresIpcException);
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
        var memberSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationNode);
        if (memberSymbol is null)
        {
            return;
        }

        foreach (var (attributeNode, namedValues) in IpcAttributeHelper.TryFindMemberAttributes(context.SemanticModel, classDeclarationNode))
        {
            // 设置了默认值却没有忽略异常。
            if (!namedValues.IgnoresIpcException && namedValues.DefaultReturn is not null)
            {
                if (attributeNode?.ArgumentList?.Arguments.FirstOrDefault(x =>
                    x.NameEquals?.Name.ToString() == nameof(IpcMethodAttribute.DefaultReturn)) is { } attributeArgumentNode)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DIPC120_IpcMember_DefaultReturnDependsOnIgnoresIpcException, attributeArgumentNode.GetLocation()));
                }
            }
        }
    }
}
