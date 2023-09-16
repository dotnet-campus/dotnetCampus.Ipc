namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

public enum JsonIpcDirectRoutedLogStateMessageType
{
    // 服务端
    ReceiveNotify,
    ReceiveRequest,
    SendResponse,

    // 客户端
    SendNotify,
    SendRequest,
    ReceiveResponse,
}
