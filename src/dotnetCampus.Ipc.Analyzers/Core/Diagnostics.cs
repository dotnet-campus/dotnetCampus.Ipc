using dotnetCampus.Ipc.Analyzers.Properties;

using Microsoft.CodeAnalysis;

using static dotnetCampus.Ipc.Analyzers.Properties.Resources;

namespace dotnetCampus.Ipc.Analyzers.Core;

/// <summary>
/// 包含 IPC 框架分析器中的所有诊断。
/// </summary>
internal static class Diagnostics
{
    public static readonly DiagnosticDescriptor DIPC001_UnknownError = new(
        nameof(DIPC001),
        Localize(nameof(DIPC001)),
        Localize(nameof(DIPC001_Message)),
        Categories.Syntax,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DIPC002_ContractTypeNotSpecified = new(
        nameof(DIPC002),
        Localize(nameof(DIPC002)),
        Localize(nameof(DIPC002_Message)),
        Categories.Syntax,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DIPC003_OnlyMethodOrPropertyIsSupported = new(
        nameof(DIPC003),
        Localize(nameof(DIPC003)),
        Localize(nameof(DIPC003_Message)),
        Categories.Syntax,
        DiagnosticSeverity.Error,
        true);

    public static readonly DiagnosticDescriptor DIPC004_OnlyGetOrGetSetPropertyIsSupported = new(
        nameof(DIPC004),
        Localize(nameof(DIPC004)),
        Localize(nameof(DIPC004_Message)),
        Categories.Syntax,
        DiagnosticSeverity.Error,
        true);

    private static class Categories
    {
        public const string Syntax = "dotnetCampus.Syntax";
    }

    private static LocalizableString Localize(string key) => new LocalizableResourceString(key, ResourceManager, typeof(Resources));
}
