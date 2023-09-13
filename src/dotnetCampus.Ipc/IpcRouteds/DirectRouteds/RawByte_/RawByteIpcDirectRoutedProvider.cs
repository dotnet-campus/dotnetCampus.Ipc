using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

public class RawByteIpcDirectRoutedProvider : IpcDirectRoutedProviderBase
{
    public RawByteIpcDirectRoutedProvider(string? pipeName = null, IpcConfiguration? ipcConfiguration = null) : base(
        pipeName, ipcConfiguration)
    {
    }

    public RawByteIpcDirectRoutedProvider(IpcProvider ipcProvider) : base(ipcProvider)
    {
    }

    protected override ulong BusinessHeader => (ulong) KnownMessageHeaders.RawByteIpcDirectRoutedMessageHeader;

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler(string routedPath, Func<IpcMessageBody, Task> handler) =>
        AddNotifyHandler(routedPath, (data, _) => handler(data));

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler(string routedPath, Func<IpcMessageBody, JsonIpcDirectRoutedContext, Task> handler)
    {
        AddNotifyHandler(routedPath, HandlerNotify);

        async void HandlerNotify(IpcMessageBody data, JsonIpcDirectRoutedContext context)
        {
            try
            {
                await handler(data, context);
            }
            catch (Exception e)
            {
                // 线程顶层，不能再抛出异常
                Logger.Warning($"Handle {routedPath} with exception. {e}");
            }
        }
    }

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddNotifyHandler(string routedPath, Action<IpcMessageBody> handler) =>
        AddNotifyHandler(routedPath, (data, _) => handler(data));

    /// <summary>
    /// 添加通知的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    /// <exception cref="InvalidOperationException"></exception>
    public void AddNotifyHandler(string routedPath, Action<IpcMessageBody, JsonIpcDirectRoutedContext> handler)
    {
        ThrowIfStarted();

        if (!HandleNotifyDictionary.TryAdd(routedPath, HandleNotify))
        {
            throw new InvalidOperationException($"重复添加对 {routedPath} 的处理");
        }

        void HandleNotify(IpcMessageBody data, JsonIpcDirectRoutedContext context)
        {
            handler(data, context);
        }
    }

    private ConcurrentDictionary<string, HandleNotify> HandleNotifyDictionary { get; } =
        new ConcurrentDictionary<string, HandleNotify>();

    private delegate void HandleNotify(IpcMessageBody data, JsonIpcDirectRoutedContext context);

    protected override void OnHandleNotify(IpcDirectRoutedMessage message, PeerMessageArgs e)
    {
        var routedPath = message.RoutedPath;

        if (HandleNotifyDictionary.TryGetValue(routedPath, out var handleNotify))
        {
            var data = message.GetData();

            var context = new JsonIpcDirectRoutedContext(e.PeerName);
            e.SetHandle("RawByteIpcDirectRouted Handled in MessageReceived");

            try
            {
                // 不等了，也没啥业务
                _ = IpcProvider.IpcContext.TaskPool.Run(() =>
                {
                    handleNotify(data, context);
                });
            }
            catch (Exception exception)
            {
                // 不能让这里的异常对外抛出，否则其他业务也许莫名不执行
                Logger.Error(exception,
                    $"[{nameof(RawByteIpcDirectRoutedProvider)}] HandleNotify Method={handleNotify.Method}");
            }
        }
        else
        {
            // 考虑可能有多个实例，每个实例处理不同的业务情况
        }
    }

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler(string routedPath,
        Func<IpcMessageBody, IpcMessage> handler)
        => AddRequestHandler(routedPath, (data, _) => handler(data));

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler(string routedPath,
        Func<IpcMessageBody, JsonIpcDirectRoutedContext, IpcMessage> handler)
        => AddRequestHandler(routedPath, (data, context) => Task.FromResult(handler(data, context)));

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler(string routedPath, Func<IpcMessageBody, Task<IpcMessage>> handler)
        => AddRequestHandler(routedPath, (data, _) => handler(data));

    /// <summary>
    /// 添加请求的处理
    /// </summary>
    /// <param name="routedPath"></param>
    /// <param name="handler"></param>
    public void AddRequestHandler(string routedPath,
        Func<IpcMessageBody, JsonIpcDirectRoutedContext, Task<IpcMessage>> handler)
    {
        ThrowIfStarted();

        if (!HandleRequestDictionary.TryAdd(routedPath, HandleRequest))
        {
            throw new InvalidOperationException($"重复添加对 {routedPath} 的处理");
        }

        Task<IpcMessage> HandleRequest(IpcMessageBody data, JsonIpcDirectRoutedContext context)
        {
            return handler(data, context);
        }
    }

    private delegate Task<IpcMessage> HandleRequest(IpcMessageBody data, JsonIpcDirectRoutedContext context);

    private ConcurrentDictionary<string, HandleRequest> HandleRequestDictionary { get; } =
        new ConcurrentDictionary<string, HandleRequest>();

    protected override async Task<IIpcResponseMessage> OnHandleRequestAsync(IpcDirectRoutedMessage message,
        IIpcRequestContext requestContext)
    {
        var routedPath = message.RoutedPath;

        if (HandleRequestDictionary.TryGetValue(routedPath, out var handler))
        {
            var context = new JsonIpcDirectRoutedContext(requestContext.Peer.PeerName);
            var taskPool = IpcProvider.IpcContext.TaskPool;

            var data = message.GetData();

            try
            {
                var ipcMessage = await taskPool.Run(async () =>
                {
                    return await handler(data, context);
                });

                IIpcResponseMessage response = new IpcHandleRequestMessageResult(ipcMessage);
                return response;
            }
            catch (Exception exception)
            {
                // 由于 handler 是业务端传过来的，在框架层需要接住异常，否则 IPC 框架将会因为某个业务抛出异常然后丢失消息
                Logger.Error(exception,
                    $"[{nameof(RawByteIpcDirectRoutedProvider)}] HandleNotify Method={handler.Method}");
                // 也有可能是错误处理了不应该调度到这里的业务处理的消息从而抛出异常，继续调度到下一项
                return KnownIpcResponseMessages.CannotHandle;
            }
        }
        else
        {
            // 考虑可能有多个实例，每个实例处理不同的业务情况
            return KnownIpcResponseMessages.CannotHandle;
        }
    }
}
