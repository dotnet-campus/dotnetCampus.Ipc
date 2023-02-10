using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;
using System.Text;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Serialization;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// IPC 传输过程中使用的内部模型，表示如何调用一个远端的方法。
    /// 目前为调试方便，暂使用 JSON 格式，易读。后续如果成为性能瓶颈，则可换成字节流。
    /// </summary>
    [DataContract]
    internal class GeneratedProxyMemberInvokeModel
    {
        [ContractPublicPropertyName(nameof(Id))]
        private string? _id;

        /// <summary>
        /// 远端对象 Id。
        /// 当同一个契约类型的对象存在多个时，则需要通过此 Id 进行区分。
        /// 空字符串（""）和空值（null）是相同含义，允许设 null 值，但获取时永不为 null（会自动转换为空字符串）。
        /// </summary>
        [DataMember(Name = "i")]
        [AllowNull]
        public string Id
        {
            get => _id ?? "";
            set => _id = value;
        }

        /// <summary>
        /// 远端对象的契约类型名称（含命名空间，不含 Token）。
        /// </summary>
        [DataMember(Name = "t")]
        public string? ContractFullTypeName { get; set; }

        /// <summary>
        /// 调用的成员 Id（由源代码生成器自动生成，唯一表示一个属性或方法）。
        /// </summary>
        [DataMember(Name = "d")]
        public ulong MemberId { get; internal set; }

        /// <summary>
        /// 调用的成员名称（属性名、方法名）。
        /// </summary>
        [DataMember(Name = "m")]
        public string? MemberName { get; set; }

        /// <summary>
        /// 指定如何调用远端的对象。目前支持读属性、写属性、调用方法和调用异步方法。
        /// </summary>
        [DataMember(Name = "c")]
        public MemberInvokingType CallType { get; set; }

        /// <summary>
        /// 参数列表。
        /// </summary>
        [DataMember(Name = "a")]
        public GeneratedProxyObjectModel?[]? Args { get; set; }

        /// <summary>
        /// 将调用远端方法的内部模型序列化成可供跨进程传输的 IPC 消息。
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public static IpcMessage Serialize(GeneratedProxyMemberInvokeModel model)
        {
            return JsonIpcMessageSerializer.Serialize(model.ToString(), model);
        }

        /// <summary>
        /// 尝试将跨进程传输过来的 IPC 消息反序列化成一个 IPC 方法调用内部模型。
        /// </summary>
        /// <param name="message">IPC 消息。</param>
        /// <param name="model"></param>
        /// <returns></returns>
        public static bool TryDeserialize(IpcMessage message, [NotNullWhen(true)] out GeneratedProxyMemberInvokeModel? model)
        {
            return JsonIpcMessageSerializer.TryDeserialize(message, out model);
        }

        /// <summary>
        /// 把方法调用过程以调用栈上一帧的方式表示出来。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var isMethod = CallType is MemberInvokingType.Method or MemberInvokingType.AsyncMethod;
            var builder = new StringBuilder();
            if (!string.IsNullOrEmpty(Id))
            {
                builder.Append('[').Append(Id).Append(']');
            }
            if (CallType is MemberInvokingType.AsyncMethod)
            {
                builder.Append("async ");
            }
            if (ContractFullTypeName is { } typeName)
            {
                builder.Append(typeName);
            }
            if (MemberName is { } memberName)
            {
                builder.Append('.');
                if (CallType is MemberInvokingType.GetProperty)
                {
                    builder.Append("get_");
                }
                else if (CallType is MemberInvokingType.SetProperty)
                {
                    builder.Append("set_");
                }
                builder.Append(memberName);
                if (isMethod)
                {
                    builder.Append('(');
                }
                else if (CallType is MemberInvokingType.SetProperty)
                {
                    builder.Append('=');
                }
            }
            if (Args is { } args)
            {
                for (var i = 0; i < args.Length; i++)
                {
                    var arg = args[i]?.Value;
                    if (i != 0)
                    {
                        builder.Append(", ");
                    }
                    builder.Append(arg?.ToString() ?? "null");
                }
            }
            if (isMethod)
            {
                builder.Append(')');
            }
            return builder.ToString();
        }
    }
}
