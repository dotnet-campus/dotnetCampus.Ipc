using dotnetCampus.Ipc.CompilerServices.Attributes;

using Microsoft.CodeAnalysis;

namespace dotnetCampus.Ipc.SourceGenerators.Utils;

/// <summary>
/// 包含 <see cref="IpcMemberAttribute"/> 相关语义分析的辅助扩展方法。
/// </summary>
public static class SemanticIpcAttributeHelper
{
    /// <summary>
    /// 检查此方法是否标记了 <see cref="IpcMethodAttribute.WaitsVoid"/>。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool CheckIpcWaitingVoid(this IMethodSymbol symbol)
    {
        return symbol.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.WaitsVoid));
    }

    /// <summary>
    /// 检查此方法是以 <see cref="IpcMethodAttribute.DefaultReturn"/> 标记的值。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static object? GetIpcDefaultReturn(this IMethodSymbol symbol)
    {
        return symbol.GetAttributeValue<IpcMethodAttribute, object>(nameof(IpcMemberAttribute.DefaultReturn));
    }

    /// <summary>
    /// 检查此方法是否标记了 <see cref="IpcMethodAttribute.IgnoreIpcException"/>。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool CheckIpcIgnoreIpcException(this IMethodSymbol symbol)
    {
        return symbol.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMemberAttribute.IgnoreIpcException));
    }

    /// <summary>
    /// 检查此方法以 <see cref="IpcMethodAttribute.Timeout"/> 标记的超时时间。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static int GetIpcTimeout(this IMethodSymbol symbol)
    {
        return symbol.GetAttributeValue<IpcMethodAttribute, int>(nameof(IpcMemberAttribute.Timeout));
    }

    /// <summary>
    /// 检查此属性是否标记了 <see cref="IpcPropertyAttribute.IsReadonly"/>。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool CheckIpcIsReadonly(this IPropertySymbol symbol)
    {
        return symbol.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IsReadonly));
    }

    /// <summary>
    /// 检查此属性是以 <see cref="IpcMethodAttribute.DefaultReturn"/> 标记的值。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static object? GetIpcDefaultReturn(this IPropertySymbol symbol)
    {
        return symbol.GetAttributeValue<IpcPropertyAttribute, object>(nameof(IpcMemberAttribute.DefaultReturn));
    }

    /// <summary>
    /// 检查此属性是否标记了 <see cref="IpcMethodAttribute.IgnoreIpcException"/>。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static bool CheckIpcIgnoreIpcException(this IPropertySymbol symbol)
    {
        return symbol.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcMemberAttribute.IgnoreIpcException));
    }

    /// <summary>
    /// 检查此属性以 <see cref="IpcMethodAttribute.Timeout"/> 标记的超时时间。
    /// </summary>
    /// <param name="symbol"></param>
    /// <returns></returns>
    public static int GetIpcTimeout(this IPropertySymbol symbol)
    {
        return symbol.GetAttributeValue<IpcPropertyAttribute, int>(nameof(IpcMemberAttribute.Timeout));
    }
}
