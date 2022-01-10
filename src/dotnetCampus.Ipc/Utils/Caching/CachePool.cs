using System;
using System.Collections.Generic;

namespace dotnetCampus.Ipc.Utils.Caching;

/// <summary>
/// 如果获取 <typeparamref name="TSource"/> 对应信息的过程比较耗时（例如反射），
/// 则可使用 <see cref="CachePool{TSource,TCache}"/> 对此过程进行缓存。
/// </summary>
/// <typeparam name="TSource">为了获取信息所需的源对象。</typeparam>
/// <typeparam name="TCache">获取的信息将会储存在此类型的缓存对象中。</typeparam>
internal sealed class CachePool<TSource, TCache> where TSource : notnull
{
    /// <summary>
    /// 使用特定的转换器创建 <see cref="CachePool{TSource,TCache}"/> 的新实例。
    /// </summary>
    /// <param name="conversion">从源对象到目标对象的转换方法，此方法仅执行一次。</param>
    /// <param name="threadSafe">如果获取缓存的过程可能在不同线程，则设置此值为 true，以便让缓存过程是线程安全的。</param>
    public CachePool(Func<TSource, TCache> conversion, bool threadSafe = false)
        : this(new CachePoolValueMap<TSource, TCache>(conversion), threadSafe)
    {
    }

    /// <summary>
    /// 使用特定的转换器创建 <see cref="CachePool{TSource,TCache}"/> 的新实例。<para />
    /// 你也可以使用隐式转换创建新实例：<para />
    /// <code>
    /// <see cref="CachePool{TSource,TCache}"/> pool = <see cref="CachePoolValueMap{TSource,TCache}"/>(key => "")<para />
    /// {<para />
    ///     [1] = o => "1",<para />
    ///     [2] = o => "2",<para />
    /// };<para />
    /// </code>
    /// </summary>
    /// <param name="conversion">从源对象到目标对象的转换器。</param>
    /// <param name="threadSafe">如果获取缓存的过程可能在不同线程，则设置此值为 true，以便让缓存过程是线程安全的。</param>
    public CachePool(CachePoolValueMap<TSource, TCache> conversion, bool threadSafe = false)
    {
        _converter = conversion ?? throw new ArgumentNullException(nameof(conversion));
        _locker = threadSafe ? new object() : null;
    }

    /// <summary>
    /// 从缓存池中获取缓存的信息，如果从未获取过信息，则将会执行一次
    /// 从 <typeparamref name="TSource"/> 到 <typeparamref name="TCache"/> 的转换。
    /// </summary>
    /// <param name="source">为了获取信息所需的源对象。</param>
    /// <returns>缓存的对象。</returns>
    public TCache this[TSource source]
    {
        get => GetOrCacheValue(source);
    }

    /// <summary>
    /// 获取锁，如果此值为 null，说明无需加锁。
    /// </summary>
    private readonly object? _locker;

    /// <summary>
    /// 获取转换对象的方法。
    /// </summary>
    private readonly CachePoolValueMap<TSource, TCache> _converter;

    /// <summary>
    /// 获取缓存了 <typeparamref name="TCache"/> 的字典。
    /// </summary>
    private readonly Dictionary<TSource, TCache> _cacheDictionary = new();

    /// <summary>
    /// 从缓存池中获取缓存的信息，如果从未获取过信息，则将会执行一次
    /// 从 <typeparamref name="TSource"/> 到 <typeparamref name="TCache"/> 的转换。
    /// </summary>
    /// <param name="source"></param>
    private TCache GetOrCacheValue(TSource source)
    {
        // 如果不需要加锁，则直接返回值。
        if (_locker == null)
        {
            return GetOrCacheValue();
        }

        // 如果需要加锁，则加锁后返回值。
        lock (_locker)
        {
            return GetOrCacheValue();
        }

        // 如果存在缓存，则获取缓存；否则从源值转换成缓存。
        TCache GetOrCacheValue()
        {
            if (!_cacheDictionary.TryGetValue(source, out var cache))
            {
                // 转换值并放入字典缓存。
                cache = _converter.Convert(source);
                _cacheDictionary[source] = cache;
            }

            return cache;
        }
    }
}
