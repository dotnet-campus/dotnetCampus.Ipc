using System.Reflection;
using dotnetCampus.Ipc.Generators.Models;

namespace dotnetCampus.Ipc.Generators.Utils;

/// <summary>
/// 为代码生成器提供名称和版本信息。
/// </summary>
internal static class GeneratorToolInfo
{
    static GeneratorToolInfo()
    {
        GeneratorName = typeof(GeneratorToolInfo).Assembly.GetName().Name;
        GeneratorVersion = typeof(GeneratorToolInfo).Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
    }

    /// <summary>
    /// 获取代码生成器工具名称。
    /// </summary>
    public static string GeneratorName { get; }

    /// <summary>
    /// 获取代码生成器版本。
    /// </summary>
    public static string GeneratorVersion { get; }

    /// <summary>
    /// 为生成的代码添加生成工具信息。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static ClassDeclarationSourceTextBuilder WithGeneratedToolInfoWithoutEditorBrowsingAttributes(this ClassDeclarationSourceTextBuilder builder)
    {
        return builder
            .WithAttribute("[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]")
            .WithAttribute($@"[System.CodeDom.Compiler.GeneratedCode(""{GeneratorName}"", ""{GeneratorVersion}"")]");
    }
}
