using System;
using System.Collections.Concurrent;
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
        /// 编译期 IPC 类型的程序集到此程序集中的所有编译期 IPC 类型的缓存。
        /// </summary>
        private static readonly ConcurrentDictionary<Assembly, AssemblyIpcProxyJointAttribute[]> AssemblyIpcAttributesCache = new();

        /// <summary>
        /// 编译期 IPC 类型（标记了 <see cref="IpcPublicAttribute"/> 的接口或标记了 <see cref="IpcShapeAttribute"/> 的代理壳类型）到代理对接类型的缓存。
        /// </summary>
        internal static CachePool<Type, (Type? proxyType, Type? jointType)> IpcTypeToProxyJointCache { get; } = new(ConvertShapeTypeToProxyJointTypes, true);

        /// <summary>
        /// 编译期 IPC 类型的完整名称到 IPC 类型的缓存。
        /// </summary>
        internal static CachePool<string, Type?> IpcTypeFullNameToIpcTypeCache { get; } = new(ConvertIpcTypeFullNameToProxyJointTypes, true);

        /// <summary>
        /// 创建用于通过 IPC 访问其他端 <typeparamref name="TContract"/> 类型的代理对象。
        /// </summary>
        /// <typeparam name="TContract">IPC 对象的契约类型。</typeparam>
        /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
        /// <param name="peer">IPC 远端。</param>
        /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
        /// <returns>契约类型。</returns>
        public static TContract CreateIpcProxy<TContract>(this IIpcProvider ipcProvider, IPeerProxy peer, string? ipcObjectId = null)
            where TContract : class
        {
            if (IpcTypeToProxyJointCache[typeof(TContract)].proxyType is { } proxyType)
            {
                var proxy = (GeneratedIpcProxy<TContract>) Activator.CreateInstance(proxyType)!;
                proxy.Context = GetContext(ipcProvider);
                proxy.PeerProxy = peer;
                proxy.ObjectId = ipcObjectId;
                return (TContract) (object) proxy;
            }
            else
            {
                throw new ArgumentException($"接口 {typeof(TContract).Name} 上没有找到 {typeof(IpcPublicAttribute).Name} 特性，因此不知道如何创建 {typeof(TContract).Name} 的 IPC 代理。", nameof(TContract));
            }
        }

        /// <summary>
        /// 创建用于通过 IPC 访问其他端 <typeparamref name="TContract"/> 类型的代理对象。
        /// </summary>
        /// <typeparam name="TContract">IPC 对象的契约类型。</typeparam>
        /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
        /// <param name="peer">IPC 远端。</param>
        /// <param name="ipcProxyConfigs">指定创建的 IPC 代理在进行 IPC 通信时应使用的相关配置。</param>
        /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
        /// <returns>契约类型。</returns>
        public static TContract CreateIpcProxy<TContract>(this IIpcProvider ipcProvider, IPeerProxy peer, IpcProxyConfigs ipcProxyConfigs, string? ipcObjectId = null)
            where TContract : class
        {
            if (IpcTypeToProxyJointCache[typeof(TContract)].proxyType is { } proxyType)
            {
                var proxy = (GeneratedIpcProxy<TContract>) Activator.CreateInstance(proxyType)!;
                proxy.Context = GetContext(ipcProvider);
                proxy.PeerProxy = peer;
                proxy.ObjectId = ipcObjectId;
                proxy.RuntimeConfigs = ipcProxyConfigs;
                return (TContract) (object) proxy;
            }
            else
            {
                throw new ArgumentException($"接口 {typeof(TContract).Name} 上没有找到 {typeof(IpcPublicAttribute).Name} 特性，因此不知道如何创建 {typeof(TContract).Name} 的 IPC 代理。", nameof(TContract));
            }
        }

        /// <summary>
        /// 创建用于通过 IPC 访问其他端 <typeparamref name="TContract"/> 类型的代理对象。
        /// </summary>
        /// <typeparam name="TContract">IPC 对象的契约类型。</typeparam>
        /// <typeparam name="TShape">用于配置 IPC 代理行为的 IPC 代理壳类型。</typeparam>
        /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
        /// <param name="peer">IPC 远端。</param>
        /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
        /// <returns>契约类型。</returns>
        public static TContract CreateIpcProxy<TContract, TShape>(this IIpcProvider ipcProvider, IPeerProxy peer, string? ipcObjectId = null)
            where TContract : class
        {
            if (IpcTypeToProxyJointCache[typeof(TShape)].proxyType is { } proxyType)
            {
                var proxy = (GeneratedIpcProxy<TContract>) Activator.CreateInstance(proxyType)!;
                proxy.Context = GetContext(ipcProvider);
                proxy.PeerProxy = peer;
                proxy.ObjectId = ipcObjectId;
                return (TContract) (object) proxy;
            }
            else
            {
                throw new ArgumentException(
                    $"类型 {typeof(TShape).Name} 上没有找到 {typeof(IpcShapeAttribute).Name} 特性，因此不知道如何创建 {typeof(TContract).Name} 的 IPC 代理。",
                    nameof(TShape));
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
            if (IpcTypeToProxyJointCache[typeof(TContract)].jointType is { } jointType)
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

        private static AssemblyIpcProxyJointAttribute[] ConvertAssemblyToIpcAttributes(Assembly assembly)
        {
            return assembly.GetCustomAttributes<AssemblyIpcProxyJointAttribute>().ToArray();
        }

        private static Type? ConvertIpcTypeFullNameToProxyJointTypes(string ipcTypeFullName) => AssemblyIpcAttributesCache
            .SelectMany(x => x.Value)
            .Select(x => x.IpcType)
            .FirstOrDefault(x => x.FullName == ipcTypeFullName);

        /// <summary>
        /// 编译期契约与傀儡类型到代理对接的转换。
        /// </summary>
        /// <param name="ipcType">标记了 <see cref="IpcPublicAttribute"/> 的契约类型或标记了 <see cref="IpcShapeAttribute"/> 的代理壳类型。</param>
        /// <returns>IPC 类型。</returns>
        private static (Type? proxyType, Type? jointType) ConvertShapeTypeToProxyJointTypes(Type ipcType)
        {
            var attributes = AssemblyIpcAttributesCache.GetOrAdd(
                ipcType.Assembly,
                _ => ipcType.Assembly.GetCustomAttributes<AssemblyIpcProxyJointAttribute>().ToArray());

            if (ipcType?.IsDefined(typeof(IpcShapeAttribute)) is true)
            {
                // 因为 IpcShape 继承了 IpcPublic，所以需要首先检查代理壳，否则 IpcPublic 接口直接就通过了，产生错误。
                var attribute = attributes.FirstOrDefault(x => x.IpcType == ipcType);
                if (attribute is null)
                {
                    throw new NotSupportedException($"因为编译时没有生成“{ipcType.Name}”代理壳的 IPC 代理类，所以运行时无法创建它们的实例。请确保使用 Visual Studio 2022 或以上版本、MSBuild 17 或以上版本进行编译。");
                }
                return (attribute.ProxyType, null);
            }
            else if (ipcType?.IsDefined(typeof(IpcPublicAttribute)) is true)
            {
                // 随后再检查 IpcPublic。
                var attribute = attributes.FirstOrDefault(x => x.IpcType == ipcType);
                if (attribute is null)
                {
                    throw new NotSupportedException($"因为编译时没有生成“{ipcType.Name}”接口的 IPC 代理与对接类，所以运行时无法创建它们的实例。请确保使用 Visual Studio 2022 或以上版本、MSBuild 17 或以上版本进行编译。");
                }
                return (attribute.ProxyType, attribute.JointType);
            }
            return (null, null);
        }
    }
}
