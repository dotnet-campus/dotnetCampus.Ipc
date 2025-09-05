using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using dotnetCampus.Ipc.CompilerServices.Attributes;

#if UseNewtonsoftJson
using Newtonsoft.Json;
#endif
#if NET6_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

[DataContract]
internal class GeneratedProxyObjectModel
{
    private string? _id;

    /// <summary>
    /// 远端对象 Id。
    /// 当同一个契约类型的对象存在多个时，则需要通过此 Id 进行区分。
    /// 空字符串（""）和空值（null）是相同含义，允许设 null 值，但获取时永不为 null（会自动转换为空字符串）。
    /// </summary>
    [AllowNull]
#if UseNewtonsoftJson
    [JsonProperty("i")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("i")]
#endif
    public string Id
    {
        get => _id ?? "";
        set => _id = value;
    }

    /// <summary>
    /// 远端对象的契约类型（即标记了 <see cref="IpcPublicAttribute"/> 的类型，不支持 <see cref="IpcShapeAttribute"/>）的名称。
    /// </summary>
#if UseNewtonsoftJson
    [JsonProperty("t")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("t")]
#endif
    public string? IpcPublicTypeFullName { get; set; }

    /// <summary>
    /// 远端对象的值。
    /// </summary>
#if NET6_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#endif
#if UseNewtonsoftJson
    [Newtonsoft.Json.JsonIgnore]
#endif
    public IpcJsonElement Value { get; set; }

#if UseNewtonsoftJson
    /// <summary>
    /// 原始 JSON 格式的远端对象的值。
    /// </summary>
    [Newtonsoft.Json.JsonProperty("v")]
#if NET6_0_OR_GREATER
    [System.Text.Json.Serialization.JsonIgnore]
#endif
    public Newtonsoft.Json.Linq.JToken? RawValueOnNewtonsoftJson
    {
        get => Value.RawValueOnNewtonsoftJson;
        set => Value = new IpcJsonElement { RawValueOnNewtonsoftJson = value };
    }
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// 原始 JSON 格式的远端对象的值。
    /// </summary>
    [System.Text.Json.Serialization.JsonPropertyName("v")]
#if UseNewtonsoftJson
    [Newtonsoft.Json.JsonIgnore]
#endif
    public System.Text.Json.JsonElement? RawValueOnSystemTextJson
    {
        get => Value.RawValueOnSystemTextJson;
        set => Value = new IpcJsonElement { RawValueOnSystemTextJson = value };
    }
#endif
}
