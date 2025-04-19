using System;
using System.Diagnostics;
using System.Text;
using dotnetCampus.Ipc.Context;

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
        public IpcMessage(string tag, IpcMessageBody body) : this(tag, body, (ulong) 0)
        {
        }

        /// <summary>
        /// 创建一条可在 IPC 框架中传输的消息。
        /// </summary>
        /// <param name="tag"></param>
        /// <param name="body"></param>
        /// <param name="ipcMessageHeader"></param>
        [DebuggerStepThrough]
        public IpcMessage(string tag, IpcMessageBody body, ulong ipcMessageHeader)
        {
            Tag = tag;
            Body = body;
            IpcMessageHeader = ipcMessageHeader;
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
        /// 用于在调试过程中追踪此 IPC 消息。
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// IPC 消息的具体内容。
        /// </summary>
        public IpcMessageBody Body { get; }

        ///// <summary>
        ///// 标记此消息可被 IPC 框架识别和处理的类型。
        ///// </summary>
        //internal CoreMessageType CoreMessageType { get; }

        /// <summary>
        /// 消息头类型，用来标识这条消息属于什么机制发送的消息。默认是 0 表示 Raw 裸消息。为 0 时，将不会带在发送的数据里面。框架内预设的消息类型，请参阅 <see cref="KnownMessageHeaders"/> 类
        /// </summary>
        public ulong IpcMessageHeader { get; }

        /// <summary>
        /// 调试使用的属性
        /// </summary>
        public KnownMessageHeaders Header => (KnownMessageHeaders) IpcMessageHeader;

        internal IpcBufferMessageContext ToIpcBufferMessageContextWithMessageHeader(IpcMessageCommandType ipcMessageCommandType)
        {
            if (IpcMessageHeader == 0)
            {
                return new IpcBufferMessageContext(Tag, ipcMessageCommandType, Body);
            }

            var header = BitConverter.GetBytes(IpcMessageHeader);
            var ipcBufferMessageContext =
                new IpcBufferMessageContext(Tag, ipcMessageCommandType, new IpcMessageBody(header), Body);
            return ipcBufferMessageContext;
        }

        internal string ToDebugString()
        {
            var guessBodyText = "";
            try
            {
                // 猜测 Body 的内容
                guessBodyText = Encoding.UTF8.GetString(Body.Buffer, Body.Start, Body.Length);
            }
            catch
            {
                // 忽略
            }

            return $"[IpcMessage] Header={Header};Tag={Tag};Body=[{string.Join(" ", Body.Buffer.Skip(Body.Start).Take(Body.Length).Select(t=>t.ToString("X2")))}](GuessText={guessBodyText})";
        }
    }
}
