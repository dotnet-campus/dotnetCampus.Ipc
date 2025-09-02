#if NET6_0_OR_GREATER

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

namespace dotnetCampus.Ipc.Serialization;

public class IpcObjectSystemJsonSerializer : IIpcObjectSerializer
{
    public IpcObjectSystemJsonSerializer(JsonSerializerContext jsonSerializerContext)
    {
        JsonSerializerContext = new IpcDefaultJsonSerializerContext(jsonSerializerContext);
    }

    public JsonSerializerContext JsonSerializerContext { get; }

    public byte[] Serialize(object? value)
    {
        if (value is null)
        {
            return "{}"u8.ToArray();
        }

        var json = JsonSerializer.Serialize(value, value.GetType(), JsonSerializerContext);
        return Encoding.UTF8.GetBytes(json);
    }

    public void Serialize(Stream stream, object? value)
    {
        if (value is null)
        {
            stream.Write("{}"u8);
            return;
        }

        JsonSerializer.Serialize(stream, value, value.GetType(), JsonSerializerContext);
    }

    public IpcJsonElement SerializeToElement(object? value)
    {
        if (value is null)
        {
            return default;
        }

        return new IpcJsonElement { RawValueOnSystemTextJson = JsonSerializer.SerializeToElement(value, value.GetType(), JsonSerializerContext), };
    }

    public T? Deserialize<T>(byte[] data, int start, int length)
    {
        var span = data.AsSpan(start, length);
        return JsonSerializer.Deserialize<T>(span, (JsonTypeInfo<T>) JsonSerializerContext.GetTypeInfo(typeof(T))!);
    }

    public T? Deserialize<T>(Stream stream)
    {
        return JsonSerializer.Deserialize<T>(stream, (JsonTypeInfo<T>) JsonSerializerContext.GetTypeInfo(typeof(T))!);
    }

    public T? Deserialize<T>(IpcJsonElement jsonElement)
    {
        if (jsonElement.RawValueOnSystemTextJson is not { } element)
        {
            return default;
        }
        return element.Deserialize<T>((JsonTypeInfo<T>) JsonSerializerContext.GetTypeInfo(typeof(T))!);
    }
}

file class IpcDefaultJsonSerializerContext : JsonSerializerContext
{
    public IpcDefaultJsonSerializerContext(JsonSerializerContext businessContext) : base(null)
    {
        _businessContext = businessContext;
    }

    private readonly JsonSerializerContext _businessContext;

    public override JsonTypeInfo? GetTypeInfo(Type type)
    {
        return _businessContext.GetTypeInfo(type) ?? IpcInternalJsonSerializerContext.Default.GetTypeInfo(type);
    }

    protected override JsonSerializerOptions GeneratedSerializerOptions => _businessContext.Options;
}

// 远程对象
[JsonSerializable(typeof(GeneratedProxyExceptionModel))]
[JsonSerializable(typeof(GeneratedProxyMemberInvokeModel))]
[JsonSerializable(typeof(GeneratedProxyMemberReturnModel))]
[JsonSerializable(typeof(GeneratedProxyObjectModel))]
// 路由
[JsonSerializable(typeof(JsonIpcDirectRoutedParameterlessType))]
// 异常信息
[JsonSerializable(typeof(JsonIpcDirectRoutedHandleRequestExceptionResponse))]
[JsonSerializable(typeof(JsonIpcDirectRoutedHandleRequestExceptionResponse.JsonIpcDirectRoutedHandleRequestExceptionInfo))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(long))]
[JsonSerializable(typeof(short))]
[JsonSerializable(typeof(bool))]
internal partial class IpcInternalJsonSerializerContext : JsonSerializerContext
{
}

#endif
