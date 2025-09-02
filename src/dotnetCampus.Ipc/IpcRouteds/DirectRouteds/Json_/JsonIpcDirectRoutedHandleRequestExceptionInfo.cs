namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 直接路由的 IPC 通讯的异常信息，即 IPC 服务端的业务层通讯异常。如参数不匹配、执行的业务端逻辑抛出异常等
/// </summary>
internal class JsonIpcDirectRoutedHandleRequestExceptionResponse
{
#if NET6_0_OR_GREATER
    [System.Text.Json.Serialization.JsonPropertyName("__$Exception")]
#endif
    public JsonIpcDirectRoutedHandleRequestExceptionInfo? ExceptionInfo { get; set; }

    internal class JsonIpcDirectRoutedHandleRequestExceptionInfo
    {
        public string? ExceptionType { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? ExceptionStackTrace { get; set; }

        public string? ExceptionToString { get; set; }
    }
}
