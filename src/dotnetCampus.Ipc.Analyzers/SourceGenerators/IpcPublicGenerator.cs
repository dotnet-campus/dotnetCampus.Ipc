using dotnetCampus.Ipc.SourceGenerators.Compiling;

using Microsoft.CodeAnalysis.Text;

using static dotnetCampus.Ipc.SourceGenerators.Utils.GeneratorHelper;

namespace dotnetCampus.Ipc;

/// <summary>
/// 为 IPC 接口生成对应的代理（Proxy）和对接（Joint）。
/// </summary>
[Generator(LanguageNames.CSharp)]
public class IpcPublicGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var compilations = context.SyntaxProvider.CreateSyntaxProvider(
            // 基本过滤：有特性的接口。
            (syntaxNode, ct) => syntaxNode is InterfaceDeclarationSyntax ids && ids.AttributeLists.Count > 0,
            // 语义解析：确定是否真的是感兴趣的 IPC 接口。
            (generatorSyntaxContext, ct) => IpcPublicCompilation.TryCreateIpcPublicCompilation(
                (InterfaceDeclarationSyntax) generatorSyntaxContext.Node,
                generatorSyntaxContext.SemanticModel,
                out var ipcPublicCompilation)
                    ? ipcPublicCompilation
                    : null)
            .Where(x => x is not null)
            .Select((x, ct) => x!);

        context.RegisterSourceOutput(compilations, Execute);
    }

    private void Execute(SourceProductionContext context, IpcPublicCompilation ipcPublicCompilation)
    {
        try
        {
            var ipcType = ipcPublicCompilation.IpcType;
            var proxySource = GenerateProxySource(ipcPublicCompilation);
            var jointSource = GenerateJointSource(ipcPublicCompilation);
            var assemblySource = GenerateAssemblySource(ipcPublicCompilation);
            context.AddSource($"{ipcType.Name}.proxy", SourceText.From(proxySource, Encoding.UTF8));
            context.AddSource($"{ipcType.Name}.joint", SourceText.From(jointSource, Encoding.UTF8));
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
    /// <param name="pc">真实对象的编译信息。</param>
    /// <returns>程序集特性的源代码。</returns>
    private string GenerateAssemblySource(IpcPublicCompilation pc)
    {
        var sourceCode = @$"using dotnetCampus.Ipc.CompilerServices.Attributes;
using {pc.GetNamespace()};

[assembly: {GetAttributeName(typeof(AssemblyIpcProxyJointAttribute).Name)}(typeof({pc.IpcType}), typeof(__{pc.IpcType.Name}IpcProxy), typeof(__{pc.IpcType.Name}IpcJoint))]";
        return sourceCode;
    }
}
