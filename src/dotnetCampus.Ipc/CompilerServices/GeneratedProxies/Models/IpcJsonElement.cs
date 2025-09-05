using dotnetCampus.Ipc.Serialization;

#if UseNewtonsoftJson
using Newtonsoft.Json.Linq;
#else
using System.Text.Json;
#endif

// ReSharper disable ReplaceWithSingleAssignment.False
namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

/// <summary>
/// 表示一个即将发送向远端的对象，或从远端接收到的对象。
/// </summary>
public readonly record struct IpcJsonElement
{
#if UseNewtonsoftJson
    /// <summary>
    /// 原始 JSON 格式的远端对象的值。
    /// </summary>
    public Newtonsoft.Json.Linq.JToken? RawValueOnNewtonsoftJson { get; init; }
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// 原始 JSON 格式的远端对象的值。
    /// </summary>
    public System.Text.Json.JsonElement? RawValueOnSystemTextJson { get; init; }
#endif

    /// <summary>
    /// 判断此 IPC 远端对象是否表示 <see langword="null"/>。
    /// </summary>
    public bool IsNull
    {
        get
        {
#if UseNewtonsoftJson
            var isNewtonsoftJsonNull = false;
            if (RawValueOnNewtonsoftJson is null)
            {
                isNewtonsoftJsonNull = true;
            }
            if (RawValueOnNewtonsoftJson?.Type == Newtonsoft.Json.Linq.JTokenType.Null)
            {
                isNewtonsoftJsonNull = true;
            }
#else
            var isNewtonsoftJsonNull = true;
#endif

            var isSystemTextJsonNull = false;
#if NET6_0_OR_GREATER
            if (RawValueOnSystemTextJson is null)
            {
                isSystemTextJsonNull = true;
            }
            if (RawValueOnSystemTextJson?.ValueKind == System.Text.Json.JsonValueKind.Null)
            {
                isSystemTextJsonNull = true;
            }
#endif
            return isNewtonsoftJsonNull && isSystemTextJsonNull;
        }
    }

    /// <summary>
    /// 将一个本地对象序列化成 IPC 可传输的 JSON 对象。
    /// </summary>
    /// <param name="value">本地对象。</param>
    /// <param name="serializer">对象序列化器。</param>
    /// <returns>IPC 可传输的 JSON 对象。</returns>
    public static IpcJsonElement Serialize(object? value, IIpcObjectSerializer serializer)
    {
        return value switch
        {
            null => new IpcJsonElement(),
#if UseNewtonsoftJson
            Newtonsoft.Json.Linq.JToken => throw new InvalidOperationException($"编写错误检查，不应该传入 JSON 对象 {value.GetType().FullName}。"),
#endif
#if NET6_0_OR_GREATER
            System.Text.Json.JsonElement => throw new InvalidOperationException($"编写错误检查，不应该传入 JSON 对象 {value.GetType().FullName}。"),
#endif
            _ => serializer.SerializeToElement(value),
        };
    }

    /// <summary>
    /// 将 IPC 可传输的 JSON 对象反序列化成本地对象。
    /// </summary>
    /// <param name="jsonElement">IPC 可传输的 JSON 对象。</param>
    /// <param name="serializer">对象序列化器。</param>
    /// <typeparam name="T">本地对象类型。</typeparam>
    /// <returns>本地对象。</returns>
    public static T? Deserialize<T>(IpcJsonElement jsonElement, IIpcObjectSerializer serializer)
    {
        if (jsonElement.IsNull)
        {
            return default!;
        }

#if UseNewtonsoftJson
        if (jsonElement.RawValueOnNewtonsoftJson is JValue jValue)
        {
            return jValue.ToObject<T>();
        }
        if (jsonElement.RawValueOnNewtonsoftJson is JObject jObject)
        {
            return jObject.ToObject<T>();
        }
        if (jsonElement.RawValueOnNewtonsoftJson is JArray jArray)
        {
            return jArray.ToObject<T>();
        }
        if (jsonElement.RawValueOnNewtonsoftJson is not null)
        {
            throw new NotSupportedException("不支持将其他 JToken 类型转换成 IPC 业务类型。");
        }
#endif

#if NET6_0_OR_GREATER
        if (jsonElement.RawValueOnSystemTextJson is { } json)
        {
            return serializer.Deserialize<T>(jsonElement);
        }
#endif

        throw new InvalidOperationException("不可能进入的分支，前面一定已经成功反序列化了。");
    }
}
