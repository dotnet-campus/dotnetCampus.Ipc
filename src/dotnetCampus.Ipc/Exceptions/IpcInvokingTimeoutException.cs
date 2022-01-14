using System;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 当获取设置属性值或调用方法时，如果指定了 <see cref="IpcMemberAttribute.Timeout"/> 但未在超时时间完成前完成调用，则会抛出此异常。
/// </summary>
internal class IpcInvokingTimeoutException : IpcLocalException
{
    public IpcInvokingTimeoutException(string memberName, TimeSpan timeout) : base()
    {
        MemberName = VerifyMemberName(memberName);
        Timeout = VerifyTimeout(timeout);
    }

    public IpcInvokingTimeoutException(string memberName, TimeSpan timeout, string message) : base(message)
    {
        MemberName = VerifyMemberName(memberName);
        Timeout = VerifyTimeout(timeout);
    }

    public IpcInvokingTimeoutException(string memberName, TimeSpan timeout, string message, Exception innerException) : base(message, innerException)
    {
        MemberName = VerifyMemberName(memberName);
        Timeout = VerifyTimeout(timeout);
    }

    /// <summary>
    /// 获取本次调用的成员名称。
    /// </summary>
    public string MemberName { get; set; }

    /// <summary>
    /// 获取本次调用所设置的超时时间（毫秒）。
    /// </summary>
    public TimeSpan Timeout { get; set; }

    private string VerifyMemberName(string? memberName)
    {
        if (string.IsNullOrWhiteSpace(memberName))
        {
            throw new ArgumentException($"“{nameof(memberName)}”不能为 null 或空白。", nameof(memberName));
        }

        return memberName;
    }

    private TimeSpan VerifyTimeout(TimeSpan timeout)
    {
        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentException($"超时时间不能为 0 或负值，当前为 {timeout}。", nameof(timeout));
        }

        return timeout;
    }
}
