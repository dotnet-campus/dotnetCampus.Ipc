using dotnetCampus.Ipc.Generators.Compiling;
using dotnetCampus.Ipc.Generators.Models;

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
        var builder = new SourceTextBuilder()
            .AddUsing("System.Threading.Tasks")
            .AddUsing("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .DeclareNamespace(ipc.GetNamespace())
            .AddClassDeclaration(root => new ClassDeclarationSourceTextBuilder(root,
                    $"__{ipc.IpcType.Name}IpcProxy",
                    $"GeneratedIpcProxy<{ipc.IpcType.Name}>",
                    ipc.IpcType.Name)
                .WithGeneratedToolInfoWithoutEditorBrowsingAttributes()
                .AddMemberDeclarations(root => ipc.EnumerateMembers()
                    .Select(x => new IpcPublicMemberProxyJointGenerator(x.ipcType, x.member))
                    .Select(x => x.GenerateProxyMember(root))));
        return builder.ToString();
    }

    /// <summary>
    /// 生成代理类。
    /// </summary>
    /// <param name="ipc">IPC 接口类型的编译信息。</param>
    /// <returns>代理类的源代码。</returns>
    internal static string GenerateProxySource(IpcShapeCompilation ipc)
    {
        var builder = new SourceTextBuilder()
            .AddUsing("System.Threading.Tasks")
            .AddUsing("dotnetCampus.Ipc.CompilerServices.GeneratedProxies")
            .DeclareNamespace(ipc.GetNamespace())
            .AddClassDeclaration(root => new ClassDeclarationSourceTextBuilder(root,
                    $"__{ipc.IpcType.Name}IpcProxy",
                    $"GeneratedIpcProxy<{ipc.ContractType.Name}>",
                    ipc.ContractType.Name)
                .WithGeneratedToolInfoWithoutEditorBrowsingAttributes()
                .AddMemberDeclarations(root => ipc.EnumerateMembersByContractType()
                    .Select(x => new IpcPublicMemberProxyJointGenerator(x.contractType, x.shapeType, x.member, x.shapeMember))
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
                .WithAttribute($"[IpcShape(typeof({ipc.IpcType.Name}))]")
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
                .WithGeneratedToolInfoWithoutEditorBrowsingAttributes()
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
