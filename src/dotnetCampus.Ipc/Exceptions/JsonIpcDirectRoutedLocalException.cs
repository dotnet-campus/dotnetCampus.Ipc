using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 直接路由里面的由本地问题导致的异常
/// </summary>
public class JsonIpcDirectRoutedLocalException : IpcLocalException
{
    internal JsonIpcDirectRoutedLocalException()
    {
    }

    internal JsonIpcDirectRoutedLocalException(string message) : base(message)
    {
    }

    internal JsonIpcDirectRoutedLocalException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
