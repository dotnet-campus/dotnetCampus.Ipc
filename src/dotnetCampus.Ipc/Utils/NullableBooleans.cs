using System.Diagnostics.Contracts;

namespace dotnetCampus.Ipc.Utils;

/// <summary>
/// 可存储 8 个可空布尔值和 16 个布尔值的结构。使用 2 位来存储一个可空布尔值。共 32 位。
/// </summary>
internal struct NullableBooleans
{
    /// <summary>
    /// 高 16 位存储 16 个布尔值；低 16 位来存储 8 个可空布尔值。
    /// </summary>
    private uint _booleans;

    /// <summary>
    /// 获取或设置指定索引处的可空布尔值，值范围 [0, 7]。
    /// </summary>
    /// <param name="index">索引，值范围 [0, 7]。</param>
    public bool? this[int index]
    {
        get => ((_booleans & (1 << (index * 2 + 1))) >> (index * 2 + 1), (_booleans & (1 << (index * 2))) >> (index * 2)) switch
        {
            // 高位 1 表示非 null，0 表示 null；低位 1 表示 true，0 表示 false
            (1, 0) => false,
            (1, 1) => true,
            _ => null,
        };
        set => _booleans = value switch
        {
            // 设置为 null：将索引处的两位都清零
            null => _booleans & (uint)~(3 << (index * 2)),
            // 设置为 true：将高位置 1，低位置 1
            true => _booleans | (uint)(3 << (index * 2)),
            // 设置为 false：将高位置 1，低位置清零
            false => (_booleans & (uint)~(1 << (index * 2 + 1))) | (uint)(1 << (index * 2)),
        };
    }

    [Pure]
    public bool GetBooleanAt(int indexFromEnd)
    {
        var index = 31 - indexFromEnd;
        return (_booleans & (1 << index)) != 0;
    }

    public void SetBooleanAt(int indexFromEnd, bool value)
    {
        var index = 31 - indexFromEnd;
        _booleans = value
            ? _booleans | (uint)(1 << index)
            : _booleans & (uint)~(1 << index);
    }
}
