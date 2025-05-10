using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Diagnostics;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Internals;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils.Extensions;

namespace dotnetCampus.Ipc.Pipes
{
    /// <summary>
    /// 关联 <see cref="IpcMessageRequestManager"/> 和 <see cref="IIpcRequestHandler"/> 的联系
    /// </summary>
    class IpcRequestHandlerProvider
    {
        public IpcRequestHandlerProvider(IpcContext ipcContext)
        {
            IpcContext = ipcContext;
        }

        public IpcContext IpcContext { get; }

        /// <summary>
        /// 处理请求消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        /// 有三步
        /// 1. 取出消息和上下文里面带的 <see cref="IIpcRequestHandler"/> 用于处理消息
        /// 2. 构建出 <see cref="IIpcRequestContext"/> 传入到 <see cref="IIpcRequestHandler"/> 处理
        /// 3. 将 <see cref="IIpcRequestHandler"/> 的返回值发送给到客户端
        public async void HandleRequest(PeerProxy sender, IpcClientRequestArgs args)
        {
            try
            {
                var requestMessage = args.IpcMessageBody;
                var peerProxy = sender;

                IpcMessage ipcMessage = new IpcMessage($"[{peerProxy.PeerName}][{args.MessageId}]", requestMessage);
                var ipcRequestContext = new IpcRequestMessageContext(ipcMessage, peerProxy, args.MessageCommandType.ToCoreMessageType());

                // 处理消息
                // 优先从 Peer 里面找处理的方法，这样上层可以对某个特定的 Peer 做不同的处理
                // Todo 需要设计这部分 API 现在因为没有 API 的设计，先全部走 DefaultIpcRequestHandler 的逻辑
                var receiveRequestTracker = new IpcMessageTracker<IpcRequestMessageContext>(
                    peerProxy.IpcContext.PipeName,
                    peerProxy.PeerName,
                    ipcRequestContext,
                    "HandleRequest",
                    IpcContext.Logger);
                IIpcResponseMessage result;

                try
                {
                    result = await HandleRequestAsync(receiveRequestTracker, peerProxy.PeerName).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    IpcContext.Logger.Error(e, "业务端 HandleRequestAsync 抛出异常");
                    result = new IpcHandleRequestMessageResult(new IpcMessage("Error", new byte[0]));
                }

                // 构建信息回复，发送回客户端。
                // 由于这里是通用的回复逻辑，所以只对需要回复的业务进行回复（不需要回复的业务就直接忽略）。
                var responseManager = IpcContext.IpcMessageResponseManager;
                var responseMessage = responseManager.CreateResponseMessage(args.MessageId, result.ResponseMessage);
                var sendResponseTracker = new IpcMessageTracker<IpcBufferMessageContext>(
                    peerProxy.IpcContext.PipeName,
                    peerProxy.PeerName,
                    responseMessage,
                    "HandleRequest",
                    IpcContext.Logger);
                sendResponseTracker.CriticalStep("Send", null, requestMessage);
                await WriteResponseMessageAsync(peerProxy, sendResponseTracker).ConfigureAwait(false);
            }
            catch (Exception e)
            {
                // 当前是后台线程了，不能接受任何的抛出
                IpcContext.Logger.Error(e);
            }
        }

        private async Task<IIpcResponseMessage> HandleRequestAsync(IpcMessageTracker<IpcRequestMessageContext> context, string remotePeerName)
        {
            IIpcResponseMessage? result = null;

            context.CriticalStep("ReceiveCore", null, context.Message.IpcBufferMessage.Body);
            var handlers = IpcContext.IpcConfiguration.GetIpcRequestHandlers();
            foreach (var ipcRequestHandler in handlers)
            {
                result = await HandleRequestCoreAsync(context.Message, ipcRequestHandler).ConfigureAwait(false);
                if (result is null)
                {
                    var errorMessage = $"在实现 {nameof(IIpcRequestHandler)}.{nameof(IIpcRequestHandler.HandleRequest)} 时，必须返回非 null 的响应。如果不知道如何处理此消息，请返回 {nameof(KnownIpcResponseMessages)}.{nameof(KnownIpcResponseMessages.CannotHandle)}";
                    IpcContext.Logger.Error(errorMessage);
#if DEBUG
                    throw new InvalidOperationException(errorMessage);
#endif
                }

                // 如果非不能响应，则代表已经处理完了
                if (!KnownIpcResponseMessages.IsCanNotHandleResponseMessage(result))
                {
                    break;
                }
            }

            if (result == null || KnownIpcResponseMessages.IsCanNotHandleResponseMessage(result) || result.ResponseMessage.Body.Length <= 0)
            {
                var possibleMessageContent = Encoding.UTF8.GetString(context.Message.IpcBufferMessage.Body.Buffer, context.Message.IpcBufferMessage.Body.Start, context.Message.IpcBufferMessage.Body.Length);
                var errorMessage = $"IPC 端 {remotePeerName} 正在等待返回，因此必须至少有一个 IPC 处理器正常处理此消息返回。出现此异常代表代码编写出现了错误，必须修复。";
                // 重新再拿一次，防止枚举遍历不正确
                handlers = IpcContext.IpcConfiguration.GetIpcRequestHandlers();
                var errorHandlers = string.Join("\r\n", handlers.Select(FormatHandlerAsErrorMessage));
                var logMessage = $"{errorMessage}\r\n消息内容猜测为：\r\n{possibleMessageContent}\r\n消息处理器有：\r\n{errorHandlers}\r\n说明所有这些消息处理器都没有处理此条消息，请添加更多的消息处理器。";
                IpcContext.Logger.Error(logMessage);
                // 由于业务代码肯定引用的是 IPC 库，以下的 DEBUG 异常一定不会在业务代码中抛出，因此除了开发过程中提示 IPC 库的开发者外，生产环境中不会有什么作用。现在 IPC 库已经稳定，以下代码先注释，方便编写单元测试
                //#if DEBUG
                //                // 这里一定说明业务代码写错了，缺少对应的 Handler。
                //                throw new InvalidOperationException(logMessage);
                //#endif
            }

            return result ?? KnownIpcResponseMessages.CannotHandle;
        }

        private string FormatHandlerAsErrorMessage(IIpcRequestHandler handler) => handler switch
        {
            GeneratedProxyJointIpcRequestHandler gpjirh => string.Join(", ", gpjirh.Owner.JointManager.EnumerateJointNames()),
            DelegateIpcRequestHandler dirh => nameof(DelegateIpcRequestHandler),
            EmptyIpcRequestHandler eirh => nameof(EmptyIpcRequestHandler),
            null => "null",
            _ => handler.GetType().FullName!,
        };

        private static async Task<IIpcResponseMessage> HandleRequestCoreAsync(IpcRequestMessageContext message, IIpcRequestHandler ipcRequestHandler)
        {
            var result = await ipcRequestHandler.HandleRequest(message).ConfigureAwait(false);
            return result;
        }

        private async Task WriteResponseMessageAsync(PeerProxy peerProxy, IpcMessageTracker<IpcBufferMessageContext> sendResponseTracker)
        {
            try
            {
                await peerProxy.IpcClientService.WriteMessageAsync(sendResponseTracker).ConfigureAwait(false);
            }
            catch (IOException)
            {
                // [正常现象] 在对方发完消息等待我方回复时退出。
            }
            catch (ObjectDisposedException)
            {
                // Cannot access a closed pipe.
                // [正常现象] 在对方发完消息等待我方回复时退出。
            }
            catch (IpcRemoteException)
            {
                // [正常现象] 因为 IPC 对方已断开连接，所以已无法回复。
            }
            catch (Exception ex)
            {
                IpcContext.Logger.Error(ex.ToString());
#if DEBUG
                // 这里一定说明业务代码写错了，缺少对应的 Handler。
                throw;
#endif
            }
        }
    }
}
