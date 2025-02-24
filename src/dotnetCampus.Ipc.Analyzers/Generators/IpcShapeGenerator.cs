using dotnetCampus.Ipc.Generators.Compiling;
using Microsoft.CodeAnalysis.Text;
using static dotnetCampus.Ipc.Generators.Utils.GeneratorHelper;

namespace dotnetCampus.Ipc.Generators;

/// <summary>
/// 为 IPC 代理壳生成对应的代理（Proxy）。
/// </summary>
[Generator]
public class IpcShapeGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var compilations = context.SyntaxProvider.CreateSyntaxProvider(
            // 基本过滤：有特性的接口。
            (syntaxNode, ct) => syntaxNode is ClassDeclarationSyntax ids && ids.AttributeLists.Count > 0,
            // 语义解析：确定是否真的是感兴趣的 IPC 接口。
            (generatorSyntaxContext, ct) => IpcShapeCompilation.TryCreateIpcShapeCompilation(
                (ClassDeclarationSyntax) generatorSyntaxContext.Node,
                generatorSyntaxContext.SemanticModel,
                out var ipcPublicCompilation)
                    ? ipcPublicCompilation
                    : null)
            .Where(x => x is not null)
            .Select((x, ct) => x!);

        context.RegisterSourceOutput(compilations, Execute);
    }

    private void Execute(SourceProductionContext context, IpcShapeCompilation ipcShapeCompilation)
    {
        try
        {
            var ipcType = ipcShapeCompilation.IpcType;
            var proxySource = GenerateProxySource(ipcShapeCompilation);
            var assemblySource = GenerateAssemblySource(ipcShapeCompilation);
            context.AddSource($"{ipcType.Name}.proxy", SourceText.From(proxySource, Encoding.UTF8));
            context.AddSource($"{ipcType.Name}.assembly", SourceText.From(assemblySource, Encoding.UTF8));
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

    /// <summary>
    /// 生成代理对接关系信息。
    /// </summary>
    /// <param name="sc">真实对象的编译信息。</param>
    /// <returns>程序集特性的源代码。</returns>
    private string GenerateAssemblySource(IpcShapeCompilation sc)
    {
        var sourceCode = @$"using dotnetCampus.Ipc.CompilerServices.Attributes;
using {sc.GetNamespace()};

[assembly: {GetAttributeName(typeof(AssemblyIpcProxyAttribute).Name)}(typeof({sc.ContractType}), typeof({sc.IpcType}), typeof(__{sc.IpcType.Name}IpcProxy))]";
        return sourceCode;
    }
}
