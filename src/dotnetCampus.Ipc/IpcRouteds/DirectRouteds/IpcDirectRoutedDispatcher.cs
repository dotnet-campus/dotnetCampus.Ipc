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

class JsonIpcDirectRoutedClientProxy
{
    public static Task NotifyAsync<T>(IPeerProxy peerProxy, string routedPath, T obj) where T : class
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, obj);
        return peerProxy.NotifyAsync(ipcMessage);
    }

    public static async Task<TResponse?> GetResponseAsync<TResponse>(IPeerProxy peerProxy, string routedPath, object obj) where TResponse : class
    {
        IpcMessage ipcMessage = BuildMessage(routedPath, obj);
        var responseMessage = await peerProxy.GetResponseAsync(ipcMessage);
        var jsonSerializer = JsonSerializer.CreateDefault();
        using var memoryStream = new MemoryStream(responseMessage.Body.Buffer, responseMessage.Body.Start, responseMessage.Body.Length, writable: false);
        using StreamReader reader = new StreamReader(memoryStream, leaveOpen: true);
        JsonReader jsonReader = new JsonTextReader(reader);
        return jsonSerializer.Deserialize<TResponse>(jsonReader);
    }

    private static IpcMessage BuildMessage(string routedPath, object obj)
    {
        using var memoryStream = new MemoryStream();
        var jsonSerializer = JsonSerializer.CreateDefault();
        using (var binaryWriter = new BinaryWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        {
            IpcDirectRoutedMessageCreator.WriteHeader(binaryWriter, (ulong) KnownMessageHeaders.JsonIpcDirectRoutedMessageHeader, routedPath);
        }

        using (var textWriter = new StreamWriter(memoryStream, Encoding.UTF8, leaveOpen: true))
        {
            jsonSerializer.Serialize(textWriter, obj);
        }

        var buffer = memoryStream.GetBuffer();
        var length = (int) memoryStream.Position;

        return new IpcMessage($"Notify {routedPath}", new IpcMessageBody(buffer, start: 0, length));
    }
}

internal class IpcDirectRoutedDispatcher
{

}
