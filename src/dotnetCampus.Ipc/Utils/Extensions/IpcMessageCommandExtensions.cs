using System;
using System.ComponentModel;
using System.Linq;
using System.Text;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Utils.Extensions
{
    /// <summary>
    /// 对 <see cref="IpcMessageCommandType"/> 提供扩展方法。
    /// </summary>
    internal static class IpcMessageCommandExtensions
    {
        /// <summary>
        /// 将 <see cref="CoreMessageType"/> 转换为可与 <see cref="IpcMessageCommandType"/> 进行位运算的标识位枚举。
        /// </summary>
        /// <param name="coreMessageType">要转换的 <see cref="CoreMessageType"/>。</param>
        /// <returns>转换后的 <see cref="IpcMessageCommandType"/>。</returns>
        public static IpcMessageCommandType AsMessageCommandTypeFlags(this CoreMessageType coreMessageType)
        {
            return (IpcMessageCommandType) ((int) coreMessageType << 3);
        }

        /// <summary>
        /// 将 <see cref="IpcMessageCommandType"/> 进行位运算以得出 <see cref="CoreMessageType"/> 消息类型。
        /// </summary>
        /// <param name="ipcMessageCommandType">要转换的 <see cref="IpcMessageCommandType"/>。</param>
        /// <returns>转换后的 <see cref="CoreMessageType"/>。</returns>
        public static CoreMessageType ToCoreMessageType(this IpcMessageCommandType ipcMessageCommandType)
        {
            return (0b_0000_0000_0011_1000 & (short) ipcMessageCommandType) switch
            {
                0b_0000_0000_0000_1000 => CoreMessageType.Raw,
                0b_0000_0000_0001_0000 => CoreMessageType.String,
                0b_0000_0000_0010_0000 => CoreMessageType.JsonObject,
                _ => CoreMessageType.NotMessageBody,
            };
        }

#if DEBUG
        internal static string? ToDebugMessageText(this IpcMessageBody body, IpcMessageCommandType ipcMessageCommandType)
        {
            return ipcMessageCommandType.ToCoreMessageType() switch
            {
                CoreMessageType.JsonObject => Encoding.UTF8.GetString(body.Buffer, body.Start, body.Length),
                CoreMessageType.String => Encoding.UTF8.GetString(body.Buffer, body.Start, body.Length),
                _ => Encoding.UTF8.GetString(body.Buffer, body.Start, body.Length),
            };
        }

        internal static string? ToDebugMessageText(this IpcBufferMessageContext context)
        {
            string? debugMessageBody = string.Join(Environment.NewLine, context.IpcBufferMessageList.Select(x =>
                x.ToDebugMessageText(context.IpcMessageCommandType) ?? ""));
            return string.IsNullOrEmpty(debugMessageBody) ? null : debugMessageBody;
        }

        internal static string? ToDebugMessageText(this PeerStreamMessageArgs args)
        {
            return args.ToPeerMessageArgs().Message.Body.ToDebugMessageText(args.MessageCommandType);
        }
#endif
    }
}
