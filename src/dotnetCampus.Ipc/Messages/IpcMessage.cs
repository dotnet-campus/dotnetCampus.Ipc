using System.Diagnostics;

namespace dotnetCampus.Ipc.Messages
{
    /// <summary>
    /// 可在 IPC 框架中传输（收、发）的消息。
    /// </summary>
    public readonly struct IpcMessage
    {
        /// <summary>
        /// 创建一条可在 IPC 框架中传输的消息。
        /// </summary>
        /// <param name="tag">请标记此消息用于在调试过程中追踪。</param>
        /// <param name="body">IPC 消息的具体内容。</param>
        [DebuggerStepThrough]
        public IpcMessage(string tag, IpcMessageBody body)
        {
            Tag = tag;
            Body = body;
            CoreMessageType = CoreMessageType.Raw;
        }

        /// <summary>
        /// 创建一条可在 IPC 框架中传输的消息。
        /// </summary>
        /// <param name="tag">请标记此消息用于在调试过程中追踪。</param>
        /// <param name="data">IPC 消息的具体内容。</param>
        [DebuggerStepThrough]
        public IpcMessage(string tag, byte[] data) : this(tag, new IpcMessageBody(data))
        {
        }

        /// <summary>
        /// 创建一条可在 IPC 框架中传输的消息。
        /// </summary>
        /// <param name="tag">请标记此消息用于在调试过程中追踪。</param>
        /// <param name="body">IPC 消息的具体内容。</param>
        /// <param name="coreMessageType">由 IPC 框架传入，用以标记此消息可被 IPC 框架识别和处理的类型。</param>
        [DebuggerStepThrough]
        internal IpcMessage(string tag, IpcMessageBody body, CoreMessageType coreMessageType)
        {
            Tag = tag;
            Body = body;
            CoreMessageType = coreMessageType;
        }

        /// <summary>
        /// 创建一条可在 IPC 框架中传输的消息。
        /// </summary>
        /// <param name="tag">请标记此消息用于在调试过程中追踪。</param>
        /// <param name="data">IPC 消息的具体内容。</param>
        /// <param name="coreMessageType">由 IPC 框架传入，用以标记此消息可被 IPC 框架识别和处理的类型。</param>
        [DebuggerStepThrough]
        internal IpcMessage(string tag, byte[] data, CoreMessageType coreMessageType) : this(tag, new IpcMessageBody(data), coreMessageType)
        {
        }

        /// <summary>
        /// 用于在调试过程中追踪此 IPC 消息。
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// IPC 消息的具体内容。
        /// </summary>
        public IpcMessageBody Body { get; }

        /// <summary>
        /// 标记此消息可被 IPC 框架识别和处理的类型。
        /// </summary>
        internal CoreMessageType CoreMessageType { get; }
    }
}
