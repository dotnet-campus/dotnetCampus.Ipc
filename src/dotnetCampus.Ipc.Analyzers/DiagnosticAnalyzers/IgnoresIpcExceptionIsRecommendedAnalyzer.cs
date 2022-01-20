using System.Collections.Immutable;

using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IgnoresIpcExceptionIsRecommendedAnalyzer : DiagnosticAnalyzer
{
    public IgnoresIpcExceptionIsRecommendedAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(DIPC101_IpcPublic_IgnoresIpcExceptionIsRecommended);
    }

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get; }

    public override void Initialize(AnalysisContext context)
    {
        context.EnableConcurrentExecution();
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);

        context.RegisterSyntaxNodeAction(AnalyzeTypeIpcAttributes, SyntaxKind.ClassDeclaration | SyntaxKind.InterfaceDeclaration);
    }

    private void AnalyzeTypeIpcAttributes(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is InterfaceDeclarationSyntax interfaceDeclarationNode)
        {
            foreach (var (attributeNode, namedValues) in IpcAttributeHelper.TryFindIpcPublicAttributes(context.SemanticModel, interfaceDeclarationNode))
            {
                if (namedValues.IgnoresIpcException is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DIPC101_IpcPublic_IgnoresIpcExceptionIsRecommended, attributeNode.GetLocation()));
                }
            }
        }

        if (context.Node is ClassDeclarationSyntax classDeclarationNode)
        {
            foreach (var (attributeNode, namedValues) in IpcAttributeHelper.TryFindIpcShapeAttributes(context.SemanticModel, classDeclarationNode))
            {
                if (namedValues.IgnoresIpcException is null)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DIPC101_IpcPublic_IgnoresIpcExceptionIsRecommended, attributeNode.GetLocation()));
                }
            }
        }
    }
}
