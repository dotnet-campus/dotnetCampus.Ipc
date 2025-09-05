using System.Collections.Concurrent;
using System.ComponentModel;
using System.Reflection;
using dotnetCampus.Ipc.CompilerServices.Attributes;
using dotnetCampus.Ipc.Utils.Caching;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

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
    private static readonly ConcurrentDictionary<Assembly, AssemblyIpcProxyJointAttribute[]> AssemblyIpcAttributesCache = [];

    /// <summary>
    /// 编译期 IPC 类型（标记了 <see cref="IpcPublicAttribute"/> 的接口或标记了 <see cref="IpcShapeAttribute"/> 的形状代理类型）到代理对接类型的缓存。
    /// </summary>
    internal static CachePool<Type, (Type? proxyType, Type? jointType)> IpcTypeToProxyJointCache { get; } = new(ConvertShapeTypeToProxyJointTypes, true);

    /// <summary>
    /// 编译期 IPC 类型（标记了 <see cref="IpcPublicAttribute"/> 的接口或标记了 <see cref="IpcShapeAttribute"/> 的形状代理类型）到代理对接对象的创建器。
    /// </summary>
    internal static ConcurrentDictionary<Type, (Func<GeneratedIpcProxy>? ProxyFactory, Func<GeneratedIpcJoint>? JointFactory)> IpcFactories { get; } = [];

    /// <summary>
    /// 由源生成器调用，注册 IPC 对象的代理与对接创建器。
    /// </summary>
    /// <param name="proxyFactory">IPC 代理创建器。</param>
    /// <param name="jointFactory">IPC 对接创建器。</param>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void RegisterIpcPublic<TPublic>(Func<GeneratedIpcProxy<TPublic>> proxyFactory, Func<GeneratedIpcJoint<TPublic>> jointFactory)
        where TPublic : class
    {
        IpcFactories[typeof(TPublic)] = (proxyFactory, jointFactory);
    }

    /// <summary>
    /// 由源生成器调用，注册 IPC 对象的形状代理。
    /// </summary>
    /// <param name="shapeFactory">IPC 形状代理创建器。</param>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <typeparam name="TShape">IPC 对象的形状类型。</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void RegisterIpcShape<TPublic, TShape>(Func<GeneratedIpcProxy<TPublic>>? shapeFactory)
        where TPublic : class
        where TShape : class
    {
        IpcFactories[typeof(TShape)] = (shapeFactory, null);
    }

    /// <summary>
    /// 创建用于通过 IPC 访问其他端 <typeparamref name="TPublic"/> 类型的代理对象。
    /// </summary>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
    /// <param name="peer">IPC 远端。</param>
    /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
    /// <returns>契约类型。</returns>
    public static TPublic CreateIpcProxy<TPublic>(this IIpcProvider ipcProvider, IPeerProxy peer, string? ipcObjectId = null)
        where TPublic : class
    {
        if (IpcFactories[typeof(TPublic)].ProxyFactory is not { } proxyFactory)
        {
            throw new ArgumentException(
                $"接口 {typeof(TPublic).Name} 上没有找到 {nameof(IpcPublicAttribute)} 特性，因此不知道如何创建 {typeof(TPublic).Name} 的 IPC 代理。",
                nameof(TPublic));
        }

        var proxy = (GeneratedIpcProxy<TPublic>)proxyFactory();
        proxy.Context = ipcProvider.GetGeneratedContext();
        proxy.PeerProxy = peer;
        proxy.ObjectId = ipcObjectId;
        return (TPublic)(object)proxy;
    }

    /// <summary>
    /// 创建用于通过 IPC 访问其他端 <typeparamref name="TPublic"/> 类型的代理对象。
    /// </summary>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
    /// <param name="peer">IPC 远端。</param>
    /// <param name="ipcProxyConfigs">指定创建的 IPC 代理在进行 IPC 通信时应使用的相关配置。</param>
    /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
    /// <returns>契约类型。</returns>
    public static TPublic CreateIpcProxy<TPublic>(this IIpcProvider ipcProvider, IPeerProxy peer, IpcProxyConfigs ipcProxyConfigs,
        string? ipcObjectId = null)
        where TPublic : class
    {
        if (IpcFactories[typeof(TPublic)].ProxyFactory is not { } proxyFactory)
        {
            throw new ArgumentException(
                $"接口 {typeof(TPublic).Name} 上没有找到 {nameof(IpcPublicAttribute)} 特性，因此不知道如何创建 {typeof(TPublic).Name} 的 IPC 代理。",
                nameof(TPublic));
        }

        var proxy = (GeneratedIpcProxy<TPublic>)proxyFactory();
        proxy.Context = ipcProvider.GetGeneratedContext();
        proxy.PeerProxy = peer;
        proxy.ObjectId = ipcObjectId;
        proxy.RuntimeConfigs = ipcProxyConfigs;
        return (TPublic)(object)proxy;
    }

    /// <summary>
    /// 创建用于通过 IPC 访问其他端 <typeparamref name="TPublic"/> 类型的代理对象。
    /// </summary>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <typeparam name="TShape">用于配置 IPC 代理行为的 IPC 形状代理类型。</typeparam>
    /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
    /// <param name="peer">IPC 远端。</param>
    /// <param name="ipcObjectId">如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。</param>
    /// <returns>契约类型。</returns>
    public static TPublic CreateIpcProxy<TPublic, TShape>(this IIpcProvider ipcProvider, IPeerProxy peer, string? ipcObjectId = null)
        where TPublic : class
    {
        if (IpcFactories[typeof(TShape)].ProxyFactory is not { } proxyFactory)
        {
            throw new ArgumentException(
                $"类型 {typeof(TShape).Name} 上没有找到 {nameof(IpcShapeAttribute)} 特性，因此不知道如何创建 {typeof(TPublic).Name} 的 IPC 代理。",
                nameof(TShape));
        }

        var proxy = (GeneratedIpcProxy<TPublic>)proxyFactory();
        proxy.Context = ipcProvider.GetGeneratedContext();
        proxy.PeerProxy = peer;
        proxy.ObjectId = ipcObjectId;
        return (TPublic)(object)proxy;
    }

    /// <summary>
    /// 创建用于对接来自其他端通过 IPC 访问 <typeparamref name="TPublic"/> 类型的对接对象。
    /// </summary>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <param name="ipcProvider">关联的 <see cref="IIpcProvider"/>。</param>
    /// <param name="realInstance">真实的对象。</param>
    /// <param name="ipcObjectId">如果被对接的对象有多个实例，请设置此 Id 值以对接正确的实例。</param>
    public static TPublic CreateIpcJoint<TPublic>(this IIpcProvider ipcProvider, TPublic realInstance, string? ipcObjectId = null)
        where TPublic : class
    {
        var realType = realInstance.GetType();
        if (IpcFactories[typeof(TPublic)].JointFactory is not { } jointFactory)
        {
            throw new ArgumentException(
                $"类型 {realType.Name} 上没有找到 {nameof(IpcPublicAttribute)} 特性，因此不知道如何创建 {typeof(TPublic).Name} 的 IPC 对接。",
                nameof(realInstance));
        }

        var context = ipcProvider.GetGeneratedContext();
        var joint = (GeneratedIpcJoint<TPublic>)jointFactory();
        joint.Context = context;
        joint.SetInstance(realInstance);
        context.JointManager.AddPublicIpcObject(joint, ipcObjectId);
        return realInstance;
    }

    private static GeneratedProxyJointIpcContext GetGeneratedContext(this IIpcProvider ipcProvider)
        => ipcProvider.IpcContext.GeneratedProxyJointIpcContext;

    /// <summary>
    /// 编译期契约与傀儡类型到代理对接的转换。
    /// </summary>
    /// <param name="ipcType">标记了 <see cref="IpcPublicAttribute"/> 的契约类型或标记了 <see cref="IpcShapeAttribute"/> 的形状代理类型。</param>
    /// <returns>IPC 类型。</returns>
    private static (Type? proxyType, Type? jointType) ConvertShapeTypeToProxyJointTypes(Type ipcType)
    {
        var attributes = AssemblyIpcAttributesCache.GetOrAdd(
            ipcType.Assembly,
            _ => ipcType.Assembly.GetCustomAttributes<AssemblyIpcProxyJointAttribute>().ToArray());

        if (ipcType?.IsDefined(typeof(IpcShapeAttribute)) is true)
        {
            // 因为 IpcShape 继承了 IpcPublic，所以需要首先检查形状代理，否则 IpcPublic 接口直接就通过了，产生错误。
            var attribute = attributes.FirstOrDefault(x => x.IpcType == ipcType);
            if (attribute is null)
            {
                throw new NotSupportedException($"因为编译时没有生成“{ipcType.Name}”形状代理的 IPC 代理类，所以运行时无法创建它们的实例。请确保使用 Visual Studio 2022 或以上版本、MSBuild 17 或以上版本进行编译。");
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
