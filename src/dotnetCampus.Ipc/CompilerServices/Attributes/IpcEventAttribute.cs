using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 指定此事件的 IPC 代理访问方式和对接方式。
/// </summary>
[AttributeUsage(AttributeTargets.Event, AllowMultiple = false, Inherited = true)]
#if !IPC_ANALYZER
public
#endif
sealed class IpcEventAttribute : IpcMemberAttribute
{
    /// <summary>
    /// 指定此事件的 IPC 代理访问方式和对接方式。
    /// </summary>
    public IpcEventAttribute()
    {
    }
}
