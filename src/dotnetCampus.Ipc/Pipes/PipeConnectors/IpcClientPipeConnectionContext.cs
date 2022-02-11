using System.IO.Pipes;
using System.Threading;

namespace dotnetCampus.Ipc.Pipes.PipeConnectors;

/// <summary>
/// 用于传递客户端的管道连接参数
/// </summary>
public readonly struct IpcClientPipeConnectionContext
{
    /// <summary>
    /// 用于传递客户端的管道连接参数
    /// </summary>
    public IpcClientPipeConnectionContext(string peerName, NamedPipeClientStream namedPipeClientStream,
        CancellationToken cancellationToken)
    {
        PeerName = peerName;
        NamedPipeClientStream = namedPipeClientStream;
        CancellationToken = cancellationToken;
    }

    /// <summary>
    /// 准备连接的管道名
    /// </summary>
    public string PeerName { get; }

    /// <summary>
    /// 用来连接的管道，需要调用 Connect 方法进行连接
    /// </summary>
    public NamedPipeClientStream NamedPipeClientStream { get; }

    /// <summary>
    /// 连接取消标记
    /// </summary>
    public CancellationToken CancellationToken { get; }
}
