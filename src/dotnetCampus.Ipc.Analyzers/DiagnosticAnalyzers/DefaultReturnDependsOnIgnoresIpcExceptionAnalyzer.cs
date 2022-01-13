using System.Collections.Immutable;

using dotnetCampus.Ipc.SourceGenerators.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace dotnetCampus.Ipc.DiagnosticAnalyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class DefaultReturnDependsOnIgnoresIpcExceptionAnalyzer : DiagnosticAnalyzer
{
    public DefaultReturnDependsOnIgnoresIpcExceptionAnalyzer()
    {
        SupportedDiagnostics = ImmutableArray.Create(DIPC102_DefaultReturnDependsOnIgnoresIpcException);
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

        if (TryFindIpcPublicType(context.SemanticModel, classDeclarationNode, out var compilation))
        {
            foreach (var (contractType, realType, member, implementationMember) in compilation.EnumerateMembersByContractType())
            {
                var attributes = implementationMember switch
                {
                    IPropertySymbol propertySymbol => propertySymbol.GetIpcAttributesAsAnInvokingArg(realType),
                    IMethodSymbol methodSymbol => methodSymbol.GetIpcAttributesAsAnInvokingArg(null, realType),
                    _ => null,
                };
                if (attributes is not null)
                {
                    if (!attributes.IgnoresIpcException && attributes.DefaultReturn is not null)
                    {
                        var memberNode = classDeclarationNode.DescendantNodes()
                            .Where(x => x is PropertyDeclarationSyntax or MethodDeclarationSyntax)
                            .FirstOrDefault(x => x switch
                            {
                                PropertyDeclarationSyntax propertyDeclarationSyntax => SymbolEqualityComparer.Default.Equals(implementationMember, context.SemanticModel.GetDeclaredSymbol(propertyDeclarationSyntax)),
                                MethodDeclarationSyntax methodDeclarationSyntax => SymbolEqualityComparer.Default.Equals(implementationMember, context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax)),
                                _ => false,
                            }) as MemberDeclarationSyntax;
                        if (memberNode is not null && memberNode.AttributeLists.SelectMany(x => x.Attributes).FirstOrDefault(x =>
                        {
                            string? attributeName = x.Name switch
                            {
                                IdentifierNameSyntax identifierName => identifierName.ToString(),
                                QualifiedNameSyntax qualifiedName => qualifiedName.ChildNodes().OfType<IdentifierNameSyntax>().LastOrDefault()?.ToString(),
                                _ => null,
                            };
                            return attributeName is not null &&
                                (attributeName.Equals(nameof(IpcPropertyAttribute), StringComparison.Ordinal)
                                || attributeName.Equals(GetAttributeName(nameof(IpcPropertyAttribute)), StringComparison.Ordinal)
                                || attributeName.Equals(nameof(IpcMethodAttribute), StringComparison.Ordinal)
                                || attributeName.Equals(GetAttributeName(nameof(IpcMethodAttribute)), StringComparison.Ordinal));
                        }) is { } attribute && attribute.ArgumentList?.Arguments.FirstOrDefault(x =>
                            x.NameEquals?.Name.ToString() == nameof(IpcMemberAttribute.DefaultReturn)) is { } argument)
                        {
                            context.ReportDiagnostic(Diagnostic.Create(DIPC102_DefaultReturnDependsOnIgnoresIpcException, argument.GetLocation()));
                        }
                        else
                        {
                            context.ReportDiagnostic(Diagnostic.Create(DIPC102_DefaultReturnDependsOnIgnoresIpcException, null));
                        }
                    }
                }
            }
        }
    }

    private static bool TryFindIpcPublicType(SemanticModel semanticModel, ClassDeclarationSyntax classDeclarationNode,
        [NotNullWhen(true)] out PublicIpcObjectCompilation? publicIpcObjectCompilation)
    {
        if (semanticModel.GetDeclaredSymbol(classDeclarationNode) is { } classDeclarationSymbol
            && classDeclarationSymbol.GetAttributes().FirstOrDefault(x => string.Equals(
                x.AttributeClass?.ToString(),
                typeof(IpcPublicAttribute).FullName,
                StringComparison.Ordinal)) is { } ipcPublicAttribute)
        {
            if (ipcPublicAttribute.ConstructorArguments.Length == 1
                && ipcPublicAttribute.ConstructorArguments[0] is TypedConstant typedConstant
                && typedConstant.Value is INamedTypeSymbol contractType)
            {
                publicIpcObjectCompilation = new PublicIpcObjectCompilation(classDeclarationNode.SyntaxTree, classDeclarationSymbol, contractType);
                return true;
            }
        }
        publicIpcObjectCompilation = null;
        return false;
    }
}
