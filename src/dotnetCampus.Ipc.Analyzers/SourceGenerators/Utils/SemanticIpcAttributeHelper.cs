using System.Globalization;

using dotnetCampus.Ipc.CompilerServices.Attributes;

using Microsoft.CodeAnalysis;

namespace dotnetCampus.Ipc.SourceGenerators.Utils;

/// <summary>
/// 包含 <see cref="IpcMemberAttribute"/> 相关语义分析的辅助扩展方法。
/// </summary>
public static class SemanticIpcAttributeHelper
{
    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="method"></param>
    /// <returns></returns>
    public static string GetIpcAttributesAsAnInvokingArg(this IMethodSymbol method)
    {
        var waitsVoid = method.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.WaitsVoid));
        var ignoreIpcException = method.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.IgnoreIpcException));
        var defaultReturn = method.GetAttributeValue<IpcMethodAttribute, object?>(nameof(IpcMethodAttribute.DefaultReturn));
        var timeout = method.GetAttributeValue<IpcMethodAttribute, int>(nameof(IpcMethodAttribute.Timeout));
        return $"new({Format(defaultReturn)}, {Format(timeout)}, ignoreIpcException: {Format(ignoreIpcException)}, waitsVoid: {Format(waitsVoid)})";
    }

    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="property"></param>
    /// <returns></returns>
    public static string GetIpcAttributesAsAnInvokingArg(this IPropertySymbol property)
    {
        var isReadonly = property.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IsReadonly));
        var ignoreIpcException = property.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IgnoreIpcException));
        var defaultReturn = property.GetAttributeValue<IpcPropertyAttribute, object?>(nameof(IpcPropertyAttribute.DefaultReturn));
        var timeout = property.GetAttributeValue<IpcPropertyAttribute, int>(nameof(IpcPropertyAttribute.Timeout));
        return $"new({Format(defaultReturn)}, {Format(timeout)}, ignoreIpcException: {Format(ignoreIpcException)}, isReadonly: {Format(isReadonly)})";
    }

    private static string Format(object? value)
    {
        return value?.ToString() ?? "null";
    }

    private static string Format(bool value)
    {
        return value ? "true" : "false";
    }

    private static string Format(int value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// 检查此方法是否标记了 <see cref="IpcMethodAttribute.WaitsVoid"/>。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool CheckIpcWaitingVoid(this IMethodSymbol symbol)
    {
        return symbol.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.WaitsVoid));
    }
}
