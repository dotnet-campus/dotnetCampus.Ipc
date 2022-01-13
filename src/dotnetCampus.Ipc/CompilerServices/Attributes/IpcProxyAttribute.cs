using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes;

/// <summary>
/// 标记在一个空白的分部类型上，可以将此类型视为某个 IPC 契约类型的代理来使用。
/// <para>
/// 这个特性不会在类型的继承树中传递。因此即使基类标记了此特性，所有派生类想要公开 IPC 对象也必须依次标记。
/// </para>
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
#if !IPC_ANALYZER
public
#endif
class IpcProxyAttribute : Attribute
{
}
