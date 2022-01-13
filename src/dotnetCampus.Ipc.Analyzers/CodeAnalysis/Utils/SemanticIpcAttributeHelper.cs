using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

/// <summary>
/// 包含 <see cref="IpcMemberAttribute"/> 相关语义分析的辅助扩展方法。
/// </summary>
internal static class SemanticIpcAttributeHelper
{
    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="method"></param>
    /// <param name="returnTypeSymbol">返回值类型，依此来决定如何将 Attribute 里的对象转为字符串。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    internal static IpcProxyMemberNamedValues GetIpcNamedValues(this IMethodSymbol method, ITypeSymbol? returnTypeSymbol, INamedTypeSymbol containingType)
    {
        var type = containingType;
        var waitsVoid = method.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.WaitsVoid));
        var ignoresIpcException = method.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.IgnoresIpcException))
            ?? type.GetAttributeValue<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException))
            ?? false;
        var defaultReturn = method.GetAttributeValue<IpcMethodAttribute, object?>(nameof(IpcMethodAttribute.DefaultReturn));
        var timeout = method.GetAttributeValue<IpcMethodAttribute, int>(nameof(IpcMethodAttribute.Timeout))
            ?? type.GetAttributeValue<IpcPublicAttribute, int>(nameof(IpcPublicAttribute.Timeout))
            ?? 0;
        return new IpcProxyMemberNamedValues
        {
            DefaultReturn = defaultReturn,
            Timeout = timeout,
            IgnoresIpcException = ignoresIpcException,
            WaitsVoid = waitsVoid
        };
    }

    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="method"></param>
    /// <param name="returnTypeSymbol">返回值类型，依此来决定如何将 Attribute 里的对象转为字符串。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    internal static string FormatIpcNamedValuesAsAnInvokingArg(this IMethodSymbol method, ITypeSymbol? returnTypeSymbol, INamedTypeSymbol containingType)
    {
        var namedValues = GetIpcNamedValues(method, returnTypeSymbol, containingType);
        var quoteObject = returnTypeSymbol?.ToString() == "string";
        return $@"new()
{{
    DefaultReturn = {Format(namedValues.DefaultReturn?.Value, quoteObject)},
    Timeout = {Format(namedValues.Timeout)},
    IgnoresIpcException = {Format(namedValues.IgnoresIpcException)},
    WaitsVoid = {Format(namedValues.WaitsVoid)}
}}";
    }

    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="property">要查找标记的属性。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    public static IpcProxyMemberNamedValues GetIpcNamedValues(this IPropertySymbol property, INamedTypeSymbol containingType)
    {
        var type = containingType;
        var isReadonly = property.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IsReadonly));
        var ignoresIpcException = property.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IgnoresIpcException))
            ?? type.GetAttributeValue<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException))
            ?? false;
        var defaultReturn = property.GetAttributeValue<IpcPropertyAttribute, object?>(nameof(IpcPropertyAttribute.DefaultReturn));
        var timeout = property.GetAttributeValue<IpcPropertyAttribute, int>(nameof(IpcPropertyAttribute.Timeout))
            ?? type.GetAttributeValue<IpcPublicAttribute, int>(nameof(IpcPublicAttribute.Timeout))
            ?? 0;
        return new IpcProxyMemberNamedValues
        {
            DefaultReturn = defaultReturn,
            Timeout = timeout,
            IgnoresIpcException = ignoresIpcException,
            IsReadonly = isReadonly,
        };
    }

    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="property">要查找标记的属性。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    public static string FormatIpcNamedValuesAsAnInvokingArg(this IPropertySymbol property, INamedTypeSymbol containingType)
    {
        var namedValues = GetIpcNamedValues(property, containingType);
        var quoteObject = property.Type.ToString() == "string";
        return $@"new()
{{
    DefaultReturn = {Format(namedValues.DefaultReturn?.Value, quoteObject)},
    Timeout = {Format(namedValues.Timeout)},
    IgnoresIpcException = {Format(namedValues.IgnoresIpcException)},
    IsReadonly = {Format(namedValues.IsReadonly)}
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
