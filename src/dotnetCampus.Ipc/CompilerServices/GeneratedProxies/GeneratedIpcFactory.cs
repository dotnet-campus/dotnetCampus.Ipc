using System;
using System.Linq;
using System.Reflection;

using dotnetCampus.Ipc.CompilerServices.Attributes;
using dotnetCampus.Ipc.Utils.Caching;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// 此工厂包含 <see cref="Ipc"/> 库中对 <see cref="IIpcProvider"/> 的上下文扩展，
    /// 因此可基于此上下文创建仅限于此 <see cref="Ipc"/> 库的功能类，包括：
    /// <list type="bullet">
    /// <item><see cref="IIpcRequestHandler"/></item>
    /// <item><see cref="GeneratedIpcProxy{TContract}"/></item>
    /// <item><see cref="GeneratedIpcJoint{TContract}"/></item>
    /// </list>
    /// </summary>
    public static class GeneratedIpcFactory
    {
        /// <summary>
        /// 真实类型到代理对接的缓存。
        /// </summary>
        internal static CachePool<Type, (Type? contractType, Type? proxyType, Type? jointType)> RealTypeToProxyJointCache { get; } = new(ConvertTypeToProxyJointTypes, true);

        /// <summary>
        /// 创建用于通过 IPC 访问其他端 <typeparamref name="TContract"/> 类型的代理对象。
        /// </summary>
        /// <typeparam name="TContract">IPC 对象的契约类型。</typeparam>
        /// <typeparam name="TRealType">IPC 实现对象的类型。</typeparam>
        /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
        /// <param name="peer">IPC 远端。</param>
        /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
        /// <returns>契约类型。</returns>
        public static TContract CreateIpcProxy<TContract, TRealType>(this IIpcProvider ipcProvider, IPeerProxy peer, string? ipcObjectId = null)
            where TContract : class
            where TRealType : TContract
        {
            if (RealTypeToProxyJointCache[typeof(TRealType)].proxyType is { } proxyType)
            {
                var proxy = (GeneratedIpcProxy<TContract>) Activator.CreateInstance(proxyType)!;
                proxy.Context = GetContext(ipcProvider);
                proxy.PeerProxy = peer;
                proxy.ObjectId = ipcObjectId;
                return (TContract) (object) proxy;
            }
            else
            {
                throw new ArgumentException($"类型 {typeof(TRealType).Name} 上没有找到 {typeof(IpcPublicAttribute).Name} 特性，因此不知道如何创建 {typeof(TContract).Name} 的 IPC 代理。", nameof(TRealType));
            }
        }

        /// <summary>
        /// 创建用于对接来自其他端通过 IPC 访问 <typeparamref name="TContract"/> 类型的对接对象。
        /// </summary>
        /// <typeparam name="TContract">IPC 对象的契约类型。</typeparam>
        /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
        /// <param name="realInstance">真实的对象。</param>
        /// <param name="ipcObjectId">如果被对接的对象有多个实例，请设置此 Id 值以对接正确的实例。</param>
        public static TContract CreateIpcJoint<TContract>(this IIpcProvider ipcProvider, TContract realInstance, string? ipcObjectId = null)
            where TContract : class
        {
            var realType = realInstance.GetType();
            if (RealTypeToProxyJointCache[realType].jointType is { } jointType)
            {
                var joint = (GeneratedIpcJoint<TContract>) Activator.CreateInstance(jointType)!;
                joint.SetInstance(realInstance);
                GetContext(ipcProvider).JointManager.AddPublicIpcObject(joint, ipcObjectId);
                return realInstance;
            }
            else
            {
                throw new ArgumentException($"类型 {realType.Name} 上没有找到 {typeof(IpcPublicAttribute).Name} 特性，因此不知道如何创建 {typeof(TContract).Name} 的 IPC 对接。", nameof(realInstance));
            }
        }

        private static GeneratedProxyJointIpcContext GetContext(IIpcProvider ipcProvider)
            => ipcProvider.IpcContext.GeneratedProxyJointIpcContext;

        /// <summary>
        /// 真实类型到代理对接的转换。
        /// </summary>
        /// <param name="realType">真实类型。</param>
        /// <returns>IPC 类型。</returns>
        private static (Type? contractType, Type? proxyType, Type? jointType) ConvertTypeToProxyJointTypes(Type? realType)
        {
            if (realType?.IsDefined(typeof(IpcPublicAttribute)) is true)
            {
                var contractType = realType.GetCustomAttribute<IpcPublicAttribute>()!.ContractType;
                var attribute = realType.Assembly.GetCustomAttributes<AssemblyIpcProxyJointAttribute>()
                    .FirstOrDefault(x => x.ContractType == contractType);
                if (attribute is null)
                {
                    throw new NotSupportedException($"因为编译时没有生成“{realType.Name} : {contractType.Name}”类型的 IPC 代理与对接类，所以运行时无法创建他们的实例。请确保使用 Visual Studio 2022 或以上版本、MSBuild 17 或以上版本进行编译。");
                }
                return (attribute.ContractType, attribute.ProxyType, attribute.JointType);
            }
            return (null, null, null);
        }
    }
}
