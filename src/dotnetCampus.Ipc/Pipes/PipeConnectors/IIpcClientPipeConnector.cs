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
    Task ConnectNamedPipeAsync(IpcClientPipeConnectionContext ipcClientPipeConnectionContext);
}
