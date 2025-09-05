namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// IPC 代理或对接匹配找不到目标签名的成员时抛出。
/// </summary>
internal class IpcMemberNotFoundException : IpcLocalException
{
    public IpcMemberNotFoundException() : base()
    {
    }

    public IpcMemberNotFoundException(string message) : base(message)
    {
    }

    public IpcMemberNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
