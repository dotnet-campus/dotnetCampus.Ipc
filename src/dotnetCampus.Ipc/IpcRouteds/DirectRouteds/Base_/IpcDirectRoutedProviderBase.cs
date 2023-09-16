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

/// <summary>
/// 直接路由的 IPC 通讯
/// </summary>
public abstract class IpcDirectRoutedProviderBase
{
    /// <summary>
    /// 直接路由的 IPC 通讯
    /// </summary>
    /// <param name="pipeName">管道名，也是当前服务的 PeerName 名。不填将会随机生成</param>
    /// <param name="ipcConfiguration"></param>
    protected IpcDirectRoutedProviderBase(string? pipeName = null, IpcConfiguration? ipcConfiguration = null)
    {
        pipeName ??= $"IpcDirectRouted_{Guid.NewGuid():N}";
        var ipcProvider = new IpcProvider(pipeName, ipcConfiguration);
        IpcProvider = ipcProvider;
    }

    /// <summary>
    /// 直接路由的 IPC 通讯。使用已配置的 <see cref="IpcProvider"/> 对象，允许多个直接路由的 IPC 通讯共用相同的 <see cref="IpcProvider"/> 对象，共用一条管道
    /// </summary>
    /// <param name="ipcProvider">将使用此对象作为基础</param>
    protected IpcDirectRoutedProviderBase(IpcProvider ipcProvider)
    {
        IpcProvider = ipcProvider;
    }

    /// <summary>
    /// 实际发送和接收消息的 Ipc 提供器
    /// </summary>
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

    /// <summary>
    /// 如果已经启动，抛出异常
    /// </summary>
    /// <exception cref="InvalidOperationException"></exception>
    protected void ThrowIfStarted()
    {
        if (_isStarted)
        {
            throw new InvalidOperationException($"禁止在启动之后再次添加处理");
        }
    }

    /// <summary>
    /// 业务头，用来分多个不同的通讯方式
    /// </summary>
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

    /// <summary>
    /// 处理通知
    /// </summary>
    /// <param name="message"></param>
    /// <param name="e"></param>
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

    /// <summary>
    /// 处理请求
    /// </summary>
    /// <param name="message"></param>
    /// <param name="requestContext"></param>
    /// <returns></returns>
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

    /// <summary>
    /// 表示 IPC 直接路由消息
    /// </summary>
    protected readonly struct IpcDirectRoutedMessage
    {
        /// <summary>
        /// 创建 IPC 直接路由消息
        /// </summary>
        /// <param name="routedPath"></param>
        /// <param name="stream"></param>
        /// <param name="payloadIpcMessage">有效负载消息</param>
        public IpcDirectRoutedMessage(string routedPath, MemoryStream stream,
            IpcMessage payloadIpcMessage)
        {
           RoutedPath = routedPath;
           Stream = stream;
           PayloadIpcMessage = payloadIpcMessage;
        }

        /// <summary>
        /// 获取消息体
        /// </summary>
        /// <returns></returns>
        public IpcMessageBody GetData()
        {
            // 消息体为从有效负载信息里面，加上消息体本身读取掉的信息
            var position = (int) Stream.Position;
            var payload = PayloadIpcMessage.Body;
            var data = new IpcMessageBody(payload.Buffer, payload.Start + position, payload.Length - position);
            return data;
        }

        /// <summary>
        /// 路由地址
        /// </summary>
        public string RoutedPath { get; }

        /// <summary>
        /// 消息本身，从有效负载消息创建。如需获取消息体，请使用 <see cref="GetData"/> 方法
        /// </summary>
        public MemoryStream Stream { get; }

        /// <summary>
        /// 有效负载消息。包含路由头 <see cref="RoutedPath"/> 信息和消息体
        /// </summary>
        public IpcMessage PayloadIpcMessage { get; }
    }
}

