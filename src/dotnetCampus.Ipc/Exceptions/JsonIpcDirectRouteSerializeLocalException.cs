using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 直接路由进行序列化过程中的异常，这是一个比较模糊边界的异常，虽然是因为本地序列化抛出的异常，但也可能是远端发送过来的数据不符合预期导致的，归属于远端异常也是可以的
/// </summary>
public class JsonIpcDirectRouteSerializeLocalException : JsonIpcDirectRoutedLocalException
{
    internal JsonIpcDirectRouteSerializeLocalException(IpcMessage responseMessage, Type responseType, Exception innerException) : base(DefaultMessage, innerException)
    {
        ResponseMessage = responseMessage;
        ResponseType = responseType;
    }

    /// <summary>
    /// 响应消息
    /// </summary>
    public IpcMessage ResponseMessage { get; }

    /// <summary>
    /// 要求的响应类型
    /// </summary>
    public Type ResponseType { get; }

    internal const string DefaultMessage = "Json Ipc DirectRoute Serialize Exception.";

    /// <inheritdoc />
    public override string Message => $"{base.Message} ResponseType={ResponseType} ResponseMessage={ResponseMessage.ToDebugString()} {InnerException?.Message}";
}
