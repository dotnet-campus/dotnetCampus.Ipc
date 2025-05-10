using Newtonsoft.Json;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

internal class JsonIpcDirectRoutedCanNotFindRequestHandlerResponse
{
    [JsonProperty("__$CanNotFindHandler")]
#if NET6_0_OR_GREATER
    [System.Text.Json.Serialization.JsonPropertyName("__$CanNotFindHandler")]
#endif
    public RequestHandlerCanNotFindExceptionInfo? ExceptionInfo { get; set; }

    internal class RequestHandlerCanNotFindExceptionInfo
    {
        public string? RoutedPath { get; set; }
    }
}
