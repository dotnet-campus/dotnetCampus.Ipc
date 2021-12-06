using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Serialization;

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

        public GeneratedProxyMemberReturnModel(object? @return)
        {
            if (@return is null)
            {
                // 当返回的对象为 null 时，返回值直接设定为 null。
                Return = null;
            }
            else
            {
                // 当返回对象为其他类型时，将尝试进行序列化。
                var jValue = KnownTypeConverter.Convert(@return);
                Return = new GeneratedProxyObjectModel
                {
                    Value = jValue,
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
            return JsonIpcMessageSerializer.Serialize("Return", model);
        }

        public static bool TryDeserialize(IpcMessage message, [NotNullWhen(true)] out GeneratedProxyMemberReturnModel? model)
        {
            return JsonIpcMessageSerializer.TryDeserialize(message, out model);
        }
    }
}
