using System;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 对方断开重连事件参数
    /// </summary>
    public interface IPeerReconnectedArgs
    {

    }

    class PeerReconnectedArgs : EventArgs, IPeerReconnectedArgs
    {

    }
}
