using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using dotnetCampus.Ipc.CompilerServices.Attributes;

using static dotnetCampus.Ipc.CompilerServices.GeneratedProxies.GeneratedIpcFactory;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models
{
    /// <summary>
    /// 在跨进程的 IPC 方法调用中，方法参数或返回值可能并不是基本类型，而是另一个 IPC 类型的实例。
    /// 本类型提供辅助方法判断和转换 <see cref="object"/> 到可被序列化的 IPC 类型，并辅助生成 IPC 类型的代理和对接。
    /// </summary>
    internal static class ObjectToIpcProxyJointConverter
    {
        /// <summary>
        /// 当从 IPC 传输过来一些对象信息时，通过此方法可以判断此对象是否是一个 IPC 公开的对象。
        /// 如果是一个 IPC 公开的对象，则可以从 <paramref name="proxy"/> 获取到这个对象的本地代理。
        /// </summary>
        /// <param name="context">基于 .NET 类型进行 IPC 传输的上下文信息。</param>
        /// <param name="peerProxy">IPC 对方端。</param>
        /// <param name="assemblyQualifiedName">对象真实实例的类型名称。</param>
        /// <param name="objectId">如果可能有同一个契约类型的多个对象，则在此传入此对象的 IPC 访问 Id。</param>
        /// <param name="proxy">如果经判断是一个 IPC 公开的对象，则可以从此参数中获取到这个对象的本地代理。</param>
        /// <returns>如果这是一个 IPC 公开的对象，则返回 true，如果只是一个普通对象，则返回 false。</returns>
        public static bool TryCreateProxyFromSerializationInfo(this GeneratedProxyJointIpcContext context,
            IPeerProxy peerProxy, string? assemblyQualifiedName, string? objectId,
            [NotNullWhen(true)] out object? proxy)
        {
            if (assemblyQualifiedName is not null
                && Type.GetType(assemblyQualifiedName) is { } realType
                && RealTypeToProxyJointCache[realType].proxyType is { } proxyType)
            {
                var proxyInstance = (GeneratedIpcProxy) Activator.CreateInstance(proxyType)!;
                proxyInstance.Context = context;
                proxyInstance.PeerProxy = peerProxy;
                proxyInstance.ObjectId = objectId;
                proxy = proxyInstance;
                return true;
            }
            proxy = default;
            return false;
        }

        /// <summary>
        /// 当试图返回一个 IPC 对象时，创建此对象的 IPC 对接，然后将此对接转换为可被 IPC 传输的对象信息。
        /// </summary>
        /// <param name="context">基于 .NET 类型进行 IPC 传输的上下文信息。</param>
        /// <param name="realInstance">IPC 本地对接。</param>
        /// <param name="objectId">如果可能有同一个契约类型的多个对象，则在此传入此对象的 IPC 访问 Id。</param>
        /// <param name="assemblyQualifiedName">对象真实实例的类型名称。</param>
        /// <returns></returns>
        public static bool TryCreateSerializationInfoFromIpcRealInstance(this GeneratedProxyJointIpcContext context,
            object? realInstance,
            [NotNullWhen(true)] out string? objectId,
            [NotNullWhen(true)] out string? assemblyQualifiedName)
        {
            if (realInstance is not null)
            {
                var realType = realInstance.GetType();
                var (contractType, _, jointType) = RealTypeToProxyJointCache[realType];
                if (contractType is not null && jointType is not null)
                {
                    objectId = Guid.NewGuid().ToString();
                    var jointInstance = (GeneratedIpcJoint) Activator.CreateInstance(jointType)!;
                    jointInstance.SetInstance(realInstance);
                    context.JointManager.AddPublicIpcObject(contractType, jointInstance, objectId);
                    assemblyQualifiedName = realType.AssemblyQualifiedName!;
                    return true;
                }
            }
            objectId = null;
            assemblyQualifiedName = null;
            return false;
        }
    }
}
