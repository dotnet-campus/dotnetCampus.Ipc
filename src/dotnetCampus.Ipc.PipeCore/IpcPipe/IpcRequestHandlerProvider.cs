﻿using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.PipeCore.Context;
using dotnetCampus.Ipc.PipeCore.IpcPipe;

namespace dotnetCampus.Ipc.PipeCore
{
    /// <summary>
    /// 关联 <see cref="ResponseManager"/> 和 <see cref="IIpcRequestHandler"/> 的联系
    /// </summary>
    class IpcRequestHandlerProvider
    {
        public IpcRequestHandlerProvider(IpcContext ipcContext)
        {
            IpcContext = ipcContext;
        }

        public IpcContext IpcContext { get; }

        public void HandleRequest(PeerProxy sender, IpcClientRequestArgs args)
        {
            //var requestMessage = args.IpcBufferMessage;

            //var ipcRequestHandler = IpcContext.IpcConfiguration.DefaultIpcRequestHandler;

             
        }
    }
}