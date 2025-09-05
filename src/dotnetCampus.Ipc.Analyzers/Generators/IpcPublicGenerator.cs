using dotnetCampus.Ipc.Generators.Compiling;
using Microsoft.CodeAnalysis.Text;
using static dotnetCampus.Ipc.Generators.Utils.GeneratorHelper;

namespace dotnetCampus.Ipc.Generators;

/// <summary>
/// 为 IPC 接口生成对应的代理（Proxy）和对接（Joint）。
/// </summary>
[Generator(LanguageNames.CSharp)]
public class IpcPublicGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var ipcPublic = context.SyntaxProvider.CreateSyntaxProvider(
                // 基本过滤：有特性的接口。
                (syntaxNode, ct) => syntaxNode is InterfaceDeclarationSyntax { AttributeLists.Count: > 0 },
                // 语义解析：确定是否真的是感兴趣的 IPC 接口。
                (generatorSyntaxContext, ct) => IpcPublicCompilation.TryCreateIpcPublicCompilation(
                    (InterfaceDeclarationSyntax)generatorSyntaxContext.Node,
                    generatorSyntaxContext.SemanticModel,
                    out var ipcPublicCompilation)
                    ? ipcPublicCompilation
                    : null)
            .Where(x => x is not null)
            .Select((x, ct) => x!);

        var ipcShape = context.SyntaxProvider.CreateSyntaxProvider(
                // 基本过滤：有特性的接口。
                (syntaxNode, ct) => syntaxNode is ClassDeclarationSyntax { AttributeLists.Count: > 0 },
                // 语义解析：确定是否真的是感兴趣的 IPC 接口。
                (generatorSyntaxContext, ct) => IpcShapeCompilation.TryCreateIpcShapeCompilation(
                    (ClassDeclarationSyntax)generatorSyntaxContext.Node,
                    generatorSyntaxContext.SemanticModel,
                    out var ipcPublicCompilation)
                    ? ipcPublicCompilation
                    : null)
            .Where(x => x is not null)
            .Select((x, ct) => x!);

        context.RegisterSourceOutput(ipcPublic, Execute);
        context.RegisterSourceOutput(ipcShape, Execute);
        context.RegisterSourceOutput(ipcPublic.Collect().Combine(ipcShape.Collect()), Execute);
    }

    private void Execute(SourceProductionContext context, IpcPublicCompilation ipcPublicCompilation)
    {
        try
        {
            var ipcType = ipcPublicCompilation.IpcType;
            var proxySource = GenerateProxySource(ipcPublicCompilation);
            var jointSource = GenerateJointSource(ipcPublicCompilation);
            context.AddSource($"{ipcType.Name}.proxy.cs", SourceText.From(proxySource, Encoding.UTF8));
            context.AddSource($"{ipcType.Name}.joint.cs", SourceText.From(jointSource, Encoding.UTF8));
        }
        catch (DiagnosticException ex)
        {
            ReportDiagnosticsThatHaveNotBeenReported(context, ex);
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(IPC000_UnknownError, null, ex));
        }
    }

    private void Execute(SourceProductionContext context, IpcShapeCompilation ipcShapeCompilation)
    {
        try
        {
            var contractType = ipcShapeCompilation.ContractType;
            var ipcType = ipcShapeCompilation.IpcType;
            var proxySource = GenerateProxySource(ipcShapeCompilation);
            context.AddSource($"{contractType.Name}.{ipcType.Name}.shape.cs", SourceText.From(proxySource, Encoding.UTF8));
        }
        catch (DiagnosticException ex)
        {
            ReportDiagnosticsThatHaveNotBeenReported(context, ex);
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(IPC000_UnknownError, null, ex));
        }
    }

    private void Execute(SourceProductionContext context, (ImmutableArray<IpcPublicCompilation> IpcPublics, ImmutableArray<IpcShapeCompilation> IpcShapes) compilations)
    {
        try
        {
            var moduleInitializerSource = GenerateModuleInitializerSource(compilations.IpcPublics, compilations.IpcShapes);
            context.AddSource("_ModuleInitializer.cs", SourceText.From(moduleInitializerSource, Encoding.UTF8));
        }
        catch (DiagnosticException ex)
        {
            ReportDiagnosticsThatHaveNotBeenReported(context, ex);
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(IPC000_UnknownError, null, ex));
        }
    }
}
