namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 所有由本地问题导致的 IPC 异常。
/// </summary>
public class IpcLocalException : IpcException
{
    /// <summary>
    /// 创建 <see cref="IpcRemoteException"/> 的新实例。
    /// </summary>
    public IpcLocalException() : base()
    {
    }

    /// <summary>
    /// 创建带有自定义消息的 <see cref="IpcLocalException"/> 的新实例。
    /// </summary>
    /// <param name="message">自定义消息。</param>
    public IpcLocalException(string message) : base(message)
    {
    }

    /// <summary>
    /// 创建带有自定义消息和内部异常的 <see cref="IpcLocalException"/> 的新实例。
    /// </summary>
    /// <param name="message">自定义消息。</param>
    /// <param name="innerException">内部异常。</param>
    public IpcLocalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
