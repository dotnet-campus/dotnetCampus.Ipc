using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Utils;

#if UseNewtonsoftJson
using Newtonsoft.Json;
#endif
#if NET6_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

[DataContract]
internal class GeneratedProxyExceptionModel
{
    public GeneratedProxyExceptionModel()
    {
    }

    public GeneratedProxyExceptionModel(Exception exception)
    {
        ExceptionType = exception.GetType().FullName;
        Message = exception.Message;
        StackTrace = exception.StackTrace;
        ExtraInfo = null;
    }

#if UseNewtonsoftJson
    [JsonProperty("t")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("t")]
#endif
    public string? ExceptionType { get; set; }

#if UseNewtonsoftJson
    [JsonProperty("m")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("m")]
#endif
    public string? Message { get; set; }

#if UseNewtonsoftJson
    [JsonProperty("s")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("s")]
#endif
    public string? StackTrace { get; set; }

#if UseNewtonsoftJson
    [JsonProperty("x")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("x")]
#endif
    public string? ExtraInfo { get; set; }

    public void Throw()
    {
        if (ExceptionType is { } typeName)
        {
            if (ExceptionRebuilders.TryGetValue(typeName, out var builder))
            {
                var exception = builder(Message, ExtraInfo);
                if (StackTrace is { } stackTrace)
                {
                    var deserializedRemoteException = ExceptionHacker.ReplaceStackTrace(exception, StackTrace);
                    ExceptionDispatchInfo.Capture(deserializedRemoteException).Throw();
                }
                else
                {
                    ExceptionDispatchInfo.Capture(exception).Throw();
                }
            }
            else
            {
                throw new IpcRemoteException($"不支持的远端异常类型 {typeName}。如果是忘加了，请添加到 ExceptionRebuilders 属性中，否则请抛 IPC 自定义的异常。", StackTrace);
            }
        }

        throw new InvalidOperationException("无法抛出远端对象的异常，因为无法获知异常的类型。");
    }

    private static readonly Dictionary<string, Func<string?, string?, Exception>> ExceptionRebuilders = new(StringComparer.Ordinal)
    {
        { typeof(ArgumentException).FullName!, (m, e) => new ArgumentException(m) },
        { typeof(ArgumentNullException).FullName!, (m, e) => new ArgumentNullException(e, m) },
        { typeof(BadImageFormatException).FullName!, (m, e) => new BadImageFormatException(m) },
        { typeof(InvalidCastException).FullName!, (m, e) => new InvalidCastException(m) },
        { typeof(InvalidOperationException).FullName!, (m, e) => new InvalidOperationException(m) },
        { typeof(NotImplementedException).FullName!, (m, e) => new NotImplementedException(m) },
        { typeof(NullReferenceException).FullName!, (m, e) => new NullReferenceException(m) },
    };
}
