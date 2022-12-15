using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Pipes.PipeConnectors;

/// <summary>
/// 配置客户端的管道连接
/// </summary>
public interface IIpcClientPipeConnector
{
    /// <summary>
    /// 进行管道连接，默认行为是调用 <code>await Task.Run(namedPipeClientStream.Connect)</code> 连接
    /// </summary>
    /// <param name="ipcClientPipeConnectionContext"></param>
    /// <returns></returns>
    Task<IpcClientNamedPipeConnectResult> ConnectNamedPipeAsync(IpcClientPipeConnectionContext ipcClientPipeConnectionContext);
}

/// <summary>
/// 客户端的管道连接结果
/// </summary>
public readonly struct IpcClientNamedPipeConnectResult
{
    /// <summary>
    /// 创建客户端的管道连接结果
    /// </summary>
    /// <param name="success"></param>
    public IpcClientNamedPipeConnectResult(bool success, string? reason = null)
    {
        Success = success;
        Reason = reason;
    }

    /// <summary>
    /// 连接是否成功
    /// </summary>
    public bool Success { get; }

    /// <summary>
    /// 连接成功或失败的原因
    /// </summary>
    public string? Reason { get; }
}
