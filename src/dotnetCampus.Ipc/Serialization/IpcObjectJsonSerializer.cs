using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace dotnetCampus.Ipc.Serialization
{
    public class IpcObjectJsonSerializer : IIpcObjectSerializer
    {
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
    }
}
