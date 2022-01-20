using dotnetCampus.Ipc.SourceGenerators.Compiling;

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Ipc;

/// <summary>
/// 为 IPC 对象生成对应的代理（Proxy）和对接（Joint）。
/// </summary>
[Generator]
public class ProxyJointGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        try
        {
            foreach (var ipcObjectType in FindIpcPublicObjects(context.Compilation))
            {
                try
                {
                    var realType = ipcObjectType.RealType;
                    var proxySource = GenerateProxySource(ipcObjectType);
                    var jointSource = GenerateJointSource(ipcObjectType);
                    var assemblySource = GenerateAssemblySource(ipcObjectType);
                    context.AddSource($"{realType.Name}.proxy", SourceText.From(proxySource, Encoding.UTF8));
                    context.AddSource($"{realType.Name}.joint", SourceText.From(jointSource, Encoding.UTF8));
                    context.AddSource($"{realType.Name}.assembly", SourceText.From(assemblySource, Encoding.UTF8));
                }
                catch (DiagnosticException ex)
                {
                    ReportDiagnosticsThatHaveNotBeenReported(context, ex);
                }
                catch (Exception ex)
                {
                    context.ReportDiagnostic(Diagnostic.Create(DIPC001_UnknownError, null, ex));
                }
            }
        }
        catch (DiagnosticException ex)
        {
            ReportDiagnosticsThatHaveNotBeenReported(context, ex);
        }
        catch (Exception ex)
        {
            context.ReportDiagnostic(Diagnostic.Create(DIPC001_UnknownError, null, ex));
        }
    }

    /// <summary>
    /// 在代码生成器中报告那些分析器中没有报告的编译错误。
    /// <para>注意：虽然代码生成器和分析器都能报告编译错误，但只有分析器才能在 Visual Studio 中画波浪线。所以我们会考虑将一些需要立即觉察的错误放到分析器中报告。</para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    private void ReportDiagnosticsThatHaveNotBeenReported(GeneratorExecutionContext context, DiagnosticException ex)
    {
        var diagnosticsThatHaveBeenReported = new List<DiagnosticDescriptor>
        {
            // 这些诊断将仅在分析器中报告，凡在生成器中发生的这些诊断都将自动忽略。
            DIPC003_ContractTypeMustBeAnInterface,
            DIPC101_IpcPublic_IgnoresIpcExceptionIsRecommended,
            DIPC120_IpcMember_DefaultReturnDependsOnIgnoresIpcException,
            DIPC121_IpcMember_EmptyIpcMemberAttributeIsUnnecessary,
        };
        if (diagnosticsThatHaveBeenReported.Find(x => x.Id == ex.Diagnostic.Id) is { } diagnostic)
        {
            // 已被分析器报告。
        }
        else
        {
            context.ReportDiagnostic(ex.ToDiagnostic());
        }
    }

    /// <summary>
    /// 生成代理类。
    /// </summary>
    /// <param name="realTypeCompilation">真实对象的编译信息。</param>
    /// <returns>代理类的源代码。</returns>
    private string GenerateProxySource(PublicIpcObjectCompilation realTypeCompilation)
    {
        var members = string.Join(
            Environment.NewLine + Environment.NewLine,
            realTypeCompilation.EnumerateMembersByContractType()
            .Select(x => new PublicIpcObjectMemberProxyJointGenerator(x.contractType, x.realType, x.member, x.implementationMember))
            .Select(x => x.GenerateProxyMember()));
        var sourceCode = FormatCode(@$"{realTypeCompilation.GetUsing()}
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

namespace {realTypeCompilation.GetNamespace()}
{{
    internal class {realTypeCompilation.RealType.Name}IpcProxy : GeneratedIpcProxy<{realTypeCompilation.ContractType.Name}>, {realTypeCompilation.ContractType.Name}
    {{
{members}
    }}
}}
");
        return sourceCode;
    }

    /// <summary>
    /// 生成对接类。
    /// </summary>
    /// <param name="realTypeCompilation">真实对象的编译信息。</param>
    /// <returns>对接类的源代码。</returns>
    private string GenerateJointSource(PublicIpcObjectCompilation realTypeCompilation)
    {
        const string realInstanceName = "real";
        var matches = string.Join(
            Environment.NewLine,
            realTypeCompilation.EnumerateMembersByContractType()
            .Select(x => new PublicIpcObjectMemberProxyJointGenerator(x.contractType, x.realType, x.member, x.implementationMember))
            .Select(x => x.GenerateJointMatch(realInstanceName)));
        var sourceCode = FormatCode(@$"{realTypeCompilation.GetUsing()}
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

namespace {realTypeCompilation.GetNamespace()}
{{
    internal class {realTypeCompilation.RealType.Name}IpcJoint : GeneratedIpcJoint<{realTypeCompilation.ContractType.Name}>
    {{
        protected override void MatchMembers({realTypeCompilation.ContractType.Name} {realInstanceName})
        {{
{matches}
        }}
    }}
}}
");
        return sourceCode;
    }

    /// <summary>
    /// 生成代理对接关系信息。
    /// </summary>
    /// <param name="realTypeCompilation">真实对象的编译信息。</param>
    /// <returns>程序集特性的源代码。</returns>
    private string GenerateAssemblySource(PublicIpcObjectCompilation realTypeCompilation)
    {
        var sourceCode = @$"using dotnetCampus.Ipc.CompilerServices.Attributes;
using {realTypeCompilation.GetNamespace()};

[assembly: {GetAttributeName(typeof(AssemblyIpcProxyJointAttribute).Name)}(typeof({realTypeCompilation.ContractType}), typeof({realTypeCompilation.RealType}), typeof({realTypeCompilation.RealType.Name}IpcProxy), typeof({realTypeCompilation.RealType.Name}IpcJoint))]";
        return sourceCode;
    }

    /// <summary>
    /// 在整个项目的编译信息中寻找 IPC 真实对象的编译信息。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <returns>所有 IPC 真实对象的编译信息</returns>
    private IEnumerable<PublicIpcObjectCompilation> FindIpcPublicObjects(Compilation compilation)
    {
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            if (PublicIpcObjectCompilation.TryFind(compilation, syntaxTree, out var publicIpcObjectCompilations))
            {
                foreach (var publicIpcObject in publicIpcObjectCompilations)
                {
                    yield return publicIpcObject;
                }
            }
        }
    }

    /// <summary>
    /// 格式化代码。
    /// </summary>
    /// <param name="sourceCode">未格式化的源代码。</param>
    /// <returns>格式化的源代码。</returns>
    private string FormatCode(string sourceCode)
    {
        var rootSyntaxNode = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();
        return rootSyntaxNode.NormalizeWhitespace().SyntaxTree.GetText().ToString();
    }
}
