using System;

namespace dotnetCampus.Ipc.Exceptions
{
    /// <summary>
    /// 进行管道连接时的异常
    /// </summary>
    public class IpcPipeConnectionException : IpcLocalException
    {
        internal IpcPipeConnectionException(string connectingPipeName, string localClientName, string remoteServerName, string message, Exception innerException) : base(message, innerException)
        {
            ConnectingPipeName = connectingPipeName;
            LocalPeerName = localClientName;
            RemotePeerName = remoteServerName;
        }

        /// <summary>
        /// 正在连接中的管道名。按照当前的设计，应该和 <see cref="RemotePeerName"/> 是相同的值
        /// </summary>
        public string ConnectingPipeName { get; }

        /// <summary>
        /// 本地当前的 Peer 名
        /// </summary>
        public string LocalPeerName { get; }

        /// <summary>
        /// 远端对方的 Peer 名
        /// </summary>
        public string RemotePeerName { get; }
    }
}
