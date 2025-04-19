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
}
