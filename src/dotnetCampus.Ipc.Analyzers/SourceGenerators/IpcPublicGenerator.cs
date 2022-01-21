using dotnetCampus.Ipc.SourceGenerators.Compiling;

using Microsoft.CodeAnalysis.Text;

using static dotnetCampus.Ipc.SourceGenerators.Utils.GeneratorHelper;

namespace dotnetCampus.Ipc;

/// <summary>
/// 为 IPC 接口生成对应的代理（Proxy）和对接（Joint）。
/// </summary>
[Generator]
public class IpcPublicGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            foreach (var ipcPublicCompilation in FindIpcPublicInterfaces(context.Compilation))
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

    /// <summary>
    /// 在整个项目的编译信息中寻找 IPC 真实对象的编译信息。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <returns>所有 IPC 真实对象的编译信息</returns>
    private IEnumerable<IpcPublicCompilation> FindIpcPublicInterfaces(Compilation compilation)
    {
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            if (IpcPublicCompilation.TryFindIpcPublicCompilations(compilation, syntaxTree, out var ipcPublicCompilations))
            {
                foreach (var ipcPublicCompilation in ipcPublicCompilations)
                {
                    yield return ipcPublicCompilation;
                }
            }
        }
    }
}
