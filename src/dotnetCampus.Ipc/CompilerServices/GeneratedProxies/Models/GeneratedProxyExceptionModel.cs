using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;
using dotnetCampus.Ipc.Exceptions;

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
#if NET6_0_OR_GREATER
                    exception = ExceptionDispatchInfo.SetRemoteStackTrace(exception, stackTrace);
#else
                    exception = ExceptionHacker.SetRemoteStackTrace(exception, stackTrace);
#endif
                }
                ExceptionDispatchInfo.Capture(exception).Throw();
            }
            else
            {
                throw new IpcInvokingException(
                    $"远端抛出了异常 {typeName}: {Message}\n如需抛出普通异常，请联系 IPC 库作者将异常添加到 ExceptionRebuilders 中。",
                    StackTrace);
            }
        }

        throw new InvalidOperationException("无法抛出远端对象的异常，因为无法获知异常的类型。");
    }

    private static readonly Dictionary<string, Func<string?, string?, Exception>> ExceptionRebuilders = new(StringComparer.Ordinal)
    {
        { typeof(ArgumentException).FullName!, (m, _) => new ArgumentException(m) },
        { typeof(ArgumentNullException).FullName!, (m, e) => new ArgumentNullException(e, m) },
        { typeof(BadImageFormatException).FullName!, (m, _) => new BadImageFormatException(m) },
        { typeof(InvalidCastException).FullName!, (m, _) => new InvalidCastException(m) },
        { typeof(InvalidOperationException).FullName!, (m, _) => new InvalidOperationException(m) },
        { typeof(NotImplementedException).FullName!, (m, _) => new NotImplementedException(m) },
        { typeof(NotSupportedException).FullName!, (m, _) => new NotSupportedException(m) },
        { typeof(NullReferenceException).FullName!, (m, _) => new NullReferenceException(m) },
    };
}

#if NET6_0_OR_GREATER
#else
file static class ExceptionHacker
{
    static ExceptionHacker()
    {
        HackExceptionStackTraceLazy = new Lazy<Func<Exception, string, Exception>>(() => HackExceptionStackTrace, LazyThreadSafetyMode.None);
    }

    internal static Exception SetRemoteStackTrace(Exception source, string stackTrace)
    {
        try
        {
            HackExceptionStackTraceLazy.Value(source, stackTrace);
        }
        catch
        {
            // 新框架已经处理了，不管旧框架的死活。
        }
        return source;
    }

    private static readonly Lazy<Func<Exception, string, Exception>> HackExceptionStackTraceLazy;

    private static readonly Func<Exception, string, Exception> HackExceptionStackTrace = new Func<Func<Exception, string, Exception>>(() =>
    {
        var source = Expression.Parameter(typeof(Exception));
        var stackTrace = Expression.Parameter(typeof(string));
        // Try to get _remoteStackTraceString first, then fallback to _stackTraceString for older .NET Frameworks
        var fieldInfo = typeof(Exception).GetField("_remoteStackTraceString", BindingFlags.NonPublic | BindingFlags.Instance)
                        ?? typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance);
        if (fieldInfo == null)
        {
            // If neither field exists, just return the exception unchanged
            return (ex, st) => ex;
        }
        var assign = Expression.Assign(Expression.Field(source, fieldInfo), stackTrace);
        return Expression.Lambda<Func<Exception, string, Exception>>(Expression.Block(assign, source), source, stackTrace).Compile();
    })();
}
#endif
