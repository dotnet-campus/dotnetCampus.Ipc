using System;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
/// <summary>
/// 为 <see cref="Garm{T}"/> 提供非泛型访问的方法。
/// </summary>
internal interface IGarmObject
{
    /// <summary>
    /// 获取 <see cref="Garm{T}"/> 的非泛型的值。
    /// </summary>
    object? Value { get; }

    /// <summary>
    /// 如果对象是 IPC 类型，则此属性为 IPC 类型的类型对象；否则此属性为 null。
    /// </summary>
    Type? IpcType { get; }
}
