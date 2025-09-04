using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

namespace dotnetCampus.Ipc.Exceptions;

/// <summary>
/// 当获取设置属性值或调用方法时，如果实现方法内抛出了异常，且不是常见的异常类型（参见 <see cref="GeneratedProxyExceptionModel"/>），则会用此异常包装。
/// </summary>
internal class IpcInvokingException(string message, string? remoteStackTrace) : IpcRemoteException(message, remoteStackTrace);
