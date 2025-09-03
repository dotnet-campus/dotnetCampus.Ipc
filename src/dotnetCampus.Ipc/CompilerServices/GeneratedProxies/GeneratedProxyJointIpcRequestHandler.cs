using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// 为生成的 <see cref="GeneratedIpcJoint{TContract}"/> 提供默认的对 <see cref="GeneratedIpcProxy{TContract}"/> 的对接。
    /// </summary>
    internal sealed class GeneratedProxyJointIpcRequestHandler : IIpcRequestHandler
    {
        private readonly IpcContext _ipcContext;

        internal GeneratedProxyJointIpcContext Owner { get; }

        internal GeneratedProxyJointIpcRequestHandler(GeneratedProxyJointIpcContext context, IpcContext ipcContext)
        {
            Owner = context;
            _ipcContext = ipcContext;
        }

        Task<IIpcResponseMessage> IIpcRequestHandler.HandleRequest(IIpcRequestContext requestContext)
        {
            return _ipcContext.TaskPool.Run(async () =>
            {
                if (Owner.JointManager.TryJoint(requestContext, out var responseTask))
                {
                    requestContext.Handled = true;
                    var response = await responseTask.ConfigureAwait(false);
                    return response;
                }

                return KnownIpcResponseMessages.CannotHandle;
            }, _ipcContext.Logger);
        }
    }
}
