using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Serialization;
using dotnetCampus.Ipc.Utils.Extensions;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    [DataContract]
    internal class GeneratedProxyMemberReturnModel
    {
#if DEBUG
        [DataMember(Name = "i")]
        public GeneratedProxyMemberInvokeModel? Invoking { get; set; }
#endif

        [DataMember(Name = "r")]
        public GeneratedProxyObjectModel? Return { get; set; }

        [DataMember(Name = "e")]
        public GeneratedProxyExceptionModel? Exception { get; set; }

        public GeneratedProxyMemberReturnModel()
        {
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

        public GeneratedProxyMemberReturnModel(Exception exception)
        {
            if (exception is null)
            {
                throw new ArgumentNullException(nameof(exception));
            }

            Exception = new GeneratedProxyExceptionModel(exception);
        }

        public static IpcMessage Serialize(GeneratedProxyMemberReturnModel model)
        {
            var serializeMessage = JsonIpcMessageSerializer.Serialize("Return", model);

            return new IpcMessage(serializeMessage.Tag, serializeMessage.Body,
                (ulong) KnownMessageHeaders.RemoteObjectMessageHeader);
        }

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
}
