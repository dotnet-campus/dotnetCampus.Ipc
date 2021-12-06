using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models
{
    internal static class KnownTypeConverter
    {
        private static readonly Dictionary<Type, (Func<object, JValue> serializer, Func<JValue, object> deserializer)> KnownTypeConverters = new()
        {
            {
                typeof(IntPtr),
                (x => new JValue(((IntPtr) x).ToInt64()), x => new IntPtr(x.ToObject<long>()))
            },
        };

        internal static JValue Convert(object? value)
        {
            if (value == null)
            {
                return JValue.CreateNull();
            }

            var type = value.GetType();
            if (KnownTypeConverters.TryGetValue(type, out var vt))
            {
                var serializer = vt.serializer;
                return serializer(value!);
            }
            return new JValue(value);
        }

        internal static T ConvertBack<T>(JValue jValue)
        {
            if (KnownTypeConverters.TryGetValue(typeof(T), out var vt))
            {
                var deserializer = vt.deserializer;
                return (T) deserializer(jValue);
            }
            return jValue.ToObject<T>()!;
        }
    }
}
