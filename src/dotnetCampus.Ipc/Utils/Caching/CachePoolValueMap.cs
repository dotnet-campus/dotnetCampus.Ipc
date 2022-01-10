using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace dotnetCampus.Ipc.Utils.Caching;

/// <summary>
/// 为 <see cref="CachePool{TSource,TCache}"/> 的多值转换提供转换器。
/// </summary>
/// <typeparam name="TSource">转换所需的源对象。</typeparam>
/// <typeparam name="TCache">转换的值将会储存在此类型的缓存对象中。</typeparam>
internal sealed class CachePoolValueMap<TSource, TCache> where TSource : notnull
{
    /// <summary>
    /// 创建 <see cref="CachePoolValueMap{TSource,TCache}"/> 类型的新实例，
    /// 用于为 <see cref="CachePool{TSource,TCache}"/> 提供多值转换。
    /// </summary>
    /// <param name="defaultConverter">默认的转换函数。</param>
    public CachePoolValueMap(Func<TSource, TCache> defaultConverter)
    {
        _converter = defaultConverter ?? throw new ArgumentNullException(nameof(defaultConverter));
    }

    /// <summary>
    /// 获取或设置值转换方法。可以通过此实例额外设置特殊值的转换规则。
    /// </summary>
    /// <param name="source">转换的源对象。</param>
    public Func<TSource, TCache> this[TSource source]
    {
        get => _additionalConverters.TryGetValue(source, out var converter) ? converter : _converter;
        set => _additionalConverters[source] = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// 转换一个值。
    /// </summary>
    /// <param name="source">转换的源对象。</param>
    /// <returns>缓存后的对象。</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal TCache Convert(TSource source) => this[source](source);

    /// <summary>
    /// 获取转换对象的方法。
    /// </summary>
    private readonly Func<TSource, TCache> _converter;

    /// <summary>
    /// 获取特殊指定的获取 <typeparamref name="TCache"/> 方法的字典。
    /// 此字典中的转换器优先级高于 <see cref="_converter"/> 方法。
    /// </summary>
    private readonly Dictionary<TSource, Func<TSource, TCache>> _additionalConverters
        = new();

    /// <summary>
    /// 从 <see cref="CachePoolValueMap{TSource,TCache}"/> 转到 <see cref="CachePool{TSource,TCache}"/> 的实例。
    /// </summary>
    /// <param name="converter"><see cref="CachePoolValueMap{TSource,TCache}"/> 的实例。</param>
    [return: NotNullIfNotNull("converter")]
    public static implicit operator CachePool<TSource, TCache>?(
        CachePoolValueMap<TSource, TCache>? converter)
        => converter is null ? null : new CachePool<TSource, TCache>(converter);

    /// <summary>
    /// 转到 <see cref="CachePool{TSource,TCache}"/> 的实例。
    /// </summary>
    public CachePool<TSource, TCache> ToCachePool() => new(this);
}
