using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.Context
{
    /// <summary>
    /// 基于委托的 IPC 请求处理
    /// </summary>
    public class DelegateIpcRequestHandler : IIpcRequestHandler
    {
        /// <summary>
        /// 创建基于委托的 IPC 请求处理
        /// </summary>
        /// <param name="handler"></param>
        public DelegateIpcRequestHandler(Func<IIpcRequestContext, Task<IIpcResponseMessage>> handler)
        {
            _handler = handler;
        }

        /// <summary>
        /// 创建基于委托的 IPC 请求处理
        /// </summary>
        public DelegateIpcRequestHandler(Func<IIpcRequestContext, IIpcResponseMessage> handler)
        {
            _handler = c => Task.FromResult(handler(c));
        }

        Task<IIpcResponseMessage> IIpcRequestHandler.HandleRequest(IIpcRequestContext requestContext)
        {
            return _handler(requestContext);
        }

        private readonly Func<IIpcRequestContext, Task<IIpcResponseMessage>> _handler;
    }
}
