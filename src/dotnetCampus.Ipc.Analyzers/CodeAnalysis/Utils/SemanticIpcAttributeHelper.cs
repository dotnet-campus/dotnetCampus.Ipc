namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

/// <summary>
/// 包含 <see cref="IpcMemberAttribute"/> 相关语义分析的辅助扩展方法。
/// </summary>
public static class SemanticIpcAttributeHelper
{
    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="method"></param>
    /// <param name="returnTypeSymbol">返回值类型，依此来决定如何将 Attribute 里的对象转为字符串。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    public static string GetIpcAttributesAsAnInvokingArg(this IMethodSymbol method, ITypeSymbol? returnTypeSymbol, INamedTypeSymbol containingType)
    {
        var type = containingType;
        var waitsVoid = method.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.WaitsVoid));
        var ignoresIpcException = method.GetAttributeValueOrDefault<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.IgnoresIpcException))
            ?? type.GetAttributeValueOrDefault<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException))
            ?? false;
        var defaultReturn = method.GetAttributeValue<IpcMethodAttribute, object?>(nameof(IpcMethodAttribute.DefaultReturn));
        var timeout = method.GetAttributeValueOrDefault<IpcMethodAttribute, int>(nameof(IpcMethodAttribute.Timeout))
            ?? type.GetAttributeValueOrDefault<IpcPublicAttribute, int>(nameof(IpcPublicAttribute.Timeout))
            ?? 0;
        var quoteObject = returnTypeSymbol?.ToString() == "string";
        return $@"new()
{{
    DefaultReturn = {Format(defaultReturn, quoteObject)},
    Timeout = {Format(timeout)},
    IgnoresIpcException = {Format(ignoresIpcException)},
    WaitsVoid = {Format(waitsVoid)}
}}";
    }

    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="property">要查找标记的属性。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    public static string GetIpcAttributesAsAnInvokingArg(this IPropertySymbol property, INamedTypeSymbol containingType)
    {
        var type = containingType;
        var isReadonly = property.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IsReadonly));
        var ignoresIpcException = property.GetAttributeValueOrDefault<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IgnoresIpcException))
            ?? type.GetAttributeValueOrDefault<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException))
            ?? false;
        var defaultReturn = property.GetAttributeValue<IpcPropertyAttribute, object?>(nameof(IpcPropertyAttribute.DefaultReturn));
        var timeout = property.GetAttributeValueOrDefault<IpcPropertyAttribute, int>(nameof(IpcPropertyAttribute.Timeout))
            ?? type.GetAttributeValueOrDefault<IpcPublicAttribute, int>(nameof(IpcPublicAttribute.Timeout))
            ?? 0;
        var quoteObject = property.Type.ToString() == "string";
        return $@"new()
{{
    DefaultReturn = {Format(defaultReturn, quoteObject)},
    Timeout = {Format(timeout)},
    IgnoresIpcException = {Format(ignoresIpcException)},
    IsReadonly = {Format(isReadonly)}
}}";
    }

    /// <summary>
    /// 将 Attribute 里的对象转为字符串。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="quoteObject">在格式化参数时，如果非 null，是否应该用引号将其包围。</param>
    /// <returns></returns>
    private static string Format(object? value, bool quoteObject)
    {
        //if (value?.ToString() == "default1")
        //{
        //    Debugger.Launch();
        //}
        if (value is null)
        {
            return "null";
        }
        else if (quoteObject)
        {
            return $@"""{value}""";
        }
        else
        {
            return value.ToString();
        }
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
