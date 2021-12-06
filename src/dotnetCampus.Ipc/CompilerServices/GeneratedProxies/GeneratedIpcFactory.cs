using System;
using System.Collections.Concurrent;
using System.Linq;

using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// 此工厂包含 <see cref="dotnetCampus.Ipc"/> 库中对 <see cref="IIpcProvider"/> 的上下文扩展，
    /// 因此可基于此上下文创建仅限于此 <see cref="dotnetCampus.Ipc"/> 库的功能类，包括：
    /// <list type="bullet">
    /// <item><see cref="IIpcRequestHandler"/></item>
    /// <item><see cref="GeneratedIpcProxy{TContract}"/></item>
    /// <item><see cref="GeneratedIpcJoint{TContract}"/></item>
    /// </list>
    /// </summary>
    public static class GeneratedIpcFactory
    {
        /// <summary>
        /// 创建用于通过 IPC 访问其他端 <typeparamref name="TContract"/> 类型的代理对象，而此代理对象的类型为 <typeparamref name="TIpcProxy"/>。
        /// </summary>
        /// <typeparam name="TContract">IPC 对象的契约类型。</typeparam>
        /// <typeparam name="TIpcProxy">IPC 代理对象的类型。</typeparam>
        /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
        /// <param name="peer">IPC 远端。</param>
        /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
        /// <returns>契约类型。</returns>
        public static TContract CreateIpcProxy<TContract, TIpcProxy>(this IIpcProvider ipcProvider, IPeerProxy peer, string? ipcObjectId = null)
            where TContract : class
            where TIpcProxy : GeneratedIpcProxy<TContract>, TContract, new() => new TIpcProxy
            {
                Context = GetContext(ipcProvider),
                PeerProxy = peer,
                ObjectId = ipcObjectId
            };

        /// <summary>
        /// 创建用于对接来自其他端通过 IPC 访问 <typeparamref name="TContract"/> 类型的对接对象，而此对接对象的类型为 <typeparamref name="TIpcJoint"/>。
        /// </summary>
        /// <typeparam name="TContract">IPC 对象的契约类型。</typeparam>
        /// <typeparam name="TIpcJoint">IPC 对接对象的类型。</typeparam>
        /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
        /// <param name="realInstance">真实的对象。</param>
        /// <param name="ipcObjectId">如果被对接的对象有多个实例，请设置此 Id 值以对接正确的实例。</param>
        public static TContract CreateIpcJoint<TContract, TIpcJoint>(this IIpcProvider ipcProvider, TContract realInstance, string? ipcObjectId = null)
            where TContract : class
            where TIpcJoint : GeneratedIpcJoint<TContract>, new()
        {
            var joint = new TIpcJoint();
            joint.SetInstance(realInstance);
            GetContext(ipcProvider).JointManager.AddPublicIpcObject(joint, ipcObjectId);
            return realInstance;
        }

        private static GeneratedProxyJointIpcContext GetContext(IIpcProvider ipcProvider)
            => ipcProvider.IpcContext.GeneratedProxyJointIpcContext;
    }
}
