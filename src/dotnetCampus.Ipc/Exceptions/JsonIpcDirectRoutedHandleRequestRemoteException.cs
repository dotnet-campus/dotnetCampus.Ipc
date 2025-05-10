using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 直接路由里面的由远端处理请求过程中导致的异常
/// </summary>
public class JsonIpcDirectRoutedHandleRequestRemoteException : JsonIpcDirectRoutedRemoteException
{
    internal JsonIpcDirectRoutedHandleRequestRemoteException(PeerProxy remotePeer, string routedPath,
        JsonIpcDirectRoutedHandleRequestExceptionResponse exceptionResponse) : base
    (
         $"""
         JsonIpcDirectRouted remote handle request exception.
         RemotePeer: {remotePeer.PeerName}
         RoutedPath: {routedPath}
         RemoteExceptionType: {exceptionResponse.ExceptionInfo!.ExceptionType}
         RemoteExceptionMessage: {exceptionResponse.ExceptionInfo!.ExceptionMessage}
         """, exceptionResponse.ExceptionInfo!.ExceptionStackTrace
    )
    {
        RemotePeer = remotePeer;
        RoutedPath = routedPath;
        ExceptionResponse = exceptionResponse;
    }

    public PeerProxy RemotePeer { get; }
    public string RoutedPath { get; }

    public string RemoteExceptionType => ExceptionInfo.ExceptionType!;
    public string? RemoteExceptionMessage => ExceptionInfo.ExceptionMessage;

    internal JsonIpcDirectRoutedHandleRequestExceptionResponse ExceptionResponse { get; }

    internal JsonIpcDirectRoutedHandleRequestExceptionResponse.JsonIpcDirectRoutedHandleRequestExceptionInfo
        ExceptionInfo => ExceptionResponse.ExceptionInfo!;

    /// <inheritdoc />
    public override string ToString()
    {
        return $"""
                {Message}
                
                RemoteException:{ExceptionInfo.ExceptionToString}
                """;
    }
}
