using System;

namespace dotnetCampus.Ipc.CompilerServices.Attributes
{
    /// <summary>
    /// 在标记了 <see cref="IpcPublicAttribute"/> 的类内部，标记一个属性是只读的。
    /// 当通过 IPC 访问过一次这个属性后，此属性不再变化，后续无需再通过 IPC 读取，可直接使用本地缓存的值。
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public sealed class IpcReadonlyAttribute : Attribute
    {
    }
}
