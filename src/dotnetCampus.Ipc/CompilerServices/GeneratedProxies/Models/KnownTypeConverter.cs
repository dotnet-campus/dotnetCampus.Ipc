#if UseNewtonsoftJson
using dotnetCampus.Ipc.Serialization;
using Newtonsoft.Json.Linq;
#else
using System.Text.Json;
using dotnetCampus.Ipc.Serialization;
#endif

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

#if UseNewtonsoftJson
/// <summary>
/// 为了让对象能在 IPC 中传输，此类型提供 <see cref="JToken"/> 和对象之间的转换。
/// </summary>
internal static class KnownTypeConverter
{
    private static readonly Dictionary<Type, (Func<object, JValue> serializer, Func<JValue, object> deserializer)> JValueConverters = new()
    {
        { typeof(IntPtr), (x => new JValue(((IntPtr) x).ToInt64()), x => new IntPtr(x.ToObject<long>())) },
    };

    internal static JToken? Convert(object? value, IIpcObjectSerializer serializer) => value switch
    {
        // null。
        null => JValue.CreateNull(),

        // dotnetCampus.Ipc 支持的类型。
        IntPtr @intPtr => new JValue(intPtr.ToInt64()),

        // 默认类型。
        _ => JToken.FromObject(value),
    };

    internal static T? ConvertBackFromJTokenOrObject<T>(object? obj, IIpcObjectSerializer serializer)
    {
        if (obj is JToken jToken)
        {
            return ConvertBack<T>(jToken, serializer);
        }
        return (T?) obj;
    }

    private static T? ConvertBack<T>(JToken? jToken, IIpcObjectSerializer serializer) => JValueConverters.TryGetValue(typeof(T), out var converter)
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
#else
/// <summary>
/// 为了让对象能在 IPC 中传输，此类型提供 <see cref="JsonElement"/> 和对象之间的转换。
/// </summary>
internal static class KnownTypeConverter
{
    internal static JsonElement? Convert(object? value, IIpcObjectSerializer serializer)
    {
        return serializer.SerializeToElement(value);
    }

    internal static T? ConvertBackFromJTokenOrObject<T>(object? obj, IIpcObjectSerializer serializer)
    {
        if (obj is JsonElement jsonElement)
        {
            return ConvertBack<T>(jsonElement, serializer);
        }
        return (T?) obj;
    }

    private static T? ConvertBack<T>(JsonElement? jsonElement, IIpcObjectSerializer serializer)
    {
        if (jsonElement is not { } element)
        {
            return default!;
        }

        try
        {
            return serializer.Deserialize<T>(element);
        }
        catch (JsonException e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
#endif
