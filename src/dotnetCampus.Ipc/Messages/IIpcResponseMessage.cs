namespace dotnetCampus.Ipc.Messages
{
    /// <summary>
    /// 处理客户端请求的结果
    /// </summary>
    public interface IIpcResponseMessage
    {
        /// <summary>
        /// 返回给对方的信息
        /// </summary>
        IpcMessage ResponseMessage { get; }
    }
}
