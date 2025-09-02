using System.Diagnostics.CodeAnalysis;
using System.Text;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Extensions;

#if UseNewtonsoftJson
using Newtonsoft.Json;
#else
using System.Text.Json;
#endif

namespace dotnetCampus.Ipc.Serialization;

/// <summary>
/// 如果某 IPC 消息计划以 JSON 格式传输，那么可使用此类型来序列化和反序列化。
/// </summary>
internal static class JsonIpcMessageSerializer
{
    /// <summary>
    /// 将 IPC 内部模型序列化成可供跨进程传输的 IPC 消息。
    /// </summary>
    /// <param name="serializer">IPC 对象序列化器。</param>
    /// <param name="model">要序列化的 IPC 内部模型。</param>
    /// <param name="tag">用于追踪调试的消息标签。</param>
    /// <returns>用于 IPC 传输的消息。</returns>
    internal static IpcMessage SerializeToIpcMessage(
        this IIpcObjectSerializer serializer,
        object model,
        string tag)
    {
        var data = serializer.Serialize(model);
        var message = new IpcMessage(tag, new IpcMessageBody(data), (ulong) KnownMessageHeaders.RemoteObjectMessageHeader);
        return message;
    }

    /// <summary>
    /// 尝试将跨进程传输过来的 IPC 消息反序列化成 IPC 模块自动生成的内部模型。
    /// </summary>
    /// <param name="serializer"></param>
    /// <param name="message">IPC 消息。</param>
    /// <param name="model"></param>
    /// <returns></returns>
    internal static bool TryDeserializeFromIpcMessage<T>(this IIpcObjectSerializer serializer, IpcMessage message, [NotNullWhen(true)] out T? model)
        where T : class, new()
    {
        const ulong header = (ulong) KnownMessageHeaders.RemoteObjectMessageHeader;
        if (!message.TryGetPayload(header, out var deserializeMessage))
        {
            // 如果业务头不对，那就不需要解析了。
            model = null;
            return false;
        }

        var body = message.Body;
        var stringBody = Encoding.UTF8.GetString(body.Buffer, body.Start, body.Length);
        if (string.IsNullOrWhiteSpace(stringBody))
        {
            model = new T();
            return true;
        }

        try
        {
            model = serializer.Deserialize<T>(body.Buffer, body.Start, body.Length);
            return model != null;
        }
#if UseNewtonsoftJson
        catch (JsonSerializationException)
        {
            model = null;
            return false;
        }
        catch (JsonReaderException)
        {
            // Newtonsoft.Json.JsonReaderException
            //     Unexpected character encountered while parsing value: {0}.
            // JSON 字符串中包含不符合格式的字符。典型情况如下：
            //  * IPC 消息头被意外合入了消息体
            //  * 待发现……
            model = null;
            return false;
        }
#endif
#if NET6_0_OR_GREATER
        catch (JsonException)
        {
            model = null;
            return false;
        }
#endif
        catch
        {
            // 此反序列化过程抛出异常是合理行为（毕竟 IPC 谁都能发，但应该主要是 JsonSerializationException）。
            // 目前来看，还不知道有没有一些合理的正常的情况下会抛出其他异常，因此暂时在 !DEBUG 下不处理消息。
#if DEBUG
            throw;
#else
            model = null;
            return false;
#endif
        }
    }
}
