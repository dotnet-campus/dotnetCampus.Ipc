using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.Context.LoggingContext;

internal static class LoggerEventIds
{
    /// <summary>
    /// 发送消息
    /// </summary>
    public static EventId SendMessageEventId => new EventId(1, "SendMessage");

    /// <summary>
    /// 收到框架的消息，裸消息，包括消息头
    /// </summary>
    public static EventId ReceiveOriginMessageEventId => new EventId(2, "ReceiveOriginMessage");

    /// <summary>
    /// 收到消息，业务消息
    /// </summary>
    public static EventId ReceiveMessageEventId => new EventId(2, "ReceiveMessage");

    /// <summary>
    /// 收到 JsonIpcDirectRouted 通知消息
    /// </summary>
    public static EventId ReceiveJsonIpcDirectRoutedNotifyEventId => new EventId(3, "ReceiveJsonIpcDirectRoutedNotify");

    /// <summary>
    /// 收到 JsonIpcDirectRouted 请求消息
    /// </summary>
    public static EventId ReceiveJsonIpcDirectRoutedRequestEventId => new EventId(4, "ReceiveJsonIpcDirectRoutedRequest");

    /// <summary>
    /// 发送 JsonIpcDirectRouted 响应消息
    /// </summary>
    /// 在 <see cref="ReceiveJsonIpcDirectRoutedRequestEventId"/> 之后返回的响应
    public static EventId SendJsonIpcDirectRoutedResponseEventId => new EventId(5, "SendJsonIpcDirectRoutedResponse");

    /// <summary>
    /// 发送 JsonIpcDirectRouted 通知消息
    /// </summary>
    public static EventId SendJsonIpcDirectRoutedNotifyEventId => new EventId(6, "SendJsonIpcDirectRoutedNotify");

    /// <summary>
    /// 发送 JsonIpcDirectRouted 请求消息
    /// </summary>
    public static EventId SendJsonIpcDirectRoutedRequestEventId => new EventId(7, "SendJsonIpcDirectRoutedRequest");

    /// <summary>
    /// 收到 JsonIpcDirectRouted 响应消息
    /// </summary>
    /// 在客户端发送 <see cref="SendJsonIpcDirectRoutedRequestEventId"/> 之后收到服务端的响应
    public static EventId ReceiveJsonIpcDirectRoutedResponseEventId => new EventId(8, "SendJsonIpcDirectRoutedRequest");
}
