using dotnetCampus.Ipc.Properties;

using static dotnetCampus.Ipc.Properties.Resources;

namespace dotnetCampus.Ipc.CodeAnalysis.Core;

/// <summary>
/// 包含 IPC 框架分析器中的所有诊断。
/// </summary>
internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor DIPC001_UnknownError = new(
        nameof(DIPC001),
        Localize(nameof(DIPC001)),
        Localize(nameof(DIPC001_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DIPC002_ContractTypeNotSpecified = new(
        nameof(DIPC002),
        Localize(nameof(DIPC002)),
        Localize(nameof(DIPC002_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DIPC003_OnlyMethodOrPropertyIsSupported = new(
        nameof(DIPC003),
        Localize(nameof(DIPC003)),
        Localize(nameof(DIPC003_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DIPC004_OnlyGetOrGetSetPropertyIsSupported = new(
        nameof(DIPC004),
        Localize(nameof(DIPC004)),
        Localize(nameof(DIPC004_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DIPC101_IgnoresIpcExceptionIsRecommended = new(
        nameof(DIPC101),
        Localize(nameof(DIPC101)),
        Localize(nameof(DIPC101_Message)),
        Categories.Readable,
        DiagnosticSeverity.Warning,
        true);

    private static class Categories
    {
        /// <summary>
        /// 因为编译要求而必须满足的条件没有满足，则报告此诊断。
        /// </summary>
        public const string Compiler = "dotnetCampus.Compiler";

        /// <summary>
        /// 为了代码可读性，使之更易于理解、方便调试，则报告此诊断。
        /// </summary>
        public const string Readable = "dotnetCampus.Readable";
    }

    private static LocalizableString Localize(string key) => new LocalizableResourceString(key, ResourceManager, typeof(Resources));
}
