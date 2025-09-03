#if NET6_0_OR_GREATER

using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

namespace dotnetCampus.Ipc.Serialization;

/// <summary>
/// 以 <see cref="System.Text.Json"/> 作为底层机制支持 IPC 对象传输。
/// </summary>
public class SystemTextJsonIpcObjectSerializer : IIpcObjectSerializer
{
    internal SystemTextJsonIpcObjectSerializer()
    {
        JsonSerializerContext = IpcInternalJsonSerializerContext.Default;
    }

    /// <summary>
    /// 创建 <see cref="SystemTextJsonIpcObjectSerializer"/> 的新实例。
    /// </summary>
    /// <param name="jsonSerializerContext">业务端用于业务对象序列化和反序列化的 JSON 序列化上下文。可由源生成器生成。</param>
    public SystemTextJsonIpcObjectSerializer(JsonSerializerContext jsonSerializerContext)
    {
        JsonSerializerContext = new IpcCompositeJsonSerializerContext(jsonSerializerContext);
    }

    /// <summary>
    /// 获取 JSON 序列化上下文。
    /// </summary>
    public JsonSerializerContext JsonSerializerContext { get; }

    /// <inheritdoc />
    public byte[] Serialize(object? value)
    {
        if (value is null)
        {
            return "{}"u8.ToArray();
        }

        var json = JsonSerializer.Serialize(value, value.GetType(), JsonSerializerContext);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, object? value)
    {
        if (value is null)
        {
            stream.Write("{}"u8);
            return;
        }

        JsonSerializer.Serialize(stream, value, value.GetType(), JsonSerializerContext);
    }

    /// <inheritdoc />
    public IpcJsonElement SerializeToElement(object? value)
    {
        if (value is null)
        {
            return default;
        }

        return new IpcJsonElement { RawValueOnSystemTextJson = JsonSerializer.SerializeToElement(value, value.GetType(), JsonSerializerContext), };
    }

    /// <inheritdoc />
    public T? Deserialize<T>(byte[] data, int start, int length)
    {
        var span = data.AsSpan(start, length);
        return JsonSerializer.Deserialize<T>(span, (JsonTypeInfo<T>) JsonSerializerContext.GetTypeInfo(typeof(T))!);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(Stream stream)
    {
        return JsonSerializer.Deserialize<T>(stream, (JsonTypeInfo<T>) JsonSerializerContext.GetTypeInfo(typeof(T))!);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(IpcJsonElement jsonElement)
    {
        if (jsonElement.RawValueOnSystemTextJson is not { } element)
        {
            return default;
        }
        return element.Deserialize<T>((JsonTypeInfo<T>) JsonSerializerContext.GetTypeInfo(typeof(T))!);
    }
}

internal class IpcCompositeJsonSerializerContext(JsonSerializerContext businessContext) : JsonSerializerContext(null)
{
    public override JsonTypeInfo? GetTypeInfo(Type type)
    {
        return businessContext.GetTypeInfo(type) ?? IpcInternalJsonSerializerContext.Default.GetTypeInfo(type);
    }

    protected override JsonSerializerOptions GeneratedSerializerOptions => businessContext.Options;
}

// 基础类型
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(byte))]
[JsonSerializable(typeof(char))]
[JsonSerializable(typeof(decimal))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(sbyte))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(uint))]
[JsonSerializable(typeof(ulong))]
[JsonSerializable(typeof(ushort))]
// 远程对象
[JsonSerializable(typeof(GeneratedProxyExceptionModel))]
[JsonSerializable(typeof(GeneratedProxyMemberInvokeModel))]
[JsonSerializable(typeof(GeneratedProxyMemberReturnModel))]
[JsonSerializable(typeof(GeneratedProxyObjectModel))]
// 路由
[JsonSerializable(typeof(JsonIpcDirectRoutedParameterlessType))]
[JsonSerializable(typeof(JsonIpcDirectRoutedHandleRequestExceptionResponse))]
[JsonSerializable(typeof(JsonIpcDirectRoutedHandleRequestExceptionResponse.JsonIpcDirectRoutedHandleRequestExceptionInfo))]
internal partial class IpcInternalJsonSerializerContext : JsonSerializerContext;

#endif
