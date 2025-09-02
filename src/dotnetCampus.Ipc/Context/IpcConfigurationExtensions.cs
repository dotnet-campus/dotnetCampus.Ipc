using dotnetCampus.Ipc.Serialization;

namespace dotnetCampus.Ipc.Context;

/// <summary>
/// IPC 配置扩展
/// </summary>
public static class IpcConfigurationExtensions
{
#if NET6_0_OR_GREATER
    /// <summary>
    /// 使用 System.Text.Json 作为 IPC 对象序列化器
    /// </summary>
    /// <param name="configuration"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public static IpcConfiguration UseSystemJsonIpcObjectSerializer(this IpcConfiguration configuration,
        System.Text.Json.Serialization.JsonSerializerContext context)
    {
        configuration.IpcObjectSerializer = new IpcObjectSystemJsonSerializer(context);
        return configuration;
    }
#endif
}
