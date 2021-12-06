namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 对方连接断开事件参数
    /// </summary>
    public interface IPeerConnectionBrokenArgs
    {
    }

    /// <summary>
    /// 断开的原因
    /// </summary>
    public enum BrokenReason
    {
        /// <summary>
        /// 未知原因，此时将会触发重连机制
        /// </summary>
        Unknown,

        /// <summary>
        /// 业务端显式退出，正常退出，不会触发重连机制
        /// </summary>
        BusinessExit,
    }
}
