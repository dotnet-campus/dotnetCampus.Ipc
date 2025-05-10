using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 直接路由里面的由远端处理请求过程中导致的异常，找不到请求对应的处理器
/// </summary>
public class JsonIpcDirectRoutedCanNotFindRequestHandlerException : JsonIpcDirectRoutedHandleRequestRemoteException
{
    internal JsonIpcDirectRoutedCanNotFindRequestHandlerException(PeerProxy remotePeer, string routedPath, JsonIpcDirectRoutedHandleRequestExceptionResponse exceptionResponse) : base
    (
        remotePeer,
        routedPath,
        exceptionResponse
    )
    {
    }
}
