namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 直接路由里面的由远端问题导致的异常
/// </summary>
public class JsonIpcDirectRoutedRemoteException : IpcRemoteException
{
    internal JsonIpcDirectRoutedRemoteException()
    {
    }

    internal JsonIpcDirectRoutedRemoteException(string message) : base(message)
    {
    }

    internal JsonIpcDirectRoutedRemoteException(string message, string? remoteStackTrace) : base(message, remoteStackTrace)
    {
    }

    internal JsonIpcDirectRoutedRemoteException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
