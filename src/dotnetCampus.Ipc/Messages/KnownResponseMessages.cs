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
        public static IIpcResponseMessage CannotHandle { get; } = new NamedIpcResponseMessage(nameof(CannotHandle));

        [DebuggerDisplay("IpcResponseMessage.{" + nameof(Name) + ",nq}")]
        private sealed class NamedIpcResponseMessage : IIpcResponseMessage, IEquatable<NamedIpcResponseMessage>
        {
            public NamedIpcResponseMessage(string name)
            {
                Name = name;
                ResponseMessage = new(name, new byte[0]);
            }

            internal string Name { get; }

            public IpcMessage ResponseMessage { get; }

            public override bool Equals(object? obj)
            {
                return obj is NamedIpcResponseMessage message && Equals(message);
            }

            public bool Equals(NamedIpcResponseMessage? other)
            {
                return other is not null && Name == other.Name;
            }

            public override int GetHashCode()
            {
                return 539060726 + EqualityComparer<string>.Default.GetHashCode(Name);
            }

            public static bool operator ==(NamedIpcResponseMessage left, NamedIpcResponseMessage right)
            {
                return left.Equals(right);
            }

            public static bool operator !=(NamedIpcResponseMessage left, NamedIpcResponseMessage right)
            {
                return !(left == right);
            }
        }
    }
}
