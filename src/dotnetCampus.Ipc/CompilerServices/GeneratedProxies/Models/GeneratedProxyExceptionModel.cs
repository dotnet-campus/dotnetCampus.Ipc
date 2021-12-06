using System;
using System.Collections.Generic;
using System.Runtime.ExceptionServices;
using System.Runtime.Serialization;

using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Utils;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
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

        [DataMember(Name = "t")]
        public string? ExceptionType { get; set; }

        [DataMember(Name = "m")]
        public string? Message { get; set; }

        [DataMember(Name = "s")]
        public string? StackTrace { get; set; }

        [DataMember(Name = "x")]
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
            { typeof(InvalidCastException).FullName!, (m, e) => new InvalidCastException(m) },
            { typeof(NotImplementedException).FullName!, (m, e) => new NotImplementedException(m) },
            { typeof(NullReferenceException).FullName!, (m, e) => new NullReferenceException(m) },
            { typeof(BadImageFormatException).FullName!, (m, e) => new BadImageFormatException(m) },
        };
    }
}
