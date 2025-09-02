using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

#if UseNewtonsoftJson
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonElement = Newtonsoft.Json.Linq.JToken;
using JsonPropertyNameAttribute = Newtonsoft.Json.JsonPropertyAttribute;
#else
using System.Text.Json;
using System.Text.Json.Serialization;
using JsonElement = System.Text.Json.JsonElement;
using JsonPropertyNameAttribute = System.Text.Json.Serialization.JsonPropertyNameAttribute;
#endif

namespace dotnetCampus.Ipc.Serialization
{
    /// <summary>
    /// 对象序列化器
    /// </summary>
    public interface IIpcObjectSerializer
    {
        /// <summary>
        /// 序列化对象
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        byte[] Serialize(object value);

        void Serialize(Stream stream, object? value);

        IpcJsonElement SerializeToElement(object? value);

        /// <summary>
        /// 反序列化对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <param name="start"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        T? Deserialize<T>(byte[] data, int start, int length);

        T? Deserialize<T>(Stream stream);

        T? Deserialize<T>(IpcJsonElement jsonElement);
    }
}
