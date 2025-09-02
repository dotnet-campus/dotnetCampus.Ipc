using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

/// <summary>
/// <para>IPC GARM 模型。</para>
/// <para>GARM = Generated Argument and Return Model 生成类的参数与返回值模型。</para>
/// 在生成的 IPC 通信类中，用来表示参数和返回值。通过此类型表示的参数和返回值可以区分序列化的普通对象和需要 IPC 代理访问的 IPC 对象。
/// <list type="bullet">
/// <item>对于 IPC 代理，只有参数需要使用此模型，返回值不需要。因为参数可能是一个尚未对接的 IPC 对象。</item>
/// <item>对于 IPC 对接，只有返回值需要使用此模型，参数不需要。因为返回值可能是一个尚未对接的 IPC 对象。</item>
/// </list>
/// </summary>
public readonly struct Garm<T> : IGarmObject
{
    /// <summary>
    /// 创建一个 GARM 模型。
    /// </summary>
    public Garm()
    {
        Value = default;
        IpcType = null;
    }

    /// <summary>
    /// 创建一个 GARM 模型。
    /// </summary>
    /// <param name="value">对象的值。</param>
    public Garm(T? value)
    {
        Value = value;
        IpcType = null;
    }

    /// <summary>
    /// 创建一个 GARM 模型。
    /// </summary>
    /// <param name="value">对象的值。</param>
    /// <param name="ipcType">对象的 IPC 类型。</param>
    public Garm(T? value, Type? ipcType)
    {
        Value = value;
        IpcType = ipcType;
    }

    /// <summary>
    /// 如果对象是 IPC 类型，则此属性为 IPC 类型的类型对象；否则此属性为 null。
    /// <para>IPC 类型为标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型。</para>
    /// </summary>
    internal Type? IpcType { get; }

    /// <summary>
    /// 对象的值。
    /// </summary>
#nullable disable
    public T Value { get; }
#nullable restore

    object? IGarmObject.Value => Value;

    Type IGarmObject.ValueType => typeof(T);

    Type? IGarmObject.IpcType => IpcType;

    /// <summary>
    /// 将 IPC 代理的“非 IPC 类型”参数或 IPC 对接的“非 IPC 类型”返回值转换成“非 IPC 类型”的 GARM 模型。
    /// </summary>
    /// <param name="value">任意值。</param>
    public static implicit operator Garm<T>(T value)
    {
        if (value is IGarmObject go)
        {
            return new Garm<T>((T?) go.Value, go.IpcType);
        }
        return new Garm<T>(value);
    }
}
