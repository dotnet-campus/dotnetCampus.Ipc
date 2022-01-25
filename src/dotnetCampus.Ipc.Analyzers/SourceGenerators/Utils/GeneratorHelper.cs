using dotnetCampus.Ipc.SourceGenerators.Compiling;
using dotnetCampus.Ipc.SourceGenerators.Models;

namespace dotnetCampus.Ipc.SourceGenerators.Utils;

internal static class GeneratorHelper
{
    /// <summary>
    /// 生成代理类。
    /// </summary>
    /// <param name="ipc">IPC 接口类型的编译信息。</param>
    /// <returns>代理类的源代码。</returns>
    internal static string GenerateProxySource(IpcPublicCompilation ipc)
    {
        var builder = new SourceTextBuilder()
            .AddUsing("System.Threading.Tasks")
            .AddUsing("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .DeclareNamespace(ipc.GetNamespace())
            .AddClassDeclaration(root => new ClassDeclarationSourceTextBuilder(root,
                    $"__{ipc.IpcType.Name}IpcProxy",
                    $"GeneratedIpcProxy<{ipc.IpcType.Name}>",
                    ipc.IpcType.Name)
                .AddMemberDeclarations(root => ipc.EnumerateMembers()
                    .Select(x => new IpcPublicMemberProxyJointGenerator(x.ipcType, x.member))
                    .Select(x => x.GenerateProxyMember(root))));
        return builder.ToString();
    }

    /// <summary>
    /// 生成代理壳类。
    /// </summary>
    /// <param name="ipc">真实对象的编译信息。</param>
    /// <param name="typeName">类型名称。</param>
    /// <param name="namespace">命名空间。</param>
    /// <returns>代理类的源代码。</returns>
    internal static string GenerateShapeSource(IpcPublicCompilation ipc, string? typeName, string? @namespace)
    {
        var builder = new SourceTextBuilder()
            .AddUsing("dotnetCampus.Ipc.CompilerServices.Attributes")
            .AddUsing("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .DeclareNamespace(@namespace ?? ipc.IpcType.ContainingNamespace.ToString())
            .AddClassDeclaration(root => new ClassDeclarationSourceTextBuilder(root,
                    typeName ?? $"{ipc.IpcType.Name}IpcShape",
                    ipc.IpcType.Name)
                .AddMemberDeclarations(root => ipc.EnumerateMembers()
                    .Select(x => new IpcPublicMemberProxyJointGenerator(x.ipcType, x.member))
                    .Select(x => x.GenerateShapeMember(root))));
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
        var builder = new SourceTextBuilder()
            .AddUsing("System")
            .AddUsing("System.Threading.Tasks")
            .AddUsing("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .DeclareNamespace(ipc.GetNamespace())
            .AddClassDeclaration(root => new ClassDeclarationSourceTextBuilder(root,
                    $"__{ipc.IpcType.Name}IpcJoint",
                    $"GeneratedIpcJoint<{ipc.IpcType.Name}>")
                .AddMemberDeclaration(root => new MemberDeclarationSourceTextBuilder(root,
                        $"protected override void MatchMembers({ipc.IpcType.Name} {realInstanceName})")
                    .AddExpressions(root => ipc.EnumerateMembers()
                        .Select(x => new IpcPublicMemberProxyJointGenerator(x.ipcType, x.member))
                        .Select(x => x.GenerateJointMatch(root, realInstanceName)))));
        return builder.ToString();
    }

    /// <summary>
    /// 在代码生成器中报告那些分析器中没有报告的编译错误。
    /// <para>注意：虽然代码生成器和分析器都能报告编译错误，但只有分析器才能在 Visual Studio 中画波浪线。所以我们会考虑将一些需要立即觉察的错误放到分析器中报告。</para>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="ex"></param>
    internal static void ReportDiagnosticsThatHaveNotBeenReported(GeneratorExecutionContext context, DiagnosticException ex)
    {
        var diagnosticsThatOnlyGeneratorWillReport = new List<DiagnosticDescriptor>
        {
            // 这些诊断将仅在分析器中报告，凡在生成器中发生的这些诊断都将自动忽略。
            IPC000_UnknownError,
        };
        if (diagnosticsThatOnlyGeneratorWillReport.Find(x => x.Id == ex.Diagnostic.Id) is { } diagnostic)
        {
            // 只有生成器报告。
            context.ReportDiagnostic(ex.ToDiagnostic());
        }
    }

    /// <summary>
    /// 格式化代码。
    /// </summary>
    /// <param name="sourceCode">未格式化的源代码。</param>
    /// <returns>格式化的源代码。</returns>
    internal static string FormatCode(string sourceCode)
    {
        var rootSyntaxNode = CSharpSyntaxTree.ParseText(sourceCode).GetRoot();
        return rootSyntaxNode.NormalizeWhitespace().SyntaxTree.GetText().ToString();
    }
}
