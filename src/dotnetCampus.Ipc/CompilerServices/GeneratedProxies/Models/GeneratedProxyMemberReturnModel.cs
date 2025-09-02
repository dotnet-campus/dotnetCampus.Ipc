using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Serialization;
using dotnetCampus.Ipc.Utils.Extensions;

#if UseNewtonsoftJson
using Newtonsoft.Json;
#endif
#if NET6_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

[DataContract]
internal class GeneratedProxyMemberReturnModel
{
    public GeneratedProxyMemberReturnModel()
    {
    }

    public GeneratedProxyMemberReturnModel(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        Exception = new GeneratedProxyExceptionModel(exception);
    }

    public GeneratedProxyMemberReturnModel(object? @return, IIpcObjectSerializer serializer)
    {
        if (@return is null)
        {
            // 当返回的对象为 null 时，返回值直接设定为 null。
            Return = null;
        }
        else
        {
            // 当返回对象为其他类型时，将尝试进行序列化。
            var jsonElement = KnownTypeConverter.Convert(@return, serializer);
            Return = new GeneratedProxyObjectModel(serializer)
            {
                Value = jsonElement,
            };
        }
    }

#if DEBUG
#if UseNewtonsoftJson
    [JsonProperty("i")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("i")]
#endif
    public GeneratedProxyMemberInvokeModel? Invoking { get; set; }
#endif

#if UseNewtonsoftJson
    [JsonProperty("r")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("r")]
#endif
    public GeneratedProxyObjectModel? Return { get; set; }

#if UseNewtonsoftJson
    [JsonProperty("e")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("e")]
#endif
    public GeneratedProxyExceptionModel? Exception { get; set; }

    [Obsolete("正在向 System.Text.Json 迁移，标记表示此方法正在迁移中，实现不可靠。", true)]
    public static IpcMessage Serialize(GeneratedProxyMemberReturnModel model)
    {
        var serializeMessage = JsonIpcMessageSerializer.Serialize("Return", model);

        return new IpcMessage(serializeMessage.Tag, serializeMessage.Body,
            (ulong) KnownMessageHeaders.RemoteObjectMessageHeader);
    }

    [Obsolete("正在向 System.Text.Json 迁移，标记表示此方法正在迁移中，实现不可靠。", true)]
    public static bool TryDeserialize(IpcMessage message, [NotNullWhen(true)] out GeneratedProxyMemberReturnModel? model)
    {
        const ulong header = (ulong) KnownMessageHeaders.RemoteObjectMessageHeader;
        if (message.TryGetPayload(header, out var deserializeMessage))
        {
            return JsonIpcMessageSerializer.TryDeserialize(deserializeMessage, out model);
        }
        else
        {
            // 如果业务头不对，那就不需要解析了
            model = null;
            return false;
        }
    }
}
