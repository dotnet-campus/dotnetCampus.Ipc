using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models
{
    internal static class KnownTypeConverter
    {
        private static readonly Dictionary<Type, (Func<object, JValue> serializer, Func<JValue, object> deserializer)> JValueConverters = new()
        {
            {
                typeof(IntPtr),
                (x => new JValue(((IntPtr) x).ToInt64()), x => new IntPtr(x.ToObject<long>()))
            },
        };

        internal static JToken? Convert(object? value) => value switch
        {
            // null。
            null => JValue.CreateNull(),

            // dotnetCampus.Ipc 支持的类型。
            IntPtr @intPtr => new JValue(intPtr.ToInt64()),

            // 默认类型。
            _ => JToken.FromObject(value),
        };

        internal static T? ConvertBack<T>(JToken? jToken) => JValueConverters.TryGetValue(typeof(T), out var converter)
            ? (T) converter.deserializer((JValue) jToken!)
            : jToken switch
            {
                null => default!,
                JValue jValue => jValue.ToObject<T>(),
                JObject jObject => jObject.ToObject<T>(),
                JArray jArray => jArray.ToObject<T>(),
                _ => throw new NotSupportedException("不支持将其他 JToken 类型转换成 IPC 业务类型。")
            };
    }
}
