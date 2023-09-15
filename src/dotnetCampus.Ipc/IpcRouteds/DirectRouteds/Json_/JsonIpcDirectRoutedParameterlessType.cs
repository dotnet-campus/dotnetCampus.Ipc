namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 表示在 JsonIpcDirectRouted 使用的无参类型
/// </summary>
internal class JsonIpcDirectRoutedParameterlessType
{
    public static JsonIpcDirectRoutedParameterlessType Instance
    {
        get => _instance ??= new JsonIpcDirectRoutedParameterlessType();
    }

    private static JsonIpcDirectRoutedParameterlessType? _instance;
}
