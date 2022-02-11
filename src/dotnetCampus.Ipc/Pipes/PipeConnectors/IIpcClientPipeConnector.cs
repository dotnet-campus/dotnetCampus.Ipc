using System.IO.Pipes;
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
    /// <param name="namedPipeClientStream"></param>
    /// <returns></returns>
    Task ConnectNamedPipeAsync(NamedPipeClientStream namedPipeClientStream);
}
