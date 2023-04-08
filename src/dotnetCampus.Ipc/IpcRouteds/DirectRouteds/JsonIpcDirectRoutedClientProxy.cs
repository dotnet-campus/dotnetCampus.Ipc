#if NET6_0_OR_GREATER
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

using Newtonsoft.Json;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 传输裸的 byte 数据的直接路由的 IPC 通讯客户端
/// </summary>
public class RawByteIpcDirectRoutedClientProxy: IpcDirectRoutedClientProxyBase
{
    public RawByteIpcDirectRoutedClientProxy(IPeerProxy peerProxy)
    {
        _peerProxy = peerProxy;
    }

    private readonly IPeerProxy _peerProxy;

    public Task NotfiyAsync(string routedPath, Span<byte> data)
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, data);
        return _peerProxy.NotifyAsync(ipcMessage);
    }

    public async Task<IpcMessageBody> GetResponseAsync(string routedPath, IpcMessageBody ipcMessageBody)
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, ipcMessageBody.AsSpan());

        var response = await _peerProxy.GetResponseAsync(ipcMessage);
        return response.Body;
    }

    private IpcMessage BuildMessage(string routedPath, Span<byte> data)
    {
        using var memoryStream = new MemoryStream();
        WriteHeader(memoryStream, routedPath);

        memoryStream.Write(data);

        return ToIpcMessage(memoryStream, $"Message To {routedPath}");
    }

    protected override ulong BusinessHeader => (ulong) KnownMessageHeaders.RawByteIpcDirectRoutedMessageHeader;
}

public abstract class IpcDirectRoutedClientProxyBase
{
    protected abstract ulong BusinessHeader { get; }

    protected void WriteHeader(MemoryStream stream, string routedPath)
    {
        using var binaryWriter = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        IpcDirectRoutedMessageWriter.WriteHeader(binaryWriter, BusinessHeader, routedPath);
    }

    protected IpcMessage ToIpcMessage(MemoryStream stream, string tag = "")
    {
        var buffer = stream.GetBuffer();
        var length = (int) stream.Position;

        return new IpcMessage(tag, new IpcMessageBody(buffer, start: 0, length));
    }
}

public class JsonIpcDirectRoutedClientProxy : IpcDirectRoutedClientProxyBase
{
    public JsonIpcDirectRoutedClientProxy(IPeerProxy peerProxy)
    {
        _peerProxy = peerProxy;
    }

    private readonly IPeerProxy _peerProxy;
    private JsonSerializer? _jsonSerializer;
    private JsonSerializer JsonSerializer => _jsonSerializer ??= JsonSerializer.CreateDefault();

    public Task NotifyAsync<T>(string routedPath, T obj) where T : class
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, obj);
        return _peerProxy.NotifyAsync(ipcMessage);
    }

    public async Task<TResponse?> GetResponseAsync<TResponse>(string routedPath, object obj) where TResponse : class
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, obj);
        var responseMessage = await _peerProxy.GetResponseAsync(ipcMessage);

        using var memoryStream = new MemoryStream(responseMessage.Body.Buffer, responseMessage.Body.Start, responseMessage.Body.Length, writable: false);
        using StreamReader reader = new StreamReader(memoryStream, leaveOpen: true);
        JsonReader jsonReader = new JsonTextReader(reader);
        return JsonSerializer.Deserialize<TResponse>(jsonReader);
    }

    private IpcMessage BuildMessage(string routedPath, object obj)
    {
        using var memoryStream = new MemoryStream();
        WriteHeader(memoryStream, routedPath);

        using (var textWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        {
            JsonSerializer.Serialize(textWriter, obj);
        }

        return ToIpcMessage(memoryStream, $"Message {routedPath}");
    }

    protected override ulong BusinessHeader => (ulong) KnownMessageHeaders.JsonIpcDirectRoutedMessageHeader;
}
#endif
