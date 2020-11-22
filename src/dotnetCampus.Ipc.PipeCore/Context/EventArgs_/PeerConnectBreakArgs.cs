using System;
using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.Abstractions.Context;

namespace dotnetCampus.Ipc.PipeCore
{
    /// <summary>
    /// 对方连接断开事件参数
    /// </summary>
    public class PeerConnectBreakArgs : EventArgs, IPeerConnectBreakArgs
    {

    }
}
