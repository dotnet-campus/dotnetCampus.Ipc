using System;
using System.IO;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 传输裸的 byte 数据的直接路由的 IPC 通讯客户端
/// </summary>
public class RawByteIpcDirectRoutedClientProxy : IpcDirectRoutedClientProxyBase
{
    public RawByteIpcDirectRoutedClientProxy(IPeerProxy peerProxy)
    {
        _peerProxy = peerProxy;
    }

    private readonly IPeerProxy _peerProxy;

#if NETCOREAPP

    public Task NotfiyAsync(string routedPath, in IpcMessageBody data)
        => NotfiyAsync(routedPath, data.AsSpan());

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
#else
    public Task NotfiyAsync(string routedPath, IpcMessageBody data)
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, data);
        return _peerProxy.NotifyAsync(ipcMessage);
    }

    public async Task<IpcMessageBody> GetResponseAsync(string routedPath, IpcMessageBody ipcMessageBody)
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, ipcMessageBody);

        var response = await _peerProxy.GetResponseAsync(ipcMessage);
        return response.Body;
    }

    private IpcMessage BuildMessage(string routedPath, in IpcMessageBody data)
    {
        using var memoryStream = new MemoryStream();
        WriteHeader(memoryStream, routedPath);

        memoryStream.Write(data.Buffer, data.Start, data.Length);

        return ToIpcMessage(memoryStream, $"Message To {routedPath}");
    }
#endif
    protected override ulong BusinessHeader => (ulong) KnownMessageHeaders.RawByteIpcDirectRoutedMessageHeader;
}
