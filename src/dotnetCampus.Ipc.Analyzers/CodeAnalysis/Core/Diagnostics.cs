using dotnetCampus.Ipc.Properties;

using static dotnetCampus.Ipc.Properties.Localizations;
using static Microsoft.CodeAnalysis.WellKnownDiagnosticTags;

namespace dotnetCampus.Ipc.CodeAnalysis.Core;

/// <summary>
/// 包含 IPC 框架分析器中的所有诊断。
/// </summary>
internal static class Diagnostics
{
    /// <summary>
    /// 生成代码时出现未知错误。
    /// </summary>
    public static DiagnosticDescriptor IPC000_UnknownError { get; } = new(
        nameof(IPC000),
        Localize(nameof(IPC000)),
        Localize(nameof(IPC000_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { AnalyzerException, NotConfigurable });

    /// <summary>
    /// 生成代码时出现已知错误。本生成器不会报告此错误，因为后续编译器会准确报告之。
    /// </summary>
    public static DiagnosticDescriptor IPC001_KnownCompilerError { get; } = new(
        nameof(IPC001),
        Localize(nameof(IPC001)),
        Localize(nameof(IPC001_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { NotConfigurable });

    /// <summary>
    /// 生成代码时出现已知错误。本生成器不会报告此错误，因为分析器会准确报告之。
    /// </summary>
    public static DiagnosticDescriptor IPC002_KnownDiagnosticError { get; } = new(
        nameof(IPC002),
        Localize(nameof(IPC002)),
        Localize(nameof(IPC002_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC101_IpcType_TimeoutCannotBeNegative { get; } = new(
        nameof(IPC101),
        Localize(nameof(IPC101)),
        Localize(nameof(IPC101_Message)),
        Categories.Useless,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC102_IpcType_TimeoutZeroIsUnnecessary { get; } = new(
        nameof(IPC102),
        Localize(nameof(IPC102)),
        Localize(nameof(IPC102_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC131_IpcMembers_IgnoresIpcExceptionIsRecommended { get; } = new(
        nameof(IPC131),
        Localize(nameof(IPC131)),
        Localize(nameof(IPC131_Message)),
        Categories.Readable,
        DiagnosticSeverity.Info,
        true);

    public static DiagnosticDescriptor IPC160_IpcShape_ContractTypeMustBeAnInterface { get; } = new(
        nameof(IPC160),
        Localize(nameof(IPC160)),
        Localize(nameof(IPC160_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC161_IpcShape_ContractTypeDismatchWithInterface { get; } = new(
        nameof(IPC161),
        Localize(nameof(IPC161)),
        Localize(nameof(IPC161_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC162_IpcShape_AllMembersShouldBeMarkedAsIpcMembers { get; } = new(
        nameof(IPC162),
        Localize(nameof(IPC162)),
        Localize(nameof(IPC162_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Warning,
        true);

    public static DiagnosticDescriptor IPC200_IpcMembers_OnlyPropertiesMethodsAndEventsAreSupported { get; } = new(
        nameof(IPC200),
        Localize(nameof(IPC200)),
        Localize(nameof(IPC200_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC201_IpcMember_EmptyIpcMemberAttributeIsUnnecessary { get; } = new(
        nameof(IPC201),
        Localize(nameof(IPC201)),
        Localize(nameof(IPC201_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC202_IpcMember_AllMembersShouldBeMarkedAsIpcMembers { get; } = new(
        nameof(IPC202),
        Localize(nameof(IPC202)),
        Localize(nameof(IPC202_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Warning,
        true);

    public static DiagnosticDescriptor IPC240_IpcProperty_IpcPropertyIsNotRecommended { get; } = new(
        nameof(IPC240),
        Localize(nameof(IPC240)),
        Localize(nameof(IPC240_Message)),
        Categories.AvoidBugs,
        DiagnosticSeverity.Info,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC241_IpcProperty_SetOnlyPropertyIsNotSupported { get; } = new(
        nameof(IPC241),
        Localize(nameof(IPC241)),
        Localize(nameof(IPC241_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC242_IpcProperty_DefaultReturnDependsOnIgnoresIpcException { get; } = new(
        nameof(IPC242),
        Localize(nameof(IPC242)),
        Localize(nameof(IPC242_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC243_IpcProperty_IsReadonlyFalseIsUnnecessary { get; } = new(
        nameof(IPC243),
        Localize(nameof(IPC243)),
        Localize(nameof(IPC243_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC244_IpcProperty_DefaultReturnDismatchWithPropertyType { get; } = new(
        nameof(IPC244),
        Localize(nameof(IPC244)),
        Localize(nameof(IPC244_Message)),
        Categories.RuntimeException,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC245_IpcProperty_DefaultReturnsStringForAnObjectType { get; } = new(
        nameof(IPC245),
        Localize(nameof(IPC245)),
        Localize(nameof(IPC245_Message)),
        Categories.Readable,
        DiagnosticSeverity.Warning,
        true);

    public static DiagnosticDescriptor IPC246_IpcProperty_DefaultReturnsStringForANonStringType { get; } = new(
        nameof(IPC246),
        Localize(nameof(IPC246)),
        Localize(nameof(IPC246_Message)),
        Categories.Readable,
        DiagnosticSeverity.Hidden,
        true);

    public static DiagnosticDescriptor IPC247_IpcProperty_DefaultReturnsStringCannotBeCompiledAsACodeSnippet { get; } = new(
        nameof(IPC247),
        Localize(nameof(IPC247)),
        Localize(nameof(IPC247_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC248_IpcProperty_PropertyTypeIsNotSupportedForIpc { get; } = new(
        nameof(IPC248),
        Localize(nameof(IPC248)),
        Localize(nameof(IPC248_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC260_IpcMethod_SyncMethodIsNotRecommended { get; } = new(
        nameof(IPC260),
        Localize(nameof(IPC260)),
        Localize(nameof(IPC260_Message)),
        Categories.AvoidBugs,
        DiagnosticSeverity.Info,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC261_IpcMethod_DefaultReturnDependsOnIgnoresIpcException { get; } = new(
        nameof(IPC261),
        Localize(nameof(IPC261)),
        Localize(nameof(IPC261_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC262_IpcMethod_WaitsVoidIsRecommended { get; } = new(
        nameof(IPC262),
        Localize(nameof(IPC262)),
        Localize(nameof(IPC262_Message)),
        Categories.Readable,
        DiagnosticSeverity.Warning,
        true);

    public static DiagnosticDescriptor IPC263_IpcMethod_WaitsVoidIsUseless { get; } = new(
        nameof(IPC263),
        Localize(nameof(IPC263)),
        Localize(nameof(IPC263_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC264_IpcMethod_DefaultReturnDismatchWithMethodReturnType { get; } = new(
        nameof(IPC264),
        Localize(nameof(IPC264)),
        Localize(nameof(IPC264_Message)),
        Categories.RuntimeException,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC265_IpcMethod_DefaultReturnIsUselessForAVoidMethod { get; } = new(
        nameof(IPC265),
        Localize(nameof(IPC265)),
        Localize(nameof(IPC265_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC266_IpcMethod_DefaultReturnIsUselessForATaskMethod { get; } = new(
        nameof(IPC266),
        Localize(nameof(IPC266)),
        Localize(nameof(IPC266_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC267_IpcMethod_DefaultReturnsStringForAnObjectType { get; } = new(
        nameof(IPC267),
        Localize(nameof(IPC267)),
        Localize(nameof(IPC267_Message)),
        Categories.Readable,
        DiagnosticSeverity.Warning,
        true);

    public static DiagnosticDescriptor IPC268_IpcMethod_DefaultReturnsStringForANonStringType { get; } = new(
        nameof(IPC268),
        Localize(nameof(IPC268)),
        Localize(nameof(IPC268_Message)),
        Categories.Readable,
        DiagnosticSeverity.Hidden,
        true);

    public static DiagnosticDescriptor IPC269_IpcMethod_DefaultReturnsStringCannotBeCompiledAsACodeSnippet { get; } = new(
        nameof(IPC269),
        Localize(nameof(IPC269)),
        Localize(nameof(IPC269_Message)),
        Categories.Compiler,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC270_IpcMethod_GenericMethodIsNotSupportedForIpc { get; } = new(
        nameof(IPC270),
        Localize(nameof(IPC270)),
        Localize(nameof(IPC270_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC271_IpcMethod_MethodParameterTypeIsNotSupportedForIpc { get; } = new(
        nameof(IPC271),
        Localize(nameof(IPC271)),
        Localize(nameof(IPC271_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC272_IpcMethod_MethodReturnTypeIsNotSupportedForIpc { get; } = new(
        nameof(IPC272),
        Localize(nameof(IPC272)),
        Localize(nameof(IPC272_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true);

    public static DiagnosticDescriptor IPC301_CreateIpcProxy_AddIpcProxyConfigs { get; } = new(
        nameof(IPC301),
        Localize(nameof(IPC301)),
        Localize(nameof(IPC301_Message)),
        Categories.CodeFixOnly,
        DiagnosticSeverity.Hidden,
        true);

    public static DiagnosticDescriptor IPC302_CreateIpcProxy_AddIpcShape { get; } = new(
        nameof(IPC302),
        Localize(nameof(IPC302)),
        Localize(nameof(IPC302_Message)),
        Categories.CodeFixOnly,
        DiagnosticSeverity.Hidden,
        true);

    public static DiagnosticDescriptor IPC303_CreateIpcProxy_AddIpcProxyConfigs { get; } = new(
        nameof(IPC303),
        Localize(nameof(IPC303)),
        Localize(nameof(IPC303_Message)),
        Categories.Readable,
        DiagnosticSeverity.Info,
        true);

    public static DiagnosticDescriptor IPC304_CreateIpcProxy_RemoveIpcProxyConfigs { get; } = new(
        nameof(IPC304),
        Localize(nameof(IPC304)),
        Localize(nameof(IPC304_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC305_CreateIpcProxy_IgnoresIpcExceptionIsRecommended { get; } = new(
        nameof(IPC305),
        Localize(nameof(IPC305)),
        Localize(nameof(IPC305_Message)),
        Categories.Readable,
        DiagnosticSeverity.Info,
        true);

    public static DiagnosticDescriptor IPC306_CreateIpcProxy_IgnoresIpcExceptionIsUseless { get; } = new(
        nameof(IPC306),
        Localize(nameof(IPC306)),
        Localize(nameof(IPC306_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC307_CreateIpcProxy_TimeoutIsRecommended { get; } = new(
        nameof(IPC307),
        Localize(nameof(IPC307)),
        Localize(nameof(IPC307_Message)),
        Categories.CodeFixOnly,
        DiagnosticSeverity.Hidden,
        true);

    public static DiagnosticDescriptor IPC308_CreateIpcProxy_TimeoutIsUseless { get; } = new(
        nameof(IPC308),
        Localize(nameof(IPC308)),
        Localize(nameof(IPC308_Message)),
        Categories.Useless,
        DiagnosticSeverity.Hidden,
        true,
        customTags: new[] { Unnecessary });

    public static DiagnosticDescriptor IPC309_CreateIpcProxy_IpcShapeIsNotValid { get; } = new(
        nameof(IPC309),
        Localize(nameof(IPC309)),
        Localize(nameof(IPC309_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPC310_CreateIpcProxy_IpcShapeDismatchWithContractType { get; } = new(
        nameof(IPC310),
        Localize(nameof(IPC310)),
        Localize(nameof(IPC310_Message)),
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    public static DiagnosticDescriptor IPCTMP1_IpcMembers_EventIsNotSupported { get; } = new(
        "IPCTMP1",
        "IPC 成员暂不支持事件",
        "IPC 成员暂不支持事件",
        Categories.Mechanism,
        DiagnosticSeverity.Error,
        true,
        customTags: new[] { NotConfigurable });

    private static class Categories
    {
        /// <summary>
        /// 可能产生 bug，则报告此诊断。
        /// </summary>
        public const string AvoidBugs = "dotnetCampus.AvoidBugs";

        /// <summary>
        /// 为了提供代码生成能力，则报告此诊断。
        /// </summary>
        public const string CodeFixOnly = "dotnetCampus.CodeFixOnly";

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

    private static LocalizableString Localize(string key) => new LocalizableResourceString(key, ResourceManager, typeof(Localizations));
}
