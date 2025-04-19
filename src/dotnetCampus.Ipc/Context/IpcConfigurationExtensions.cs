using dotnetCampus.Ipc.Serialization;
using Newtonsoft.Json;

namespace dotnetCampus.Ipc.Context;

public static class IpcConfigurationExtensions
{
    public static IpcConfiguration UseNewtonsoftJsonIpcObjectSerializer(this IpcConfiguration configuration, JsonSerializer? jsonSerializer)
    {
        if (jsonSerializer is null)
        {
            configuration.IpcObjectSerializer = IpcConfiguration.DefaultNewtonsoftJsonSerializer;
        }
        else
        {
            configuration.IpcObjectSerializer = new IpcObjectJsonSerializer(jsonSerializer);
        }

        return configuration;
    }

#if NET6_0_OR_GREATER
    public static IpcConfiguration UseSystemJsonIpcObjectSerializer(this IpcConfiguration configuration,
        System.Text.Json.Serialization.JsonSerializerContext context)
    {
        configuration.IpcObjectSerializer = new IpcObjectSystemJsonSerializer(context);
        return configuration;
    }
#endif
}
