using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.PipeCore.Context;
using dotnetCampus.Ipc.PipeCore.IpcPipe;

namespace dotnetCampus.Ipc.PipeCore
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
            var requestMessage = args.IpcBufferMessage;

            var ipcRequestContext = new IpcRequestContext(requestMessage);

            // 处理消息
            IIpcRequestHandler ipcRequestHandler = IpcContext.IpcConfiguration.DefaultIpcRequestHandler;
            var result = ipcRequestHandler.HandleRequestMessage(ipcRequestContext);

            // 构建信息回复
            var responseManager = IpcContext.IpcMessageResponseManager;
            var responseMessage = responseManager.CreateResponseMessage(args.MessageId, result.ReturnMessage);

            var peerProxy = sender;

            // 发送回客户端
            await peerProxy.IpcClientService.WriteMessageAsync(responseMessage);
        }
    }
}

