using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Extensions;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Diagnostics
{
    interface IIpcMessageTracker
    {
        string Tag { get; }

        void Debug(string message);
    }

    /// <summary>
    /// <para>在 IPC 框架内部，提供消息收发的全程追踪。无论此消息被封装还是解包，都会在此类型的帮助下包含全程追踪信息。</para>
    /// 在以下情况下，此追踪可完全保证某个消息的来源和去路：
    /// <list type="bullet">
    /// <item>业务方准备发一条消息直至此消息最终通过管道发出的全链路。</item>
    /// <item>管道收到一条消息直至这条消息传递给业务的全链路。</item>
    /// </list>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class IpcMessageTracker<T> : IIpcMessageTracker
    {
        /// <summary>
        /// 本地 Peer 名称。
        /// </summary>
        private readonly string _localPeerName;

        /// <summary>
        /// 本消息将发至此 Peer 或本消息从此 Peer 来。
        /// </summary>
        private readonly string _remotePeerName;

        /// <summary>
        /// 输出日志的方法。
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// 记录追踪过程中产生的所有日志。
        /// </summary>
        private readonly List<string> _trackingLogs;

        /// <summary>
        /// 使用追踪器追踪某个消息。
        /// </summary>
        /// <param name="localPeerName">本地 Peer 名称。</param>
        /// <param name="remotePeerName">本消息将发至此 Peer 或本消息从此 Peer 来。</param>
        /// <param name="message">要追踪的消息。</param>
        /// <param name="tag">此消息的业务标记。</param>
        /// <param name="logger">日志。</param>
        public IpcMessageTracker(string localPeerName, string remotePeerName, T message, string tag, ILogger logger)
        {
            _localPeerName = localPeerName ?? throw new ArgumentNullException(nameof(localPeerName));
            _remotePeerName = remotePeerName ?? throw new ArgumentNullException(nameof(remotePeerName));
            Tag = tag ?? "";
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _trackingLogs = new();
            Message = message;
        }

        /// <summary>
        /// 继续追踪封装或解包的消息。
        /// </summary>
        /// <param name="localPeerName">本地 Peer 名称。</param>
        /// <param name="remotePeerName">本消息将发至此 Peer 或本消息从此 Peer 来。</param>
        /// <param name="message">要追踪的消息。</param>
        /// <param name="tag">此消息的业务标记。</param>
        /// <param name="logger">日志。</param>
        /// <param name="trackingLogs">已记录的追踪。</param>
        private IpcMessageTracker(string localPeerName, string remotePeerName, T message, string tag, ILogger logger, List<string> trackingLogs)
        {
            _localPeerName = localPeerName ?? throw new ArgumentNullException(nameof(localPeerName));
            _remotePeerName = remotePeerName ?? throw new ArgumentNullException(nameof(remotePeerName));
            Tag = tag ?? "";
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _trackingLogs = trackingLogs;
            Message = message;
        }

        /// <summary>
        /// 此消息的业务标记。
        /// </summary>
        public string Tag { get; }

        /// <summary>
        /// 追踪的对象
        /// </summary>
        public T Message { get; }

        /// <summary>
        /// 对追踪的消息包 <see cref="Message"/> 进行封装或解包后，返回对此封装或解包后的新追踪器。
        /// </summary>
        /// <typeparam name="TNext">封装或解包后的新消息类型。</typeparam>
        /// <param name="nextMessage">封装或解包后的新消息实例。</param>
        /// <returns>对新消息的追踪器，具有原消息的追踪记录。</returns>
        public IpcMessageTracker<TNext> TrackNext<TNext>(TNext nextMessage)
        {
            return new IpcMessageTracker<TNext>(_localPeerName, _remotePeerName, nextMessage, Tag, _logger, _trackingLogs);
        }

        [Conditional("DEBUG")]
        public void Debug(string message, [CallerMemberName] string memberName = "")
        {
            _logger.Log(LogLevel.Trace, new EventId(0, _localPeerName), this, null,
                (s, e) => $"[IPC] [{Tag}] [{memberName ?? "null"}] {message}");
        }

        void IIpcMessageTracker.Debug(string message)
        {
            _logger.Log(LogLevel.Trace, new EventId(0, _localPeerName), this, null,
                (s, e) => $"[IPC] [{Tag}] {message}");
        }

        /// <summary>
        /// 标记正在执行关键步骤，然后将全部消息内容记录下来用于调试。
        /// </summary>
        /// <param name="stepName">步骤名。</param>
        /// <param name="ack">消息序号（为 null 表示无法确定 ACK）。</param>
        /// <param name="message">消息体。</param>
        [Conditional("DEBUG")]
        internal void CriticalStep(string stepName, Ack? ack, IpcMessageBody message)
        {
            CriticalStep(stepName, ack, new[] { message });
        }

        /// <summary>
        /// 标记正在执行关键步骤，然后将全部消息内容记录下来用于调试。
        /// </summary>
        /// <param name="stepName">步骤名。</param>
        /// <param name="ack">消息序号（为 null 表示无法确定 ACK）。</param>
        /// <param name="messages">消息体（不含关键消息头，含其他消息头）。</param>
        [Conditional("DEBUG")]
        internal void CriticalStep(string stepName, Ack? ack, IEnumerable<IpcMessageBody> messages)
        {
            var manager = IpcMessageInspectorManager.FromLocalPeerName(_localPeerName);
            var context = new IpcMessageInspectionContext(_localPeerName, _remotePeerName, ack, Tag, messages);
            manager.Call(stepName switch
            {
                // 从业务端发起的请求或回复。
                "Send" => i => i.Send(context),
                // 框架层最终发送的请求或回复。
                "SendCore" => i => i.SendCore(context),
                // 框架层最开始收到的消息。
                "ReceiveCore" => i => i.ReceiveCore(context),
                // 从收到后发至业务端的消息。
                "Receive" => i => i.Receive(context),
                _ => throw new NotSupportedException($"暂不支持检查 {stepName} 关键步骤名称的消息。"),
            });
        }
    }
}
