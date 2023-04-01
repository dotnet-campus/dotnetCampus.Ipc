using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

using Newtonsoft.Json;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

static class IpcDirectRoutedMessageCreator
{
    //public int GetHeaderByteCount(string routedPath)
    //{
    //}

    public static void WriteHeader(BinaryWriter writer, ulong businessMessageHeader, string routedPath)
    {
        writer.Write(businessMessageHeader);
        writer.Write(routedPath);
    }

    //public IpcMessage CreateNotifyMessage()
    //{
    //}
}



public class JsonIpcDirectRoutedClientProxy
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
        using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        {
            IpcDirectRoutedMessageCreator.WriteHeader(binaryWriter, (ulong) KnownMessageHeaders.JsonIpcDirectRoutedMessageHeader, routedPath);
        }

        using (var textWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        {
            JsonSerializer.Serialize(textWriter, obj);
        }

        var buffer = memoryStream.GetBuffer();
        var length = (int) memoryStream.Position;

        return new IpcMessage($"Notify {routedPath}", new IpcMessageBody(buffer, start: 0, length));
    }
}

internal class IpcDirectRoutedDispatcher
{

}
