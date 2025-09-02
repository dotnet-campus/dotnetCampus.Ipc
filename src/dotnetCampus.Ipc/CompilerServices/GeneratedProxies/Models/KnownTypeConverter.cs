using System;
using System.Collections.Generic;

#if UseNewtonsoftJson
using Newtonsoft.Json.Linq;
#endif

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

/// <summary>
/// 为了让对象能在 IPC 中传输，此类型提供 <see cref="JToken"/> 和对象之间的转换。
/// </summary>
internal static class KnownTypeConverter
{
#if UseNewtonsoftJson
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

    internal static T? ConvertBackFromJTokenOrObject<T>(object? obj)
    {
        if (obj is JToken jToken)
        {
            return ConvertBack<T>(jToken);
        }
        return (T?) obj;
    }

    private static T? ConvertBack<T>(JToken? jToken) => JValueConverters.TryGetValue(typeof(T), out var converter)
        ? (T) converter.deserializer((JValue) jToken!)
        : jToken switch
        {
            null => default!,
            JValue jValue => jValue.ToObject<T>(),
            JObject jObject => jObject.ToObject<T>(),
            JArray jArray => jArray.ToObject<T>(),
            _ => throw new NotSupportedException("不支持将其他 JToken 类型转换成 IPC 业务类型。")
        };
#else
    internal static object? Convert(object? value) => throw new NotSupportedException("当前不支持非 Newtonsoft.Json 的 JSON 序列化方式。");
    internal static T? ConvertBackFromJTokenOrObject<T>(object? obj) => throw new NotSupportedException("当前不支持非 Newtonsoft.Json 的 JSON 序列化方式。");
#endif
}
