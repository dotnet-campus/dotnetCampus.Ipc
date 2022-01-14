using dotnetCampus.Ipc.Properties;

using static dotnetCampus.Ipc.Properties.Resources;
using static Microsoft.CodeAnalysis.WellKnownDiagnosticTags;

namespace dotnetCampus.Ipc.CodeAnalysis.Core;

/// <summary>
/// 包含 IPC 框架分析器中的所有诊断。
/// </summary>
internal static class Diagnostics
{
    public static DiagnosticDescriptor DIPC001_UnknownError { get; } = new(
        nameof(DIPC001),
        Localize(nameof(DIPC001)),
        Localize(nameof(DIPC001_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { AnalyzerException, NotConfigurable });

    public static DiagnosticDescriptor DIPC002_ContractTypeIsNotSpecified { get; } = new(
        nameof(DIPC002),
        Localize(nameof(DIPC002)),
        Localize(nameof(DIPC002_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC003_ContractTypeMustBeAnInterface { get; } = new(
        nameof(DIPC003),
        Localize(nameof(DIPC003)),
        Localize(nameof(DIPC003_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC004_ContractTypeDismatchWithInterface { get; } = new(
        nameof(DIPC004),
        Localize(nameof(DIPC004)),
        Localize(nameof(DIPC004_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC010_NoIpcShapeFound { get; } = new(
        nameof(DIPC010),
        Localize(nameof(DIPC010)),
        Localize(nameof(DIPC010_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC011_IpcProxyDismatchWithContractType { get; } = new(
        nameof(DIPC011),
        Localize(nameof(DIPC011)),
        Localize(nameof(DIPC011_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC012_IpcJointDismatchWithContractType { get; } = new(
        nameof(DIPC012),
        Localize(nameof(DIPC012)),
        Localize(nameof(DIPC012_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC020_OnlyPropertiesAndMethodsAreSupportedForIpcObject { get; } = new(
        nameof(DIPC020),
        Localize(nameof(DIPC020)),
        Localize(nameof(DIPC020_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC021_EventIsNotSupportedForIpcObject { get; } = new(
        nameof(DIPC021),
        Localize(nameof(DIPC021)),
        Localize(nameof(DIPC021_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC022_SetOnlyPropertyIsNotSupportedForIpcObject { get; } = new(
        nameof(DIPC022),
        Localize(nameof(DIPC022)),
        Localize(nameof(DIPC022_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor DIPC101_IpcPublic_IgnoresIpcExceptionIsRecommended { get; } = new(
        nameof(DIPC101),
        Localize(nameof(DIPC101)),
        Localize(nameof(DIPC101_Message)),
        Categories.Readable,
        DiagnosticSeverity.Warning,
        true);

    public static DiagnosticDescriptor DIPC102_IpcPublic_TimeoutCannotBeNegative { get; } = new(
        nameof(DIPC102),
        Localize(nameof(DIPC102)),
        Localize(nameof(DIPC102_Message)),
        Categories.Useless,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor DIPC103_IpcPublic_TimeoutZeroIsUnnecessary { get; } = new(
        nameof(DIPC103),
        Localize(nameof(DIPC103)),
        Localize(nameof(DIPC103_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor DIPC120_IpcMember_DefaultReturnDependsOnIgnoresIpcException { get; } = new(
        nameof(DIPC120),
        Localize(nameof(DIPC120)),
        Localize(nameof(DIPC120_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor DIPC121_IpcMember_EmptyIpcMemberAttributeIsUnnecessary { get; } = new(
        nameof(DIPC121),
        Localize(nameof(DIPC121)),
        Localize(nameof(DIPC121_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor DIPC122_IpcMember_WaitsVoidIsRecommended { get; } = new(
        nameof(DIPC122),
        Localize(nameof(DIPC122)),
        Localize(nameof(DIPC122_Message)),
        Categories.Readable,
        DiagnosticSeverity.Warning,
        true);

    public static DiagnosticDescriptor DIPC123_IpcMember_IsReadonlyFalseIsUnnecessary { get; } = new(
        nameof(DIPC123),
        Localize(nameof(DIPC123)),
        Localize(nameof(DIPC123_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor DIPC124_IpcMember_DefaultReturnDismatchWithPropertyType { get; } = new(
        nameof(DIPC124),
        Localize(nameof(DIPC124)),
        Localize(nameof(DIPC124_Message)),
        Categories.RuntimeException,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor DIPC125_IpcMember_DefaultReturnDismatchWithMethodReturnType { get; } = new(
        nameof(DIPC125),
        Localize(nameof(DIPC125)),
        Localize(nameof(DIPC125_Message)),
        Categories.RuntimeException,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor DIPC126_IpcMember_DefaultReturnIsUselessForAVoidMethod { get; } = new(
        nameof(DIPC126),
        Localize(nameof(DIPC126)),
        Localize(nameof(DIPC126_Message)),
        Categories.Useless,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor DIPC127_IpcMember_DefaultReturnsStringForAnObjectType { get; } = new(
        nameof(DIPC127),
        Localize(nameof(DIPC127)),
        Localize(nameof(DIPC127_Message)),
        Categories.Readable,
        DiagnosticSeverity.Hidden,
        true);

    public static DiagnosticDescriptor DIPC128_IpcMember_DefaultReturnsStringForANonStringType { get; } = new(
        nameof(DIPC128),
        Localize(nameof(DIPC128)),
        Localize(nameof(DIPC128_Message)),
        Categories.Readable,
        DiagnosticSeverity.Hidden,
        true);

    public static DiagnosticDescriptor DIPC129_IpcMember_DefaultReturnsStringCannotBeCompiledAsACodeSnippet { get; } = new(
        nameof(DIPC129),
        Localize(nameof(DIPC129)),
        Localize(nameof(DIPC129_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true);

    private static class Categories
    {
        /// <summary>
        /// 因编译要求而必须满足的条件没有满足，则报告此诊断。
        /// </summary>
        public const string Compiler = "dotnetCampus.Compiler";

        /// <summary>
        /// 因 IPC 库内的机制限制，必须满足此要求 IPC 才可正常工作，则报告此诊断。
        /// </summary>
        public const string Mechanism = "dotnetCampus.Mechanism";

        /// <summary>
        /// 为了代码可读性，使之更易于理解、方便调试，则报告此诊断。
        /// </summary>
        public const string Readable = "dotnetCampus.Readable";

        /// <summary>
        /// 能写得出来正常编译，但会引发运行时异常，则报告此诊断。
        /// </summary>
        public const string RuntimeException = "dotnetCampus.RuntimeException";

        /// <summary>
        /// 编写了无法生效的代码，则报告此诊断。
        /// </summary>
        public const string Useless = "dotnetCampus.Useless";
    }

    private static LocalizableString Localize(string key) => new LocalizableResourceString(key, ResourceManager, typeof(Resources));
}
