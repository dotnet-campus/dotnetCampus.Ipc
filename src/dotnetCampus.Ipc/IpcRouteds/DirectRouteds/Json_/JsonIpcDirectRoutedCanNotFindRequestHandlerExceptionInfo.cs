using System.Diagnostics.CodeAnalysis;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

internal static class JsonIpcDirectRoutedCanNotFindRequestHandlerExceptionInfo
{
    public static JsonIpcDirectRoutedHandleRequestExceptionResponse CreateExceptionResponse(string routedPath)
    {
        return new JsonIpcDirectRoutedHandleRequestExceptionResponse()
        {
            ExceptionInfo = new JsonIpcDirectRoutedHandleRequestExceptionResponse.JsonIpcDirectRoutedHandleRequestExceptionInfo()
            {
                ExceptionMessage = $"Can not find '{routedPath}' request Handler. 找不到 '{routedPath}' 请求的处理器，请确保服务端已经及时注册该路由处理器，也可能是客户端服务端版本不匹配问题",
                ExceptionType = ExceptionType,
            }
        };
    }

    public static bool IsCanNotFindRequestHandlerException([NotNullWhen(true)] JsonIpcDirectRoutedHandleRequestExceptionResponse? response)
    {
        return string.Equals(ExceptionType, response?.ExceptionInfo?.ExceptionType, StringComparison.Ordinal);
    }

    // 异常类型做好标识，确保可以唯一识别。不使用 nameof 的原因是，确保后续如果有改动，还能做好兼容
    private const string ExceptionType = "__JsonIpcDirectRoutedCanNotFindRequestHandlerException__";
}
