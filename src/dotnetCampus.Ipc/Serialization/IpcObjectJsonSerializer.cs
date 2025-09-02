#if UseNewtonsoftJson
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonSerializer = Newtonsoft.Json.JsonSerializer;

namespace dotnetCampus.Ipc.Serialization
{
    public class IpcObjectJsonSerializer : IIpcObjectSerializer
    {
        private static readonly Dictionary<Type, (Func<object, JValue> serializer, Func<JValue, object> deserializer)> JValueConverters = new()
        {
            { typeof(IntPtr), (x => new JValue(((IntPtr) x).ToInt64()), x => new IntPtr(x.ToObject<long>())) },
        };

        public IpcObjectJsonSerializer()
        {
            JsonSerializer = JsonSerializer.CreateDefault();
        }

        public IpcObjectJsonSerializer(JsonSerializer jsonSerializer)
        {
            JsonSerializer = jsonSerializer;
        }

        private JsonSerializer JsonSerializer { get; }

        public byte[] Serialize(object value)
        {
            var json = JsonConvert.SerializeObject(value);
            return Encoding.UTF8.GetBytes(json);
        }

        public void Serialize(Stream stream, object? value)
        {
            using var textWriter = new StreamWriter(stream, Encoding.UTF8, leaveOpen: true, bufferSize: 2048);
            JsonSerializer.Serialize(textWriter, value);
        }

        public IpcJsonElement SerializeToElement(object? value) => new IpcJsonElement
        {
            RawValueOnNewtonsoftJson = value switch
            {
                // null。
                null => JValue.CreateNull(),

                // dotnetCampus.Ipc 支持的类型。
                IntPtr pointer => new JValue(pointer.ToInt64()),

                // 默认类型。
                _ => JToken.FromObject(value),
            },
        };

        public T? Deserialize<T>(byte[] byteList)
        {
            var json = Encoding.UTF8.GetString(byteList);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public T? Deserialize<T>(Stream stream)
        {
            using StreamReader reader = new StreamReader
            (
                stream,
#if !NETCOREAPP
                Encoding.UTF8,
                detectEncodingFromByteOrderMarks: false,
                bufferSize: 2048,
#endif
                leaveOpen: true
            );
            JsonReader jsonReader = new JsonTextReader(reader);
            return JsonSerializer.Deserialize<T>(jsonReader);
        }

        public T? Deserialize<T>(IpcJsonElement jsonElement) => jsonElement.RawValueOnNewtonsoftJson switch
        {
            null => default!,
            JValue jValue => jValue.ToObject<T>(),
            JObject jObject => jObject.ToObject<T>(),
            JArray jArray => jArray.ToObject<T>(),
            _ => throw new NotSupportedException("不支持将其他 JToken 类型转换成 IPC 业务类型。")
        };
    }
}
#endif
