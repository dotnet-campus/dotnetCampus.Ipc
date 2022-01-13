namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

/// <summary>
/// 包含语义分析的辅助扩展方法。
/// </summary>
internal static class SemanticAttributeHelper
{
    /// <summary>
    /// 检查此成员的 <typeparamref name="TAttribute"/> 类型特性的 <paramref name="namedArgumentName"/> 名字的参数的值。
    /// </summary>
    /// <typeparam name="TAttribute">特性类型。</typeparam>
    /// <typeparam name="T">参数类型。</typeparam>
    /// <param name="symbol">要查找特性的语义符号。</param>
    /// <param name="namedArgumentName">参数名称。</param>
    /// <returns>参数的值。</returns>
    [return: MaybeNull]
    public static T? GetAttributeValue<TAttribute, T>(this ISymbol symbol, string namedArgumentName)
    {
        var value = GetAttributeValue(symbol, typeof(TAttribute).FullName, namedArgumentName);
        if (value == null)
        {
            return default;
        }
        if (typeof(T) == typeof(object))
        {
            return (T?) value;
        }
        if (typeof(T) == typeof(bool))
        {
            return (T) (object) Convert.ToBoolean(value);
        }
        else if (typeof(T) == typeof(int))
        {
            return (T) (object) Convert.ToInt32(value);
        }
        throw new NotSupportedException("尚不支持读取其他类型的特性。");
    }

#nullable disable
    public static T? GetAttributeValueOrDefault<TAttribute, T>(this ISymbol symbol, string namedArgumentName)
        where T : struct
    {
        var value = GetAttributeValue(symbol, typeof(TAttribute).FullName, namedArgumentName);
        if (value == null)
        {
            return default;
        }
        if (typeof(T) == typeof(object))
        {
            return (T?) value;
        }
        if (typeof(T) == typeof(bool))
        {
            return (T) (object) Convert.ToBoolean(value);
        }
        else if (typeof(T) == typeof(int))
        {
            return (T) (object) Convert.ToInt32(value);
        }
        throw new NotSupportedException("尚不支持读取其他类型的特性。");
    }
#nullable restore

    /// <summary>
    /// 检查此成员的 <paramref name="attributeTypeName"/> 类型特性的 <paramref name="namedArgumentName"/> 名字的参数的值。
    /// </summary>
    /// <param name="symbol">要查找特性的语义符号。</param>
    /// <param name="attributeTypeName">特性类型的名称。</param>
    /// <param name="namedArgumentName">参数名称。</param>
    /// <returns>参数的值。</returns>
    private static object? GetAttributeValue(ISymbol symbol, string attributeTypeName, string namedArgumentName)
    {
        if (symbol.GetAttributes().FirstOrDefault(x => string.Equals(
             x.AttributeClass?.ToString(),
             attributeTypeName,
             StringComparison.Ordinal)) is { } ipcMethodAttribute)
        {
            var value = ipcMethodAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == namedArgumentName).Value;
            return value.Value;
        }
        return null;
    }
}
