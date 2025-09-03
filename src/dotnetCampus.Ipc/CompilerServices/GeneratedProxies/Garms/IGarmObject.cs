using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

/// <summary>
/// 为 <see cref="Garm{T}"/> 提供非泛型访问的方法。
/// </summary>
public interface IGarmObject
{
    /// <summary>
    /// 获取 <see cref="Garm{T}"/> 的值。
    /// </summary>
    /// <remarks>
    /// 对于发送方，值可能是：<see langword="null"/>、可 JSON 序列化的任意对象（即将被序列化）、IPC 代理对象。<br/>
    /// 对于接收方，值可能是：<see langword="null"/>、<see cref="IpcJsonElement"/>（用于延迟反序列化）、IPC 代理对象。
    /// </remarks>
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

internal static class GarmObjectExtensions
{
    internal static IGarmObject Default { get; } = new Garm<object?>();
}
