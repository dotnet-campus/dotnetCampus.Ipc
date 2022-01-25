using System;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// IPC 代理或对接匹配出现多义性时引发的异常。
/// </summary>
internal class IpcAmbiguousMatchException : IpcLocalException
{
    public IpcAmbiguousMatchException() : base()
    {
    }

    public IpcAmbiguousMatchException(string message) : base(message)
    {
    }

    public IpcAmbiguousMatchException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
