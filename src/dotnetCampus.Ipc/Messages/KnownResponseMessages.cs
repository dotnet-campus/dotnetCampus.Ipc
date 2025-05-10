using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace dotnetCampus.Ipc.Messages
{
    /// <summary>
    /// IPC 框架已知的 IPC 响应消息。
    /// </summary>
    public static class KnownIpcResponseMessages
    {
        /// <summary>
        /// 不会处理此种类型的 IPC 消息，因此返回了一个“不会处理”响应。
        /// </summary>
        public static IIpcResponseMessage CannotHandle { get; } = new NamedCanNotHandleIpcResponseMessage(nameof(CannotHandle));

        /// <summary>
        /// 是否传入的响应消息是一个“不会/能处理”响应消息。
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        internal static bool IsCanNotHandleResponseMessage(IIpcResponseMessage responseMessage)
        {
            return responseMessage is NamedCanNotHandleIpcResponseMessage;
        }

        /// <summary>
        /// 创建带特殊信息的“不会/能处理”响应消息。
        /// </summary>
        /// <param name="responseMessage"></param>
        /// <returns></returns>
        internal static IIpcResponseMessage CreateCanNotHandleResponseMessage(IpcMessage responseMessage)
        {
            return new NamedCanNotHandleIpcResponseMessage(responseMessage);
        }

        [DebuggerDisplay("IpcResponseMessage.{" + nameof(Name) + ",nq}")]
        private sealed class NamedCanNotHandleIpcResponseMessage : IIpcResponseMessage, IEquatable<NamedCanNotHandleIpcResponseMessage>
        {
            public NamedCanNotHandleIpcResponseMessage(string name)
            {
                Name = name;
                ResponseMessage = new(name, new byte[0]);
            }

            public NamedCanNotHandleIpcResponseMessage( IpcMessage responseMessage)
            {
                Name = responseMessage.Tag;
                ResponseMessage = responseMessage;
            }

            internal string Name { get; }

            public IpcMessage ResponseMessage { get; }

            public override bool Equals(object? obj)
            {
                return obj is NamedCanNotHandleIpcResponseMessage message && Equals(message);
            }

            public bool Equals(NamedCanNotHandleIpcResponseMessage? other)
            {
                return other is not null && Name == other.Name;
            }

            public override int GetHashCode()
            {
                return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
            }

            public static bool operator ==(NamedCanNotHandleIpcResponseMessage left, NamedCanNotHandleIpcResponseMessage right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(NamedCanNotHandleIpcResponseMessage left, NamedCanNotHandleIpcResponseMessage right)
            {
                return !(left == right);
            }
        }
    }
}
