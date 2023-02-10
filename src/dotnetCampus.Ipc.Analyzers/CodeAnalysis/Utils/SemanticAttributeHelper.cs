using System.Diagnostics;

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
    public static Assignable<T>? GetAttributeValue<TAttribute, T>(this ISymbol symbol, string namedArgumentName)
    {
        var assignable = GetAttributeValue(symbol, typeof(TAttribute).FullName, namedArgumentName);

        // 未赋值。
        if (assignable is null)
        {
            return null;
        }

        // 引用类型已赋值。
        if (typeof(T) == typeof(object))
        {
            return new((T?) assignable.Value);
        }

        // null 已赋值。
        var value = assignable.Value.Value;
        if (value is null)
        {
            return new(default);
        }

        // 值类型已复制。
        if (typeof(T) == typeof(bool))
        {
            return new((T) (object) Convert.ToBoolean(value));
        }
        else if (typeof(T) == typeof(int))
        {
            return new((T) (object) Convert.ToInt32(value));
        }

        // 其他已赋值。
        throw new NotSupportedException($"尚不支持读取 {typeof(T).Name} 类型的特性。");
    }

    /// <summary>
    /// 检查此成员是否已定义了 <typeparamref name="TAttribute"/> 类型的特性。
    /// </summary>
    /// <typeparam name="TAttribute">要检查的类型。</typeparam>
    /// <param name="symbol">要查找特性的语义符号。</param>
    /// <returns>如果定义了 <typeparamref name="TAttribute"/>，则返回 true；否则返回 false。</returns>
    internal static bool GetIsDefined<TAttribute>(this ISymbol symbol)
    {
        return symbol.GetAttributes().FirstOrDefault(x => string.Equals(
            x.AttributeClass?.ToString(),
            typeof(TAttribute).FullName,
            StringComparison.Ordinal)) is { } ipcMethodAttribute;
    }

    /// <summary>
    /// 检查此成员的 <paramref name="attributeTypeName"/> 类型特性的 <paramref name="namedArgumentName"/> 名字的参数的值。
    /// </summary>
    /// <param name="symbol">要查找特性的语义符号。</param>
    /// <param name="attributeTypeName">特性类型的名称。</param>
    /// <param name="namedArgumentName">参数名称。</param>
    /// <returns>参数的值。</returns>
    private static Assignable<object?>? GetAttributeValue(ISymbol symbol, string attributeTypeName, string namedArgumentName)
    {
        if (symbol.GetAttributes().FirstOrDefault(x => string.Equals(
             x.AttributeClass?.ToString(),
             attributeTypeName,
             StringComparison.Ordinal)) is { } ipcMethodAttribute)
        {
            var argumentPair = ipcMethodAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == namedArgumentName);
            if (argumentPair.Key is not null)
            {
                return new(argumentPair.Value.Value);
            }
        }
        return null;
    }
}
