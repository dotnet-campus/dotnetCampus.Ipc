using dotnetCampus.Ipc.Serialization;

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
            if (RawValueOnNewtonsoftJson is null)
            {
                return true;
            }
            if (RawValueOnNewtonsoftJson.Type == Newtonsoft.Json.Linq.JTokenType.Null)
            {
                return true;
            }
#endif
#if NET6_0_OR_GREATER
            if (RawValueOnSystemTextJson is null)
            {
                return true;
            }
            if (RawValueOnSystemTextJson?.ValueKind == System.Text.Json.JsonValueKind.Null)
            {
                return true;
            }
#endif
            return true;
        }
    }

    public static IpcJsonElement Serialize(object? value, IIpcObjectSerializer serializer)
    {
        serializer.SerializeToElement(value);
        return value switch
        {
            null => new IpcJsonElement(),
#if NET6_0_OR_GREATER
            System.Text.Json.JsonElement => throw new InvalidOperationException($"编写错误检查，不应该传入 JSON 对象 {value.GetType().FullName}。"),
#endif
#if UseNewtonsoftJson
            Newtonsoft.Json.Linq.JToken => throw new InvalidOperationException($"编写错误检查，不应该传入 JSON 对象 {value.GetType().FullName}。"),
#endif
            _ => serializer.SerializeToElement(value),
        };
    }
}
