using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils;

namespace dotnetCampus.Ipc.Context;

/// <summary>
/// 连接已经存在的 Peer 的结果
/// </summary>
public readonly struct ConnectExistsPeerResult
{
    internal ConnectExistsPeerResult(PeerProxy? peerProxy, Task peerConnectFinishedTask)
    {
        PeerProxy = peerProxy;
        PeerConnectFinishedTask = peerConnectFinishedTask;
    }

    /// <summary>
    /// 连接到的 Peer 的代理
    /// </summary>
    public PeerProxy? PeerProxy { get; }

    /// <summary>
    /// 用于等待 Peer 完成连接完成
    /// </summary>
    public Task PeerConnectFinishedTask { get; }

    /// <summary>
    /// 是否连接成功
    /// </summary>
    [MemberNotNullWhen(true, nameof(PeerProxy))]
    public bool IsSuccess => PeerProxy != null;

    internal static ConnectExistsPeerResult Fail() => new ConnectExistsPeerResult(null, TaskUtils.CompletedTask);
}
