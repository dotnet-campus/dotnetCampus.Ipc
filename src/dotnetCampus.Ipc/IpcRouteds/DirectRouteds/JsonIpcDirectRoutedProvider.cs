#if NET6_0_OR_GREATER
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;

using Newtonsoft.Json;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

public class JsonIpcDirectRoutedProvider
{
    public JsonIpcDirectRoutedProvider(string? pipeName = null, IpcConfiguration? ipcConfiguration = null)
    {
        pipeName ??= $"JsonIpcDirectRouted_{Guid.NewGuid():N}";
        var ipcProvider = new IpcProvider(pipeName, ipcConfiguration);
        IpcProvider = ipcProvider;
    }

    public JsonIpcDirectRoutedProvider(IpcProvider ipcProvider)
    {
        IpcProvider = ipcProvider;
    }

    public void StartServer()
    {
        // 处理请求消息
        var requestHandler = new RequestHandler(this);
        IpcProvider.IpcContext.IpcConfiguration.AddFrameworkRequestHandlers(requestHandler);

        IpcProvider.StartServer();
        // 处理 Notify 消息
        IpcProvider.IpcServerService.MessageReceived += IpcServerService_MessageReceived;
    }

    /// <summary>
    /// 获取对 <see cref="serverPeerName"/> 请求的客户端
    /// </summary>
    /// <param name="serverPeerName"></param>
    /// <returns></returns>
    public async Task<JsonIpcDirectRoutedClientProxy> GetAndConnectClientAsync(string serverPeerName)
    {
        var peer = await IpcProvider.GetAndConnectToPeerAsync(serverPeerName);
        return new JsonIpcDirectRoutedClientProxy(peer);
    }

    #region Notify

    public void AddNotifyHandler<T>(string routedPath, Func<T, Task> handler)
    {
        AddNotifyHandler<T>(routedPath, (args, _) => handler(args));
    }

    public void AddNotifyHandler<T>(string routedPath, Func<T, JsonIpcDirectRoutedContext, Task> handler)
    {
        // ReSharper disable once AsyncVoidLambda
        // ReSharper disable once ConvertToLocalFunction
        Action<T, JsonIpcDirectRoutedContext> notifyHandler = async (argument, context) =>
        {
            try
            {
                await handler(argument, context);
            }
            catch (Exception e)
            {
                // 后台顶层，抛出就没了哦
                IpcProvider.IpcContext.Logger.Warning($"Handle {routedPath} with exception. {e}");
            }
        };
        AddNotifyHandler(routedPath, notifyHandler);
    }

    public void AddNotifyHandler<T>(string routedPath, Action<T> handler)
    {
        AddNotifyHandler<T>(routedPath, (args, _) => handler(args));
    }

    public void AddNotifyHandler<T>(string routedPath, Action<T, JsonIpcDirectRoutedContext> handler)
    {
        ThrowIfStarted();

        if (!HandleNotifyDictionary.TryAdd(routedPath, NotifyHandler))
        {
            throw new InvalidOperationException($"重复添加对 {routedPath} 的处理");
        }

        void NotifyHandler(MemoryStream stream, JsonIpcDirectRoutedContext context)
        {
            var argument = ToObject<T>(stream);
            handler(argument!, context);
        }
    }

    private delegate void HandleNotify(MemoryStream stream, JsonIpcDirectRoutedContext context);

    private ConcurrentDictionary<string, HandleNotify> HandleNotifyDictionary { get; } = new ConcurrentDictionary<string, HandleNotify>();

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
            if (HandleNotifyDictionary.TryGetValue(routedPath, out var handleNotify))
            {
                var context = new JsonIpcDirectRoutedContext(e.PeerName);
                handleNotify(stream, context);
            }
            else
            {
                IpcProvider.IpcContext.Logger.Warning($"找不到对 {routedPath} 的 {nameof(JsonIpcDirectRoutedProvider)} 处理，是否忘记调用 {nameof(AddNotifyHandler)} 添加处理");
            }

            e.SetHandle("JsonIpcDirectRouted Handled in MessageReceived");
        }
    }

    #endregion

    #region Request Response

    public void AddRequestHandler<TRequest, TResponse>(string routedPath, Func<TRequest, TResponse> handler)
    {
        AddRequestHandler<TRequest, TResponse>(routedPath, (request, _) =>
        {
            var response = handler(request);
            return Task.FromResult(response);
        });
    }

    public void AddRequestHandler<TRequest, TResponse>(string routedPath, Func<TRequest, Task<TResponse>> handler)
    {
        AddRequestHandler<TRequest, TResponse>(routedPath, (request, _) => handler(request));
    }

    public void AddRequestHandler<TRequest, TResponse>(string routedPath,
        Func<TRequest, JsonIpcDirectRoutedContext, Task<TResponse>> handler)
    {
        ThrowIfStarted();
        HandleRequest handleRequest = HandleRequest;

        if (!HandleRequestDictionary.TryAdd(routedPath, handleRequest))
        {
            throw new InvalidOperationException($"重复添加对 {routedPath} 的处理");
        }

        async ValueTask<IpcMessage> HandleRequest(MemoryStream stream, JsonIpcDirectRoutedContext context)
        {
            var argument = ToObject<TRequest>(stream);
            var response = await handler(argument!, context);
            var responseMemoryStream = new MemoryStream();
            using (TextWriter textWriter = new StreamWriter(responseMemoryStream, Encoding.UTF8, leaveOpen: true))
            {
                JsonSerializer.Serialize(textWriter, response);
            }

            var buffer = responseMemoryStream.GetBuffer();
            var length = (int) responseMemoryStream.Position;
            return new IpcMessage($"Handle {routedPath} response", new IpcMessageBody(buffer, 0, length));
        }
    }

    private delegate ValueTask<IpcMessage> HandleRequest(MemoryStream stream, JsonIpcDirectRoutedContext context);

    private ConcurrentDictionary<string, HandleRequest> HandleRequestDictionary { get; } =
        new ConcurrentDictionary<string, HandleRequest>();

    class RequestHandler : IIpcRequestHandler
    {
        public RequestHandler(JsonIpcDirectRoutedProvider jsonIpcDirectRoutedProvider)
        {
            JsonIpcDirectRoutedProvider = jsonIpcDirectRoutedProvider;
        }
        private JsonIpcDirectRoutedProvider JsonIpcDirectRoutedProvider { get; }

        async Task<IIpcResponseMessage> IIpcRequestHandler.HandleRequest(IIpcRequestContext requestContext)
        {
            if (JsonIpcDirectRoutedProvider.TryHandleMessage(requestContext.IpcBufferMessage, out var stream, out var routedPath))
            {
                using (stream)
                {
                    if (JsonIpcDirectRoutedProvider.HandleRequestDictionary.TryGetValue(routedPath, out var handler))
                    {
                        var context = new JsonIpcDirectRoutedContext(requestContext.Peer.PeerName);
                        var ipcMessage = await handler(stream, context);

                        IIpcResponseMessage response = new IpcHandleRequestMessageResult(ipcMessage);
                        return response;
                    }
                    else
                    {
                        JsonIpcDirectRoutedProvider.IpcProvider.IpcContext.Logger.Warning($"找不到对 {routedPath} 的 {nameof(JsonIpcDirectRoutedProvider)} 处理，是否忘记调用 {nameof(AddRequestHandler)} 添加处理");
                        return KnownIpcResponseMessages.CannotHandle;
                    }
                };
            }

            return KnownIpcResponseMessages.CannotHandle;
        }
    }

    #endregion

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

    private void ThrowIfStarted()
    {
        if (IpcProvider.IsStarted)
        {
            throw new InvalidOperationException($"禁止在启动之后再次添加处理");
        }
    }

    private T? ToObject<T>(MemoryStream stream)
    {
        using StreamReader reader = new StreamReader(stream, leaveOpen: true);
        JsonReader jsonReader = new JsonTextReader(reader);
        return JsonSerializer.Deserialize<T>(jsonReader);
    }
}

#endif
