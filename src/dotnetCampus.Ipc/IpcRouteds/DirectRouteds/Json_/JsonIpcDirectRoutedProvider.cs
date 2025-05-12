using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Serialization;
using dotnetCampus.Ipc.Utils.Extensions;
using dotnetCampus.Ipc.Utils.Logging;

#if !NETCOREAPP
using ValueTask = System.Threading.Tasks.Task;
#endif

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

/// <summary>
/// 提供 Json 直接路由的 IPC 通讯
/// </summary>
public class JsonIpcDirectRoutedProvider : IpcDirectRoutedProviderBase
{
    /// <summary>
    /// 创建 Json 直接路由的 IPC 通讯
    /// </summary>
    /// <param name="pipeName"></param>
    /// <param name="ipcConfiguration"></param>
    public JsonIpcDirectRoutedProvider(string? pipeName = null, IpcConfiguration? ipcConfiguration = null) : base(pipeName, ipcConfiguration)
    {
    }

    /// <summary>
    /// 创建 Json 直接路由的 IPC 通讯
    /// </summary>
    /// <param name="ipcProvider"></param>
    public JsonIpcDirectRoutedProvider(IpcProvider ipcProvider) : base(ipcProvider)
    {
    }

    /// <summary>
    /// 获取对 <paramref name="serverPeerName"/> 请求的客户端
    /// </summary>
    /// <param name="serverPeerName"></param>
    /// <returns></returns>
    public async Task<JsonIpcDirectRoutedClientProxy> GetAndConnectClientAsync(string serverPeerName)
    {
        var peer = await IpcProvider.GetAndConnectToPeerAsync(serverPeerName);
        return new JsonIpcDirectRoutedClientProxy(peer);
    }

    #region Notify

    /// <summary>
    /// 添加通知的处理，无参
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler(string routedPath, Action handler)
        => AddNotifyHandler<JsonIpcDirectRoutedParameterlessType>(routedPath, _ => handler());

    /// <summary>
    /// 添加通知的处理，无参
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler(string routedPath, Func<Task> handler)
        => AddNotifyHandler<JsonIpcDirectRoutedParameterlessType>(routedPath, _ => handler());

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler<T>(string routedPath, Func<T, Task> handler)
    {
        AddNotifyHandler<T>(routedPath, (args, _) => handler(args));
    }

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler<T>(string routedPath, Func<T, JsonIpcDirectRoutedContext, Task> handler)
    {
        Task HandleNotify(MemoryStream stream, JsonIpcDirectRoutedContext context)
        {
            var argument = ToObject<T>(stream);
            return handler(argument!, context);
        }

        AddNotifyHandler(routedPath, new NotifyHandler() { AsyncHandler = HandleNotify });
    }

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler<T>(string routedPath, Action<T> handler)
    {
        AddNotifyHandler<T>(routedPath, (args, _) => handler(args));
    }

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler<T>(string routedPath, Action<T, JsonIpcDirectRoutedContext> handler)
    {
        void HandleNotify(MemoryStream stream, JsonIpcDirectRoutedContext context)
        {
            var argument = ToObject<T>(stream);
            handler(argument!, context);
        }

        AddNotifyHandler(routedPath, new NotifyHandler() { SyncHandler = HandleNotify });
    }

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="notifyHandler"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void AddNotifyHandler(string routedPath, NotifyHandler notifyHandler)
    {
        ThrowIfStarted();

        if (!HandleNotifyDictionary.TryAdd(routedPath, notifyHandler))
        {
            throw new InvalidOperationException($"重复添加对 {routedPath} 的处理");
        }
    }

    private class NotifyHandler
    {
        public Func<MemoryStream, JsonIpcDirectRoutedContext, Task>? AsyncHandler { get; set; }
        public Action<MemoryStream, JsonIpcDirectRoutedContext>? SyncHandler { get; set; }
    }


    private ConcurrentDictionary<string, NotifyHandler> HandleNotifyDictionary { get; } = new ConcurrentDictionary<string, NotifyHandler>();

    /// <inheritdoc />
    protected override ulong BusinessHeader => (ulong) KnownMessageHeaders.JsonIpcDirectRoutedMessageHeader;

    /// <inheritdoc />
    protected override void OnHandleNotify(IpcDirectRoutedMessage message, PeerMessageArgs e)
    {
        // 接下来进行调度
        var routedPath = message.RoutedPath;
        var stream = message.Stream;
        if (HandleNotifyDictionary.TryGetValue(routedPath, out var handleNotify))
        {
            var context = new JsonIpcDirectRoutedContext(e.PeerName);
            e.SetHandle("JsonIpcDirectRouted Handled in MessageReceived");

            IpcProvider.IpcContext.LogReceiveJsonIpcDirectRoutedNotify(routedPath, e.PeerName, stream);

            try
            {
                // 分为同步和异步两个版本，防止异步版本执行过程没有等待，导致原本期望顺序执行的业务变成了并发执行
                if (handleNotify.SyncHandler is { } syncHandler)
                {
                    // 不等了，也没啥业务
                    _ = IpcProvider.IpcContext.TaskPool.Run(() =>
                    {
                        syncHandler(stream, context);
                    });
                }
                else if (handleNotify.AsyncHandler is { } asyncHandler)
                {
                    // 不等了，也没啥业务
                    _ = IpcProvider.IpcContext.TaskPool.Run(async () =>
                    {
                        await asyncHandler(stream, context);
                        return true;
                    });
                }
                else
                {
                    // 不能吧，如果进入此分支则表示框架里面的代码有问题
                    Logger.Error($"[{nameof(JsonIpcDirectRoutedProvider)}] HandleNotify not found any handler. IPC Library internal error! 框架内部异常");
                }
            }
            catch (Exception exception)
            {
                // 不能让这里的异常对外抛出，否则其他业务也许莫名不执行
                Logger.Error(exception, $"[{nameof(JsonIpcDirectRoutedProvider)}] HandleNotify Method={handleNotify.SyncHandler?.Method ?? handleNotify.AsyncHandler?.Method}");
            }
        }
        else
        {
            // 考虑可能有多个实例，每个实例处理不同的业务情况
            //IpcProvider.IpcContext.Logger.Warning($"找不到对 {routedPath} 的 {nameof(JsonIpcDirectRoutedProvider)} 处理，是否忘记调用 {nameof(AddNotifyHandler)} 添加处理");
        }
    }

    #endregion

    #region Request Response

    /// <summary>
    /// 添加请求的处理，无参请求
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler<TResponse>(string routedPath, Func<TResponse> handler) =>
        AddRequestHandler<JsonIpcDirectRoutedParameterlessType, TResponse>(routedPath, _ => handler());

    /// <summary>
    /// 添加请求的处理，无参请求
    /// </summary>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler<TResponse>(string routedPath, Func<Task<TResponse>> handler) =>
        AddRequestHandler<JsonIpcDirectRoutedParameterlessType, TResponse>(routedPath, _ => handler());

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler<TRequest, TResponse>(string routedPath, Func<TRequest, TResponse> handler)
    {
        AddRequestHandler<TRequest, TResponse>(routedPath, (request, _) =>
        {
            var response = handler(request);
            return Task.FromResult(response);
        });
    }

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler<TRequest, TResponse>(string routedPath, Func<TRequest, Task<TResponse>> handler)
    {
        AddRequestHandler<TRequest, TResponse>(routedPath, (request, _) => handler(request));
    }

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler<TRequest, TResponse>(string routedPath,
        Func<TRequest, JsonIpcDirectRoutedContext, TResponse> handler)
    {
        AddRequestHandler<TRequest, TResponse>(routedPath, (request, context) =>
        {
            var result = handler(request, context);
            return Task.FromResult(result);
        });
    }

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddRequestHandler<TRequest, TResponse>(string routedPath,
        Func<TRequest, JsonIpcDirectRoutedContext, Task<TResponse>> handler)
    {
        ThrowIfStarted();
        HandleRequest handleRequest = HandleRequest;

        if (!HandleRequestDictionary.TryAdd(routedPath, handleRequest))
        {
            throw new InvalidOperationException($"重复添加对 {routedPath} 的处理");
        }

#if NETCOREAPP
        async ValueTask<IpcMessage> HandleRequest(MemoryStream stream, JsonIpcDirectRoutedContext context)
#else
        async Task<IpcMessage> HandleRequest(MemoryStream stream, JsonIpcDirectRoutedContext context)
#endif
        {
            var argument = ToObject<TRequest>(stream);
            // 业务端处理参数，返回响应内容
            var response = await handler(argument!, context);

            IpcMessageBody ipcMessageBody = ResponseToIpcMessageBody(response);
            return new IpcMessage($"Handle '{routedPath}' response", ipcMessageBody);
        }
    }

#if NETCOREAPP
    private delegate ValueTask<IpcMessage> HandleRequest(MemoryStream stream, JsonIpcDirectRoutedContext context);
#else
    private delegate Task<IpcMessage> HandleRequest(MemoryStream stream, JsonIpcDirectRoutedContext context);
#endif

    private ConcurrentDictionary<string, HandleRequest> HandleRequestDictionary { get; } =
        new ConcurrentDictionary<string, HandleRequest>();

    /// <summary>
    /// 处理请求的核心逻辑
    /// </summary>
    /// <param name="message"></param>
    /// <param name="requestContext"></param>
    /// <returns></returns>
    protected override async Task<IIpcResponseMessage> OnHandleRequestAsync(IpcDirectRoutedMessage message, IIpcRequestContext requestContext)
    {
        var routedPath = message.RoutedPath;
        var stream = message.Stream;

        if (HandleRequestDictionary.TryGetValue(routedPath, out var handler))
        {
            var context = new JsonIpcDirectRoutedContext(requestContext.Peer.PeerName);
            var taskPool = IpcProvider.IpcContext.TaskPool;

            IpcProvider.IpcContext.LogReceiveJsonIpcDirectRoutedRequest(routedPath, requestContext.Peer.PeerName,
                stream);

            try
            {
                var ipcMessage = await taskPool.Run(async () =>
                {
                    return await handler(stream, context);
                });

                IpcProvider.IpcContext.LogSendJsonIpcDirectRoutedResponse(routedPath, requestContext.Peer.PeerName, ipcMessage.Body);
                IIpcResponseMessage response = new IpcHandleRequestMessageResult(ipcMessage);
                return response;
            }
            catch (Exception exception)
            {
                // 由于 handler 是业务端传过来的，在框架层需要接住异常，否则 IPC 框架将会因为某个业务抛出异常然后丢失消息
                Logger.Error(exception, $"[{nameof(JsonIpcDirectRoutedProvider)}] HandleRequest Method={handler.Method} RoutedPath={routedPath}");
                // 经过实际业务大量测试，不会有因为错误调度而导致进入非预期分支的异常，能够进入到此异常的，都是业务端报错。更加正确的处理是将其返回给到另一端 Peer 端，返回时，给出异常信息，这样才能更好的方便上层业务调试
                // 经过实际业务大量测试发现，很多时候服务端的业务执行逻辑抛出了异常，但客户端业务层没有收到，开发者经常都会第一时间尝试调查客户端业务层，结果毫无所获。为了更好的调试体验，决定在此出现异常时，包装为 IPC 远端异常数据返回。当前只有 JsonIpc 方式能够进行包装，暂不知道 RawByte 等方式可以进行如何封装

                //// 也有可能是错误处理了不应该调度到这里的业务处理的消息从而抛出异常，继续调度到下一项
                //return KnownIpcResponseMessages.CannotHandle;

                var response = new JsonIpcDirectRoutedHandleRequestExceptionResponse()
                {
                    ExceptionInfo = new JsonIpcDirectRoutedHandleRequestExceptionResponse.JsonIpcDirectRoutedHandleRequestExceptionInfo()
                    {
                        ExceptionType = exception.GetType().FullName,
                        ExceptionMessage = exception.Message,
                        ExceptionStackTrace = exception.StackTrace,
                        // 在 ToString 过程里面会包含 Inner 异常等的信息，一般靠这个就足够了
                        ExceptionToString = exception.ToString(),
                    }
                };

                IpcMessageBody ipcMessageBody = ResponseToIpcMessageBody(response);
                var ipcMessage = new IpcMessage($"Handle '{routedPath}' response exception", ipcMessageBody);
                return new IpcHandleRequestMessageResult(ipcMessage);
            }
        }
        else
        {
            // 尽管说客户端能够推测出 RoutedPath 是什么，但这个类型仅仅是为了以后方便扩展
            // 和让输出有东西，方便抓取信息了解内容
            var response = JsonIpcDirectRoutedCanNotFindRequestHandlerExceptionInfo.CreateExceptionResponse(routedPath);

            IpcMessageBody ipcMessageBody = ResponseToIpcMessageBody(response);
            var ipcMessage = new IpcMessage($"Can not find '{routedPath}' request Handler", ipcMessageBody);

            // 考虑可能有多个实例，每个实例处理不同的业务情况
            //JsonIpcDirectRoutedProvider.IpcProvider.IpcContext.Logger.Warning($"找不到对 {routedPath} 的 {nameof(JsonIpcDirectRoutedProvider)} 处理，是否忘记调用 {nameof(AddRequestHandler)} 添加处理");
            // 返回不能处理即可。最后一次返回不能处理则会将此信息传递给到客户端。如果后续还有其他的实例处理了请求信息，则能够覆盖本条信息。正常来说不会有非常多个实例，即使有所浪费，预计也不多
            return KnownIpcResponseMessages.CreateCanNotHandleResponseMessage(ipcMessage);
        }
    }

    private IpcMessageBody ResponseToIpcMessageBody<TResponse>(TResponse response)
    {
        var responseMemoryStream = new MemoryStream();

        JsonSerializer.Serialize(responseMemoryStream, response);

        var buffer = responseMemoryStream.GetBuffer();
        var length = (int) responseMemoryStream.Position;
        return new IpcMessageBody(buffer, 0, length);
    }

    #endregion

    private IIpcObjectSerializer JsonSerializer => IpcProvider.IpcContext.IpcConfiguration.IpcObjectSerializer;

    private T? ToObject<T>(MemoryStream stream)
    {
        return JsonSerializer.Deserialize<T>(stream);
    }
}
