namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 提供 Json 直接路由的 IPC 通讯的上下文
/// </summary>
public class JsonIpcDirectRoutedContext
{
    /// <summary>
    /// 创建提供 Json 直接路由的 IPC 通讯的上下文
    /// </summary>
    /// <param name="peerName"></param>
    public JsonIpcDirectRoutedContext(string peerName)
    {
        PeerName = peerName;
    }

    /// <summary>
    /// 通讯方的 Peer 名
    /// </summary>
    public string PeerName { get; }
}
