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

    public static EventId ReceiveJsonIpcDirectRoutedNotifyEventId => new EventId(3, "ReceiveJsonIpcDirectRoutedNotify");

    public static EventId ReceiveJsonIpcDirectRoutedRequestEventId => new EventId(4, "ReceiveJsonIpcDirectRoutedRequest");
}
