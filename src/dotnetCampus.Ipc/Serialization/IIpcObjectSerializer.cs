using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

namespace dotnetCampus.Ipc.Serialization;

/// <summary>
/// 支持 IPC 对象序列化和反序列化的接口。
/// </summary>
public interface IIpcObjectSerializer
{
    /// <summary>
    /// 序列化对象为二进制数据。
    /// </summary>
    /// <param name="value">要序列化的对象。</param>
    /// <returns>序列化后的二进制数据。</returns>
    byte[] Serialize(object value);

    /// <summary>
    /// 序列化对象到流。
    /// </summary>
    /// <param name="stream">序列化到此流。</param>
    /// <param name="value">要序列化的对象。</param>
    void Serialize(Stream stream, object? value);

    /// <summary>
    /// 序列化为 IPC 中间 JSON 对象。此中间对象将经过进一步处理在再进一步序列化以进行传输。
    /// </summary>
    /// <param name="value">要序列化的对象。</param>
    /// <returns>IPC 中间 JSON 对象。</returns>
    IpcJsonElement SerializeToElement(object? value);

    /// <summary>
    /// 从二进制数据反序列化为对象。
    /// </summary>
    /// <typeparam name="T">要反序列化的对象类型。</typeparam>
    /// <param name="data">二进制数据。</param>
    /// <param name="start">所需数据在 <paramref name="data"/> 中的起始位置。</param>
    /// <param name="length">所需数据的长度。</param>
    /// <returns>反序列化得到的对象。</returns>
    T? Deserialize<T>(byte[] data, int start, int length);

    /// <summary>
    /// 从流反序列化为对象。
    /// </summary>
    /// <param name="stream">要从中反序列化的流。</param>
    /// <typeparam name="T">要反序列化的对象类型。</typeparam>
    /// <returns>反序列化得到的对象。</returns>
    T? Deserialize<T>(Stream stream);

    /// <summary>
    /// 从 IPC 中间 JSON 对象反序列化为对象。
    /// </summary>
    /// <typeparam name="T">要反序列化的对象类型。</typeparam>
    /// <param name="jsonElement">IPC 中间 JSON 对象。</param>
    /// <returns>反序列化得到的对象。</returns>
    T? Deserialize<T>(IpcJsonElement jsonElement);
}
