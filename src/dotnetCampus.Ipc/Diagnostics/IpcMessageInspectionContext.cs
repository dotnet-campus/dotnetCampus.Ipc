using System;
using System.Collections.Generic;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Diagnostics
{
    /// <summary>
    /// 为 <see cref="IIpcMessageInspector"/> 的检查提供上下文。
    /// </summary>
    public sealed class IpcMessageInspectionContext
    {
        private readonly IEnumerable<IpcMessageBody> _messageParts;

        internal IpcMessageInspectionContext(string localPeerName, string remotePeerName, Ack? ack, string tag, IEnumerable<IpcMessageBody> messageParts)
        {
            LocalPeerName = localPeerName ?? throw new ArgumentNullException(nameof(localPeerName));
            RemotePeerName = remotePeerName ?? throw new ArgumentNullException(nameof(remotePeerName));
            Ack = ack;
            Tag = tag ?? throw new ArgumentNullException(nameof(tag));
            _messageParts = messageParts ?? throw new ArgumentNullException(nameof(messageParts));
        }

        /// <summary>
        /// 本地 IPC 服务名称。
        /// </summary>
        public string LocalPeerName { get; }

        /// <summary>
        /// 发送目标的名称。
        /// </summary>
        public string RemotePeerName { get; }

        /// <summary>
        /// 标记此消息的描述性信息。
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// 消息的序号。
        /// </summary>
        public Ack? Ack { get; }

        /// <summary>
        /// 获取单个有意义消息的不同部分。其中，业务端检查时只能获取到业务内容的那一部分；框架端检查时可获取到消息头的非关键部分。
        /// </summary>
        public IEnumerable<IpcMessageBody> GetMessageParts() => _messageParts;
    }
}
