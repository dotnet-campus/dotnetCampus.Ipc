using System.Text;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

#if UseNewtonsoftJson
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;
#endif

namespace dotnetCampus.Ipc.Serialization;

#if UseNewtonsoftJson
/// <summary>
/// 以 <see cref="Newtonsoft.Json"/> 作为底层机制支持 IPC 对象传输。
/// </summary>
[Obsolete("此类型已改名为 NewtonsoftJsonIpcObjectSerializer，并已在新的 .NET 框架中移除。")]
public class IpcObjectJsonSerializer : NewtonsoftJsonIpcObjectSerializer
{
    /// <summary>
    /// 创建 <see cref="IpcObjectJsonSerializer"/> 的新实例。
    /// </summary>
    public IpcObjectJsonSerializer()
    {
    }

    /// <summary>
    /// 创建 <see cref="IpcObjectJsonSerializer"/> 的新实例。
    /// </summary>
    /// <param name="jsonSerializer">用于序列化和反序列化的 <see cref="JsonSerializer"/> 实例。</param>
    public IpcObjectJsonSerializer(JsonSerializer jsonSerializer) : base(jsonSerializer)
    {
    }
}

/// <summary>
/// 以 <see cref="Newtonsoft.Json"/> 作为底层机制支持 IPC 对象传输。
/// </summary>
public class NewtonsoftJsonIpcObjectSerializer : IIpcObjectSerializer
{
    private static readonly Dictionary<Type, (Func<object, JValue> serializer, Func<JValue, object> deserializer)> JValueConverters = new()
    {
        { typeof(IntPtr), (x => new JValue(((IntPtr) x).ToInt64()), x => new IntPtr(x.ToObject<long>())) },
    };

    /// <summary>
    /// 创建 <see cref="NewtonsoftJsonIpcObjectSerializer"/> 的新实例。
    /// </summary>
    public NewtonsoftJsonIpcObjectSerializer()
    {
        JsonSerializer = JsonSerializer.CreateDefault();
    }

    /// <summary>
    /// 创建 <see cref="NewtonsoftJsonIpcObjectSerializer"/> 的新实例。
    /// </summary>
    /// <param name="jsonSerializer">用于序列化和反序列化的 <see cref="JsonSerializer"/> 实例。</param>
    public NewtonsoftJsonIpcObjectSerializer(JsonSerializer jsonSerializer)
    {
        JsonSerializer = jsonSerializer;
    }

    /// <summary>
    /// 获取 JSON 序列化器。
    /// </summary>
    private JsonSerializer JsonSerializer { get; }

    /// <inheritdoc />
    public byte[] Serialize(object value)
    {
        var json = JsonConvert.SerializeObject(value);
        return Encoding.UTF8.GetBytes(json);
    }

    /// <inheritdoc />
    public void Serialize(Stream stream, object? value)
    {
        using var textWriter = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true, bufferSize: 2048);
        JsonSerializer.Serialize(textWriter, value);
    }

    /// <inheritdoc />
    public IpcJsonElement SerializeToElement(object? value) => new IpcJsonElement
    {
        RawValueOnNewtonsoftJson = value switch
        {
            // null。
            null => JValue.CreateNull(),

            // dotnetCampus.Ipc 支持的类型。
            IntPtr pointer => new JValue(pointer.ToInt64()),

            // 默认类型。
            _ => JToken.FromObject(value),
        },
    };

    /// <inheritdoc />
    public T? Deserialize<T>(byte[] byteList, int start, int length)
    {
#if NETCOREAPP3_0_OR_GREATER
        var data = byteList.AsSpan(start, length);
#else
        var data = new byte[length];
        Array.Copy(byteList, start, data, 0, length);
#endif
        var json = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject<T>(json);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(Stream stream)
    {
        using StreamReader reader = new StreamReader
        (
            stream,
#if !NETCOREAPP
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 2048,
#endif
            leaveOpen: true
        );
        JsonReader jsonReader = new JsonTextReader(reader);
        return JsonSerializer.Deserialize<T>(jsonReader);
    }

    /// <inheritdoc />
    public T? Deserialize<T>(IpcJsonElement jsonElement) => jsonElement.RawValueOnNewtonsoftJson switch
    {
        null => default!,
        JValue jValue => jValue.ToObject<T>(),
        JObject jObject => jObject.ToObject<T>(),
        JArray jArray => jArray.ToObject<T>(),
        _ => throw new NotSupportedException("不支持将其他 JToken 类型转换成 IPC 业务类型。")
    };
}
#else
/// <summary>
/// 新的 .NET 框架中已不再支持 Newtonsoft.Json 依赖，请更换成基于 System.Text.Json 的 SystemTextJsonIpcObjectSerializer 实现。
/// </summary>
[Obsolete("新的 .NET 框架中已不再支持 Newtonsoft.Json 依赖，请更换成基于 System.Text.Json 的 SystemTextJsonIpcObjectSerializer 实现。", true)]
public class IpcObjectJsonSerializer : IIpcObjectSerializer
{
    byte[] IIpcObjectSerializer.Serialize(object value) => throw null!;
    void IIpcObjectSerializer.Serialize(Stream stream, object? value) => throw null!;
    IpcJsonElement IIpcObjectSerializer.SerializeToElement(object? value) => throw null!;
    T? IIpcObjectSerializer.Deserialize<T>(byte[] data, int start, int length) where T : default => throw null!;
    T? IIpcObjectSerializer.Deserialize<T>(Stream stream) where T : default => throw null!;
    T? IIpcObjectSerializer.Deserialize<T>(IpcJsonElement jsonElement) where T : default => throw null!;
}

#endif
