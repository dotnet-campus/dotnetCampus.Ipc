#if NET6_0_OR_GREATER
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;

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

public class JsonIpcDirectRoutedProvider
{
    public JsonIpcDirectRoutedProvider(string? pipeName = null, IpcConfiguration? ipcConfiguration = null)
    {
        pipeName ??= $"JsonIpcDirectRouted_{Guid.NewGuid().ToString("N")}";
        var ipcProvider = new IpcProvider(pipeName, ipcConfiguration);
        IpcProvider = ipcProvider;
    }

    public JsonIpcDirectRoutedProvider(IpcProvider ipcProvider)
    {
        IpcProvider = ipcProvider;
    }

    public void StartServer()
    {        
        IpcProvider.StartServer();
        IpcProvider.IpcServerService.MessageReceived += IpcServerService_MessageReceived;
        var requestHandler = new RequestHandler(this);
        IpcProvider.IpcContext.IpcConfiguration.AddFrameworkRequestHandlers(requestHandler);
    }

    private void IpcServerService_MessageReceived(object? sender, PeerMessageArgs e)
    {
        if (e.Handle)
        {
            return;
        }

        // 这里是全部的消息都会进入的，但是这里通过判断业务头，只处理 Json 的
        if (TryHandleMessage(e.Message, out var stream, out var routedPath))
        {
            // 接下来进行调度


            e.SetHandle("JsonIpcDirectRouted Handled in MessageReceived");
        }
    }

    private static bool TryHandleMessage(in IpcMessage ipcMessage, [NotNullWhen(true)] out MemoryStream? stream, [NotNullWhen(true)] out string? routedPath)
    {
        const ulong header = (ulong) KnownMessageHeaders.JsonIpcDirectRoutedMessageHeader;
        if (ipcMessage.TryGetPayload(header, out var message))
        {
            stream = new(message.Body.Buffer, message.Body.Start, message.Body.Length);
            using BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
            routedPath = binaryReader.ReadString();
            return true;
        }
        stream = default;
        routedPath = default;
        return false;
    }

    private IpcProvider IpcProvider { get; }
    private JsonSerializer JsonSerializer => _jsonSerializer ??= JsonSerializer.CreateDefault();
    private JsonSerializer? _jsonSerializer;

    public void AddNotifyHandler<T>(string routedPath, Action<T> handler)
    {
        ThrowIfStarted();

        Action<MemoryStream> notifyHandler = (MemoryStream stream) =>
        {
            using StreamReader reader = new StreamReader(stream, leaveOpen: true);
            JsonReader jsonReader = new JsonTextReader(reader);
            var argument = JsonSerializer.Deserialize<T>(jsonReader);
            handler(argument);
        };


    }

    private void ThrowIfStarted()
    {
        if (IpcProvider.IsStarted)
        {
            throw new InvalidOperationException($"禁止在启动之后再次添加处理");
        }
    }

    delegate void HandleNotify(MemoryStream stream);

    class RequestHandler : IIpcRequestHandler
    {
        public RequestHandler(JsonIpcDirectRoutedProvider jsonIpcDirectRoutedProvider)
        {
            JsonIpcDirectRoutedProvider = jsonIpcDirectRoutedProvider;
        }
        private JsonIpcDirectRoutedProvider JsonIpcDirectRoutedProvider { get; }

        public async Task<IIpcResponseMessage> HandleRequest(IIpcRequestContext requestContext)
        {
            if (JsonIpcDirectRoutedProvider.TryHandleMessage(requestContext.IpcBufferMessage, out var stream, out var routedPath))
            {
                using (stream)
                {

                };
            }

            return KnownIpcResponseMessages.CannotHandle;
        }
    }
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
#endif
