namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 表示在 JsonIpcDirectRouted 使用的无参类型
/// </summary>
internal class JsonIpcDirectRoutedParameterlessType
{
    /// <summary>
    /// 无参类型的实例，使用具体的类型而不是 null 是为了序列化时能够返回 `{}` 内容，从而提升兼容性
    /// 比如客户端是旧版本，旧版本时约定是无参。但是服务端是新版本，新版本时约定是有参。此时的服务端依然能够收到旧客户端发送过来的消息，且消息参数不为空，只是里面没有内容。如果服务端强行约束参数，则可以收到异常
    /// </summary>
    public static JsonIpcDirectRoutedParameterlessType Instance
    {
        get => _instance ??= new JsonIpcDirectRoutedParameterlessType();
    }

    private static JsonIpcDirectRoutedParameterlessType? _instance;
}
