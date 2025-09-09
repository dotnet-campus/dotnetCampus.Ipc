namespace dotnetCampus.Ipc.Core.ComponentModels;

/// <summary>
/// 表示一个可被赋值的量，对于值类型，含已赋值和未赋值两种状态；对于引用类型，含已赋值为 null、已赋值为非 null 和未赋值三种状态。
/// <para>你不应该直接使用这个类型本身，而是应该结合 Nullable 一起用，即 Assignable&lt;T&gt;?。其中 null 表示未赋值过，而非 null 表示已赋值过。</para>
/// </summary>
/// <typeparam name="T">值类型或引用类型。</typeparam>
internal readonly struct Assignable<T>
{
    public Assignable()
    {
        Value = default;
    }

    public Assignable(T? value)
    {
        Value = value;
    }

    public T? Value { get; }

    public static explicit operator Assignable<T>?(T value)
    {
        return new Assignable<T>(value);
    }

    public static implicit operator T?(Assignable<T>? assignable)
    {
        return assignable is null ? default : assignable.Value.Value;
    }
}
