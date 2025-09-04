using System.Diagnostics.Contracts;

namespace dotnetCampus.Ipc.Utils;

/// <summary>
/// 可存储 16 个可空布尔值的结构。使用 2 位来存储一个可空布尔值。
/// </summary>
internal struct NullableBooleans
{
    /// <summary>
    /// 使用 2 位来存储一个可空布尔值，共 32 位可存储 16 个可空布尔值。
    /// </summary>
    private uint _booleans;

    /// <summary>
    /// 获取或设置指定索引处的可空布尔值，值范围 [0, 15]。
    /// </summary>
    /// <param name="index">索引，值范围 [0, 15]。</param>
    public bool? this[int index]
    {
        get => (_booleans & (1 << index * 2 + 1), _booleans & (1 << index * 2)) switch
        {
            // 高位 1 表示非 null，0 表示 null；低位 1 表示 true，0 表示 false
            (1, 0) => false,
            (1, 1) => true,
            _ => null,
        };
        set
        {
            if (value is null)
            {
                // 设置为 null：将索引处的两位都清零
                _booleans &= (uint) ~(3 << index * 2);
            }
            else if (value.Value)
            {
                // 设置为 true：将高位置 1，低位置 1
                _booleans |= (uint) (3 << index * 2);
            }
            else
            {
                // 设置为 false：将高位置 1，低位置清零
                _booleans = (_booleans & (uint) ~(1 << index * 2 + 1)) | (uint) (1 << index * 2);
            }
        }
    }

    [Pure]
    public bool GetBooleanAt(int indexFromEnd)
    {
        var index = 15 - indexFromEnd;
        return (_booleans & (1 << index)) != 0;
    }

    public void SetBooleanAt(int indexFromEnd, bool value)
    {
        var index = 15 - indexFromEnd;
        _booleans = value
            ? _booleans | (uint)(1 << index)
            : _booleans & (uint)~(1 << index);
    }
}
