using System;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
/// <summary>
/// 为 <see cref="Garm{T}"/> 提供非泛型访问的方法。
/// </summary>
public interface IGarmObject
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// <see cref="IGarmObject"/> 的默认值，等效于可空类型的 null。
    /// </summary>
    internal static Garm<object?> Default { get; } = new Garm<object?>();
#endif

    /// <summary>
    /// 获取 <see cref="Garm{T}"/> 的值。
    /// </summary>
    object? Value { get; }

    /// <summary>
    /// 值的类型，即 <see cref="Garm{T}"/> 中的 T 的类型。
    /// </summary>
    public Type ValueType { get; }

    /// <summary>
    /// 如果对象是 IPC 类型，则此属性为 IPC 类型的类型对象；否则此属性为 null。
    /// </summary>
    Type? IpcType { get; }
}

#if !NET6_0_OR_GREATER
internal static class GarmObjectExtensions
{
    internal static IGarmObject Default { get; } = new Garm<object?>();
}
#endif
