#if NET461_OR_GREATER ||  NETCOREAPP3_0_OR_GREATER
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

using dotnetCampus.Ipc.CompilerServices.Attributes;

using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
[DataContract]
internal class GeneratedProxyObjectModel
{
    [ContractPublicPropertyName(nameof(Id))]
    private string? _id;

    /// <summary>
    /// 远端对象 Id。
    /// 当同一个契约类型的对象存在多个时，则需要通过此 Id 进行区分。
    /// 空字符串（""）和空值（null）是相同含义，允许设 null 值，但获取时永不为 null（会自动转换为空字符串）。
    /// </summary>
    [DataMember(Name = "i")]
    [AllowNull]
    public string Id
    {
        get => _id ?? "";
        set => _id = value;
    }

    /// <summary>
    /// 远端对象类型（即标记了 <see cref="IpcPublicAttribute"/> 的类型，不支持 <see cref="IpcShapeAttribute"/>）的名称。
    /// </summary>
    [DataMember(Name = "t")]
    public string? IpcTypeFullName { get; set; }

    /// <summary>
    /// 远端对象的值。
    /// </summary>
    [DataMember(Name = "v")]
    public JToken? Value { get; set; }

    /// <summary>
    /// 将 <see cref="Value"/> 转换为 <typeparamref name="T"/> 类型的对象。
    /// </summary>
    /// <typeparam name="T">要转换的类型。</typeparam>
    /// <returns>转换后的对象。</returns>
    internal T? ToObject<T>()
    {
        return KnownTypeConverter.ConvertBackFromJTokenOrObject<T>(Value);
    }
}
#endif
