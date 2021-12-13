using System;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Runtime.Serialization;

using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models
{
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
        /// 远端对象的类型名称（含命名空间，不含 Token）。
        /// </summary>
        [DataMember(Name = "t")]
        public string? AssemblyQualifiedName { get; set; }

        [DataMember(Name = "v")]
        public JToken? Value { get; set; }
    }
}
