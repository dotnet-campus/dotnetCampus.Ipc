using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 标记一个类型是 IPC 代理壳。这个类型的所有成员都没有实现，唯一的作用就是定义 IPC 类型中的每个方法应如何通过 IPC 代理来访问。
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
#if !IPC_ANALYZER
public
#endif
class IpcShapeAttribute : IpcPublicAttribute
{
}
