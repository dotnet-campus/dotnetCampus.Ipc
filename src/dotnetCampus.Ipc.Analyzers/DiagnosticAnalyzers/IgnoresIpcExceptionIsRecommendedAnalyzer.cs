using System.Collections.Immutable;
using System.Diagnostics;

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
        SupportedDiagnostics = ImmutableArray.Create(DIPC101_IgnoresIpcExceptionIsRecommended);
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
        foreach (var (attribute, namedValues) in IpcAttributeHelper.TryFindClassAttributes(context.SemanticModel, (ClassDeclarationSyntax) context.Node))
        {
            if (namedValues.IgnoresIpcException is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DIPC101_IgnoresIpcExceptionIsRecommended, attribute.GetLocation()));
            }
        }
    }
}
