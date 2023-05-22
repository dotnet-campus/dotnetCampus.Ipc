#if NET6_0_OR_GREATER
using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Utils.Extensions;
using dotnetCampus.Ipc.Utils.Logging;

namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

public abstract class IpcDirectRoutedProviderBase
{
    protected IpcDirectRoutedProviderBase(string? pipeName = null, IpcConfiguration? ipcConfiguration = null)
    {
        pipeName ??= $"RawByteIpcDirectRouted_{Guid.NewGuid():N}";
        var ipcProvider = new IpcProvider(pipeName, ipcConfiguration);
        IpcProvider = ipcProvider;
    }

    protected IpcDirectRoutedProviderBase(IpcProvider ipcProvider)
    {
        IpcProvider = ipcProvider;
    }

    public IpcProvider IpcProvider { get; }
    private protected ILogger Logger => IpcProvider.IpcContext.Logger;
    private bool _isStarted;

    /// <summary>
    /// 启动服务。启动服务之后将不能再添加通知处理和请求处理
    /// </summary>
    public void StartServer()
    {
        if (IpcProvider.IsStarted)
        {
            // 如果在当前框架启动之前，已经启动了 IPC 服务，那就记录一条调试信息
            Logger.Debug($"[{nameof(JsonIpcDirectRoutedProvider)}][StartServer] 在 JsonIpcDirectRouted 框架启动服务之前，传入的 {nameof(IpcProvider)} 已经启动。可能启动的 {nameof(IpcProvider)} 已在接收消息，接收掉的消息将不会被 JsonIpcDirectRouted 框架处理。可能丢失消息");
        }

        // 处理请求消息
        var requestHandler = new RequestHandler(this);
        IpcProvider.IpcContext.IpcConfiguration.AddFrameworkRequestHandlers(requestHandler);

        IpcProvider.StartServer();
        // 处理 Notify 消息
        IpcProvider.IpcServerService.MessageReceived += IpcServerService_MessageReceived;

        _isStarted = true;
    }

    protected void ThrowIfStarted()
    {
        if (_isStarted)
        {
            throw new InvalidOperationException($"禁止在启动之后再次添加处理");
        }
    }

    protected abstract ulong BusinessHeader { get; }

    private void IpcServerService_MessageReceived(object? sender, PeerMessageArgs e)
    {
        if (e.Handle)
        {
            return;
        }

        // 这里是全部的消息都会进入的，但是这里通过判断业务头，只处理感兴趣的
        if (TryHandleMessage(e.Message, out var message))
        {
            // 这是一个 MemoryStream 释放或不释放都没啥差别
            //using (stream) ;
            try
            {
                OnHandleNotify(message.Value, e);
            }
            catch (Exception exception)
            {
                // 不能让这里的异常对外抛出，否则其他业务也许莫名不执行
                Logger.Error(exception, $"[{nameof(IpcDirectRoutedProviderBase)}] HandleNotify");
            }
        }
    }

    protected abstract void OnHandleNotify(IpcDirectRoutedMessage message, PeerMessageArgs e);

    class RequestHandler : IIpcRequestHandler
    {
        public RequestHandler(IpcDirectRoutedProviderBase ipcDirectRoutedProviderBase)
        {
            IpcDirectRoutedProviderBase = ipcDirectRoutedProviderBase;
        }

        private IpcDirectRoutedProviderBase IpcDirectRoutedProviderBase { get; }

        public Task<IIpcResponseMessage> HandleRequest(IIpcRequestContext requestContext)
        {
            if (IpcDirectRoutedProviderBase.TryHandleMessage(requestContext.IpcBufferMessage, out var message))
            {
                try
                {
                    return IpcDirectRoutedProviderBase.OnHandleRequestAsync(message.Value, requestContext);
                }
                catch (Exception e)
                {
                    IpcDirectRoutedProviderBase.Logger.Error(e, $"[{nameof(IpcDirectRoutedProviderBase)}] HandleRequest");
                }
            }

            return Task.FromResult(KnownIpcResponseMessages.CannotHandle);
        }
    }

    protected abstract Task<IIpcResponseMessage> OnHandleRequestAsync(IpcDirectRoutedMessage message, IIpcRequestContext requestContext);

    private bool TryHandleMessage(in IpcMessage ipcMessage, [NotNullWhen(true)] out IpcDirectRoutedMessage? ipcDirectRoutedMessage)
    {
        try
        {
            if (ipcMessage.TryGetPayload(BusinessHeader, out var message))
            {
                var stream = new MemoryStream(message.Body.Buffer, message.Body.Start, message.Body.Length, writable: true, publiclyVisible: true);
                using BinaryReader binaryReader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true);
                var routedPath = binaryReader.ReadString();
                ipcDirectRoutedMessage = new IpcDirectRoutedMessage(routedPath, stream, message);
                return true;
            }
        }
        catch (Exception e)
        {
            Logger.Error(e, $"HandleMessage");
        }

        ipcDirectRoutedMessage = default;
        return false;
    }

    protected readonly record struct IpcDirectRoutedMessage(string RoutedPath, MemoryStream Stream,
        IpcMessage PayloadIpcMessage)
    {
        public IpcMessageBody GetData()
        {
            var position = (int) Stream.Position;
            var payload = PayloadIpcMessage.Body;
            var data = new IpcMessageBody(payload.Buffer, payload.Start + position, payload.Length - position);
            return data;
        }
    }
}

#endif
