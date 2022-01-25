using dotnetCampus.Ipc.SourceGenerators.Compiling;

using Microsoft.CodeAnalysis.Text;

using static dotnetCampus.Ipc.SourceGenerators.Utils.GeneratorHelper;

namespace dotnetCampus.Ipc;

/// <summary>
/// 为 IPC 代理壳生成对应的代理（Proxy）。
/// </summary>
[Generator]
public class IpcShapeGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            foreach (var ipcObjectType in FindIpcShapeClasses(context.Compilation))
            {
                try
                {
                    var ipcType = ipcObjectType.IpcType;
                    var proxySource = GenerateProxySource(ipcObjectType);
                    var assemblySource = GenerateAssemblySource(ipcObjectType);
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

    /// <summary>
    /// 在整个项目的编译信息中寻找 IPC 真实对象的编译信息。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <returns>所有 IPC 真实对象的编译信息</returns>
    private IEnumerable<IpcShapeCompilation> FindIpcShapeClasses(Compilation compilation)
    {
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            if (IpcShapeCompilation.TryFindIpcShapeCpmpilations(compilation, syntaxTree, out var ipcShapeCompilations))
            {
                foreach (var ipcShapeCompilation in ipcShapeCompilations)
                {
                    yield return ipcShapeCompilation;
                }
            }
        }
    }
}
