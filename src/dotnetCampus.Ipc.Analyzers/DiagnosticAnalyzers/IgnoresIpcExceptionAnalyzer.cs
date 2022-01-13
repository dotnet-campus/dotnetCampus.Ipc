using System.Collections.Immutable;

using dotnetCampus.Ipc.CodeAnalysis.Utils;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class IgnoresIpcExceptionAnalyzer : DiagnosticAnalyzer
{
    public IgnoresIpcExceptionAnalyzer()
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
        var classDeclarationNode = (ClassDeclarationSyntax) context.Node;
        var typeSymbol = context.SemanticModel.GetDeclaredSymbol(classDeclarationNode);
        var ignoresIpcException = typeSymbol.GetAttributeValueOrDefault<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException));
        if (ignoresIpcException is null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DIPC101_IgnoresIpcExceptionIsRecommended, classDeclarationNode.GetLocation()));
        }
    }
}
