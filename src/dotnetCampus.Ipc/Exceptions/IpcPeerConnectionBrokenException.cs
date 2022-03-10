namespace dotnetCampus.Ipc.Exceptions
{
    /// <summary>
    /// 远端对方断开连接异常
    /// </summary>
    public class IpcPeerConnectionBrokenException : IpcRemoteException
    {
        /// <summary>
        /// 远端对方断开连接异常
        /// </summary>
        public IpcPeerConnectionBrokenException() : base($"对方已断开")
        {
        }

        /// <summary>
        /// 远端对方断开连接异常
        /// </summary>
        public IpcPeerConnectionBrokenException(string message) : base(message)
        {
        }
    }
}
