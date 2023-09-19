using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using Newtonsoft.Json;

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
    private JsonSerializer? _jsonSerializer;
    private JsonSerializer JsonSerializer => _jsonSerializer ??= JsonSerializer.CreateDefault();

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

        var responseMessage = await _peerProxy.GetResponseAsync(ipcMessage);

        using var memoryStream = responseMessage.Body.ToMemoryStream();
        IpcContext.LogReceiveJsonIpcDirectRoutedResponse(routedPath, _peerProxy.PeerName, memoryStream);

        using StreamReader reader = new StreamReader
        (
            memoryStream,
#if !NETCOREAPP
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: false,
            bufferSize: 2048,
#endif
            leaveOpen: true
        );
        JsonReader jsonReader = new JsonTextReader(reader);
        return JsonSerializer.Deserialize<TResponse>(jsonReader);
    }

    private IpcMessage BuildMessage(string routedPath, object obj)
    {
        using var memoryStream = new MemoryStream();
        WriteHeader(memoryStream, routedPath);

        using (var textWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true, bufferSize: 2048))
        {
            JsonSerializer.Serialize(textWriter, obj);
        }

        return ToIpcMessage(memoryStream, $"Message {routedPath}");
    }

    /// <inheritdoc />
    protected override ulong BusinessHeader => (ulong) KnownMessageHeaders.JsonIpcDirectRoutedMessageHeader;
}
