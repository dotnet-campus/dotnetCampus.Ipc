using dotnetCampus.Ipc.Generators.Compiling;
using dotnetCampus.Ipc.Generators.Builders;

namespace dotnetCampus.Ipc.Generators.Utils;

internal static class GeneratorHelper
{
    /// <summary>
    /// 生成代理类。
    /// </summary>
    /// <param name="ipc">IPC 接口类型的编译信息。</param>
    /// <returns>代理类的源代码。</returns>
    internal static string GenerateProxySource(IpcPublicCompilation ipc)
    {
        using var builder = new SourceTextBuilder(ipc.GetNamespace())
            {
                UseFileScopedNamespace = false,
                Nullable = null,
            }
            .Using("System.Threading.Tasks")
            .Using("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .AddTypeDeclaration($"internal sealed class __{ipc.IpcType.Name}IpcProxy", t => t
                .AddBaseTypes($"GeneratedIpcProxy<{ipc.IpcType.ToUsingString()}>", ipc.IpcType.ToUsingString())
                .AddGeneratedToolAndEditorBrowsingAttributes()
                .AddRawMembers(ipc.EnumerateMembers()
                    .Select(x => new IpcPublicMemberProxyJointGenerator(x.IpcType, x.Member))
                    .Select(x => x.GenerateProxyMember())));
        return builder.ToString();
    }

    /// <summary>
    /// 生成代理类。
    /// </summary>
    /// <param name="ipc">IPC 接口类型的编译信息。</param>
    /// <returns>代理类的源代码。</returns>
    internal static string GenerateProxySource(IpcShapeCompilation ipc)
    {
        using var builder = new SourceTextBuilder(ipc.GetNamespace())
            {
                UseFileScopedNamespace = false,
                Nullable = null,
            }
            .Using("System.Threading.Tasks")
            .Using("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .AddTypeDeclaration($"internal sealed class __{ipc.IpcType.Name}IpcProxy", t => t
                .AddBaseTypes($"GeneratedIpcProxy<{ipc.ContractType.ToUsingString()}>", ipc.ContractType.ToUsingString())
                .AddGeneratedToolAndEditorBrowsingAttributes()
                .AddRawMembers(ipc.EnumerateMembersByContractType()
                    .Select(x => new IpcPublicMemberProxyJointGenerator(x.contractType, x.shapeType, x.member, x.shapeMember))
                    .Select(x => x.GenerateProxyMember())));
        return builder.ToString();
    }

    /// <summary>
    /// 生成形状代理类。
    /// </summary>
    /// <param name="ipc">真实对象的编译信息。</param>
    /// <param name="typeName">类型名称。</param>
    /// <param name="namespace">命名空间。</param>
    /// <returns>代理类的源代码。</returns>
    internal static string GenerateShapeSource(IpcPublicCompilation ipc, string? typeName, string? @namespace)
    {
        using var builder = new SourceTextBuilder(@namespace ?? ipc.IpcType.ContainingNamespace.ToString())
            {
                SimplifyTypeNamesByUsingNamespace = true,
                ShouldPrependGlobal = false,
                UseFileScopedNamespace = false,
                Nullable = null,
            }
            .Using("dotnetCampus.Ipc.CompilerServices.Attributes")
            .Using("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .AddTypeDeclaration($"internal sealed class {typeName ?? $"{ipc.IpcType.Name}IpcShape"}", t => t
                .AddBaseTypes(ipc.IpcType.ToUsingString())
                .AddAttribute($"[IpcShape(typeof({ipc.IpcType.ToUsingString()}))]")
                .AddRawMembers(ipc.EnumerateMembers()
                    .Select(x => new IpcPublicMemberProxyJointGenerator(x.IpcType, x.Member))
                    .Select(x => x.GenerateShapeMember())));
        return builder.ToString();
    }

    /// <summary>
    /// 生成对接类。
    /// </summary>
    /// <param name="ipc">真实对象的编译信息。</param>
    /// <returns>对接类的源代码。</returns>
    internal static string GenerateJointSource(IpcPublicCompilation ipc)
    {
        const string realInstanceName = "real";
        using var builder = new SourceTextBuilder(ipc.GetNamespace())
            {
                UseFileScopedNamespace = false,
                Nullable = null,
            }
            .Using("System")
            .Using("System.Threading.Tasks")
            .Using("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .AddTypeDeclaration($"internal sealed class __{ipc.IpcType.Name}IpcJoint", t => t
                .AddBaseTypes($"GeneratedIpcJoint<{ipc.IpcType.ToUsingString()}>")
                .AddGeneratedToolAndEditorBrowsingAttributes()
                .AddMethodDeclaration($"protected override void MatchMembers({ipc.IpcType.ToUsingString()} {realInstanceName})", m => m
                    .AddRawStatements(ipc.EnumerateMembers()
                        .Select(x => new IpcPublicMemberProxyJointGenerator(x.IpcType, x.Member))
                        .Select(x => x.GenerateJointMatch(realInstanceName)))));
        return builder.ToString();
    }

    /// <summary>
    /// 生成代理对接关系信息。
    /// </summary>
    /// <param name="ipcPublicCompilations">真实对象的编译信息。</param>
    /// <param name="ipcShapeCompilations">形状代理类型的编译信息。</param>
    /// <returns>程序集特性的源代码。</returns>
    internal static string GenerateModuleInitializerSource(
        IReadOnlyList<IpcPublicCompilation> ipcPublicCompilations,
        IReadOnlyList<IpcShapeCompilation> ipcShapeCompilations)
    {
        using var builder = new SourceTextBuilder()
            {
                UseFileScopedNamespace = false,
                Nullable = null,
            }
            // .AddRawText("#if NET5_0_OR_GREATER")
            // .AddRawText("using static global::dotnetCampus.Ipc.CompilerServices.GeneratedProxies.GeneratedIpcFactory;")
            // .AddTypeDeclaration("internal static class DotNetCampusIpcModuleInitializer", t => t
            //     .AddGeneratedToolAndEditorBrowsingAttributes()
            //     .AddMethodDeclaration("internal static void Initialize()", m => m
            //         .AddAttribute("[global::System.Runtime.CompilerServices.ModuleInitializerAttribute]")
            //         .AddRawStatements(ipcPublicCompilations.Select(GenerateIpcPublicRegistration))
            //         .AddLineSeparator()
            //         .AddRawStatements(ipcShapeCompilations.Select(GenerateIpcPublicRegistration))))
            // .AddRawText("#else")
            .AddRawStatements(ipcPublicCompilations.Select(GenerateIpcPublicAssemblyAttribute))
            .AddRawStatements(ipcShapeCompilations.Select(GenerateIpcPublicAssemblyAttribute))
            // .AddRawText("#endif")
            ;
        return builder.ToString();
    }

    private static string GenerateIpcPublicRegistration(IpcPublicCompilation ipc) => $"""
        RegisterIpcPublic<{ipc.IpcType.ToUsingString()}>(
            () => new global::{ipc.GetNamespace()}.__{ipc.IpcType.Name}IpcProxy(),
            () => new global::{ipc.GetNamespace()}.__{ipc.IpcType.Name}IpcJoint());
        """;

    private static string GenerateIpcPublicRegistration(IpcShapeCompilation ipc) => $"""
        RegisterIpcShape<{ipc.ContractType.ToUsingString()}, {ipc.IpcType.ToUsingString()}>(
            () => new global::{ipc.GetNamespace()}.__{ipc.IpcType.Name}IpcProxy());
        """;

    private static string GenerateIpcPublicAssemblyAttribute(IpcPublicCompilation ipc) => $"""
        [assembly: global::dotnetCampus.Ipc.CompilerServices.Attributes.AssemblyIpcProxyJointAttribute(
            typeof({ipc.IpcType.ToUsingString()}),
            typeof(global::{ipc.GetNamespace()}.__{ipc.IpcType.Name}IpcProxy),
            typeof(global::{ipc.GetNamespace()}.__{ipc.IpcType.Name}IpcJoint))]
        """;

    private static string GenerateIpcPublicAssemblyAttribute(IpcShapeCompilation ipc) => $"""
        [assembly: global::dotnetCampus.Ipc.CompilerServices.Attributes.AssemblyIpcProxyAttribute(
            typeof({ipc.ContractType.ToUsingString()}),
            typeof({ipc.IpcType.ToUsingString()}),
            typeof(global::{ipc.GetNamespace()}.__{ipc.IpcType.Name}IpcProxy))]
        """;

    /// <summary>
    /// 在代码生成器中报告那些分析器中没有报告的编译错误。
    /// <para>注意：虽然代码生成器和分析器都能报告编译错误，但只有分析器才能在 Visual Studio 中画波浪线。所以我们会考虑将一些需要立即觉察的错误放到分析器中报告。</para>
    /// <para>因此，在这个代码生成器项目中：</para>
    /// <list type="bullet">
    /// <item>如果某个错误后续一定会报编译错误，则抛出 <see cref="IPC001_KnownCompilerError"/>。</item>
    /// <item>如果某个错误已经写了分析器，就报 <see cref="IPC002_KnownDiagnosticError"/>。</item>
    /// <item>如果未来会写某个分析器，但现在还没完成，则报具体的分析器错误。</item>
    /// <item>其他情况，该抛什么异常就抛什么异常，不局限于诊断异常。</item>
    /// </list>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    internal static void ReportDiagnosticsThatHaveNotBeenReported(SourceProductionContext context, DiagnosticException ex)
    {
        var diagnosticsThatWillBeReported = new List<DiagnosticDescriptor>
        {
            // 这些诊断将仅在分析器中报告，凡在生成器中发生的这些诊断都将自动忽略。
            IPC001_KnownCompilerError,
            IPC002_KnownDiagnosticError,
        };

        if (diagnosticsThatWillBeReported.Find(x => x.Id == ex.Diagnostic.Id) is not null)
        {
            return;
        }

        // 报告所有非已知诊断。
        context.ReportDiagnostic(ex.ToDiagnostic());
    }
}
