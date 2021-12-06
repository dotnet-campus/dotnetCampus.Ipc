using dotnetCampus.Ipc.Context;

namespace dotnetCampus.Ipc
{
    /// <summary>
    /// 对等 IPC 通信的总提供类型。
    /// </summary>
    public interface IIpcProvider
    {
        /// <summary>
        /// 上下文信息
        /// </summary>
        IpcContext IpcContext { get; }
    }
}
