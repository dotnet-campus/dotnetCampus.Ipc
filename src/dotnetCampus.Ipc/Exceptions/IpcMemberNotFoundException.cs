namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// IPC 代理或对接匹配找不到目标签名的成员时抛出。
/// </summary>
public class IpcMemberNotFoundException : IpcLocalException
{
    /// <summary>
    /// 初始化 <see cref="IpcMemberNotFoundException"/> 类的新实例。
    /// </summary>
    public IpcMemberNotFoundException() : base()
    {
    }

    /// <summary>
    /// 初始化 <see cref="IpcMemberNotFoundException"/> 类的新实例。
    /// </summary>
    /// <param name="message">异常提示信息。</param>
    public IpcMemberNotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// 初始化 <see cref="IpcMemberNotFoundException"/> 类的新实例。
    /// </summary>
    /// <param name="message">异常提示信息。</param>
    /// <param name="innerException">引起当前异常的异常。</param>
    public IpcMemberNotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
