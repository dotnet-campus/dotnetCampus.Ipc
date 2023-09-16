namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 提供 Json 直接路由的 IPC 通讯的日志消息类型
/// </summary>
public enum JsonIpcDirectRoutedLogStateMessageType
{
    // 服务端
    /// <summary>
    /// 收到通知
    /// </summary>
    ReceiveNotify,

    /// <summary>
    /// 收到请求
    /// </summary>
    ReceiveRequest,

    /// <summary>
    /// 发送响应
    /// </summary>
    SendResponse,

    // 客户端
    /// <summary>
    /// 发送通知
    /// </summary>
    SendNotify,

    /// <summary>
    /// 发送请求
    /// </summary>
    SendRequest,

    /// <summary>
    /// 收到响应
    /// </summary>
    ReceiveResponse,
}
