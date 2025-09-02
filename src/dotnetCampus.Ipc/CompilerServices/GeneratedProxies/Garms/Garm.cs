using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Serialization;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

/// <summary>
/// 提供非泛型的临时 GARM 模型，最终使用时请转换为泛型版本再用。
/// </summary>
internal readonly record struct Garm : IGarmObject
{
    private readonly object? _value;

    /// <summary>
    /// 创建一个存储 IPC 中 <see cref="IpcJsonElement"/> 的临时 GARM 模型。
    /// </summary>
    /// <param name="value">模型的值。</param>
    /// <param name="valueType">模型中值的类型。</param>
    public Garm(IpcJsonElement value, Type valueType)
    {
        _value = value;
        ValueType = valueType ?? throw new ArgumentNullException(nameof(valueType));
        IpcType = null;
    }

    /// <summary>
    /// 创建一个存储 IPC 中 IPC 代理对象的临时 GARM 模型。
    /// </summary>
    /// <param name="value">模型的值。</param>
    /// <param name="ipcType">模型中的 IPC 类型。</param>
    public Garm(object? value, Type ipcType)
    {
        _value = value;
        ValueType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        IpcType = ipcType;
    }

    /// <summary>
    /// 无法获取 GARM 模型的值！请先使用 <see cref="ToGeneric{T}"/> 转换为泛型类型后再获取值。
    /// </summary>
    /// <remarks>
    /// 数据在 IPC 通道中的传输会经过如下步骤：
    /// proxy -> IPC model in proxy side -> IPC model in joint side -> joint
    /// 其中，proxy 和 joint 均由编译器在用户端生成了泛型代码，因此这些上下文中都可以获取到泛型类型。
    /// 但中间的传输过程中没有泛型上下文，所以会用到此临时类型中转值。
    /// 由于中转的值可能是 JsonElement 这种未确定类型的值，为避免代码中意外写出错误的代码，因此禁止在此类型中获取值。
    /// </remarks>
    object? IGarmObject.Value => throw new InvalidOperationException("当 GARM 未获得泛型类型时，不可获取其值。请等待泛型上下文中调用 ToGeneric<T> 转换为泛型类型后再获取值。");

    /// <summary>
    /// 获取 GARM 模型的值的类型。
    /// </summary>
    public Type ValueType { get; }

    /// <summary>
    /// 如果对象是 IPC 类型，则此属性为 IPC 类型的类型对象；否则此属性为 null。
    /// </summary>
    public Type? IpcType { get; }

    /// <summary>
    /// 将 GARM 模型转换为泛型 GARM 模型。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型 GARM 模型。</returns>
    public Garm<T> ToGeneric<T>(IIpcObjectSerializer serializer) => _value switch
    {
        // 是 null。
        null => new Garm<T>(default, IpcType),
        // 是一个普通的 IPC 对象。
        IpcJsonElement jsonElement => new Garm<T>(IpcJsonElement.Deserialize<T>(jsonElement, serializer), IpcType),
        // 是一个 IPC 代理对象。
        _ => new Garm<T>((T?) _value, IpcType),
    };
}
