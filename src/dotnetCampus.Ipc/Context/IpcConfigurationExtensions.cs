using dotnetCampus.Ipc.Serialization;
#if UseNewtonsoftJson
using Newtonsoft.Json;
#endif

namespace dotnetCampus.Ipc.Context;

/// <summary>
/// IPC 配置扩展
/// </summary>
public static class IpcConfigurationExtensions
{
#if UseNewtonsoftJson
    /// <summary>
    /// 使用 Newtonsoft.Json 作为 IPC 对象序列化器
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="jsonSerializer"></param>
    /// <returns></returns>
    public static IpcConfiguration UseNewtonsoftJsonIpcObjectSerializer(this IpcConfiguration configuration, JsonSerializer? jsonSerializer)
    {
        if (jsonSerializer is null)
        {
            configuration.IpcObjectSerializer = IpcConfiguration.DefaultNewtonsoftJsonSerializer;
        }
        else
        {
            configuration.IpcObjectSerializer = new NewtonsoftJsonIpcObjectSerializer(jsonSerializer);
        }

        return configuration;
    }
#endif

#if NET6_0_OR_GREATER
    /// <summary>
    /// 使用 System.Text.Json 作为 IPC 对象序列化器
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IpcConfiguration UseSystemTextJsonIpcObjectSerializer(this IpcConfiguration configuration,
        System.Text.Json.Serialization.JsonSerializerContext context)
    {
        configuration.IpcObjectSerializer = new SystemTextJsonIpcObjectSerializer(context);
        return configuration;
    }
#endif
}
