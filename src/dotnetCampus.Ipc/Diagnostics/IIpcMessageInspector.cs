namespace dotnetCampus.Ipc.Diagnostics
{
    /// <summary>
    /// 实现此接口并注册到 <see cref="IpcMessageInspectorManager"/> 中可检查所有 IPC 收发的消息内容，以供调试。
    /// </summary>
    public interface IIpcMessageInspector
    {
        /// <summary>
        /// 实现此方法以检查从业务端发起的消息。
        /// </summary>
        /// <param name="context">包含消息发送的上下文。</param>
        void Send(IpcMessageInspectionContext context);

        /// <summary>
        /// 实现此方法以检查从框架最终发出的消息。
        /// </summary>
        /// <param name="context">包含消息发送的上下文。</param>
        void SendCore(IpcMessageInspectionContext context);

        /// <summary>
        /// 实现此方法以检查从框架收到的最原始的消息内容。
        /// </summary>
        /// <param name="context">包含消息接收的上下文。</param>
        void ReceiveCore(IpcMessageInspectionContext context);

        /// <summary>
        /// 实现此方法以检查从收到消息后发到业务后的业务部分。
        /// </summary>
        /// <param name="context">包含消息接收的上下文。</param>
        void Receive(IpcMessageInspectionContext context);
    }
}
