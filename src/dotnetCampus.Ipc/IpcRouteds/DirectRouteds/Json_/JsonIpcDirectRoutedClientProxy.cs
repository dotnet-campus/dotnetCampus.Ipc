using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Serialization;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 提供 Json 直接路由的 IPC 通讯的客户端
/// </summary>
public class JsonIpcDirectRoutedClientProxy : IpcDirectRoutedClientProxyBase
{
    /// <summary>
    /// 创建 Json 直接路由的 IPC 通讯的客户端
    /// </summary>
    public JsonIpcDirectRoutedClientProxy(PeerProxy peerProxy)
    {
        _peerProxy = peerProxy;
    }

    private readonly PeerProxy _peerProxy;
    private IpcContext IpcContext => _peerProxy.IpcContext;
    private IIpcObjectSerializer IpcObjectSerializer => IpcContext.IpcConfiguration.IpcObjectSerializer;

    /// <summary>
    /// 不带参数的通知服务端
    /// </summary>
    /// <param name="routedPath"></param>
    /// <returns></returns>
    public Task NotifyAsync(string routedPath)
        => NotifyAsync(routedPath, JsonIpcDirectRoutedParameterlessType.Instance);

    /// <summary>
    /// 通知服务端
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="routedPath">路由地址</param>
    /// <param name="obj"></param>
    /// <returns>返回仅代表消息到达服务端，不代表服务端业务上处理完成</returns>
    public Task NotifyAsync<T>(string routedPath, T obj) where T : class
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, obj);
        IpcContext.LogSendJsonIpcDirectRoutedNotify(routedPath, _peerProxy.PeerName, ipcMessage.Body);
        return _peerProxy.NotifyAsync(ipcMessage);
    }

    /// <summary>
    /// 获取服务端的响应，不带参数
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath">路由地址</param>
    /// <returns></returns>
    public Task<TResponse?> GetResponseAsync<TResponse>(string routedPath) where TResponse : class
        => GetResponseAsync<TResponse>(routedPath, JsonIpcDirectRoutedParameterlessType.Instance);

    /// <summary>
    /// 获取服务端的响应
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath">路由地址</param>
    /// <param name="obj">发送给服务端的对象</param>
    /// <returns></returns>
    public async Task<TResponse?> GetResponseAsync<TResponse>(string routedPath, object obj) where TResponse : class
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, obj);
        IpcContext.LogSendJsonIpcDirectRoutedRequest(routedPath, _peerProxy.PeerName, ipcMessage.Body);

        IpcMessage responseMessage = await _peerProxy.GetResponseAsync(ipcMessage);

        using var memoryStream = responseMessage.Body.ToMemoryStream();
        IpcContext.LogReceiveJsonIpcDirectRoutedResponse(routedPath, _peerProxy.PeerName, memoryStream);

        try
        {
            // 反序列化响应。正常情况下，会需要反序列化两次
            // 第一次，获取是否存在异常信息
            // 第二次，读取真正的响应数据
            var exceptionResponse =
                IpcObjectSerializer.Deserialize<JsonIpcDirectRoutedHandleRequestExceptionResponse>(memoryStream);

            // 是否为没有找到处理器的情况
            if (JsonIpcDirectRoutedCanNotFindRequestHandlerExceptionInfo
                .IsCanNotFindRequestHandlerException(exceptionResponse))
            {
                throw new JsonIpcDirectRoutedCanNotFindRequestHandlerException(_peerProxy, routedPath,
                    exceptionResponse);
            }

            // 是否为存在异常的情况，判断方式就是判断是否存在异常类型。因为异常类型在框架内是必然有异常就会赋值的
            bool existsException = !string.IsNullOrEmpty(exceptionResponse?.ExceptionInfo?.ExceptionType);
            if (existsException)
            {
                Debug.Assert(exceptionResponse != null);
                Debug.Assert(exceptionResponse!.ExceptionInfo != null);
                // 远端异常，准备构建异常信息抛出
                throw new JsonIpcDirectRoutedHandleRequestRemoteException(_peerProxy, routedPath, exceptionResponse);
            }
            else
            {
                // 没异常，正常处理
                // 重新设置内存流的位置，前面反序列化异常信息时已经读取了内存流
                memoryStream.Position = 0;
            }

            return IpcObjectSerializer.Deserialize<TResponse>(memoryStream);
        }
        catch (IpcException)
        {
            // 自己框架内抛出的异常，那就原封不动继续抛出
            throw;
        }
        catch (Exception e)
        {
            // 序列化错误
            throw new JsonIpcDirectRouteSerializeLocalException(responseMessage, typeof(TResponse), e);
        }
    }

    private IpcMessage BuildMessage(string routedPath, object obj)
    {
        using var memoryStream = new MemoryStream();
        WriteHeader(memoryStream, routedPath);

        IpcObjectSerializer.Serialize(memoryStream, obj);

        return ToIpcMessage(memoryStream, $"Message {routedPath}");
    }

    /// <inheritdoc />
    protected override ulong BusinessHeader => (ulong) KnownMessageHeaders.JsonIpcDirectRoutedMessageHeader;
}
