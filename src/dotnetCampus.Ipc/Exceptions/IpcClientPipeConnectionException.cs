using System;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// IPC的客户端连接失败异常
/// </summary>
public class IpcClientPipeConnectionException : IpcRemoteException
{
    /// <summary>
    /// IPC的客户端连接失败异常
    /// </summary>
    /// <param name="peerName">连接的服务名</param>
    /// <param name="message"></param>
    public IpcClientPipeConnectionException(string peerName, string? message = null) : this(peerName, null, message)
    {
    }

    /// <summary>
    /// IPC的客户端连接失败异常
    /// </summary>
    public IpcClientPipeConnectionException(string peerName, Exception? innerException, string? message = null) : base(message, innerException)
    {
        PeerName = peerName;
        _message = message ?? innerException?.Message;
    }
    
    /// <inheritdoc />
    public override string Message => _message ?? $"连接管道服务失败。服务管道名:{PeerName}";

    /// <summary>
    /// 连接的服务名
    /// </summary>
    public string PeerName { get; }

    private readonly string? _message;
}
