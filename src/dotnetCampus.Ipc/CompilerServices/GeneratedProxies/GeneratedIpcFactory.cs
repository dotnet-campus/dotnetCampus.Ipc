using System.Collections.Concurrent;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using dotnetCampus.Ipc.CompilerServices.Attributes;
#if !NET5_0_OR_GREATER
using System.Reflection;
#endif

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
    /// 编译期 IPC 类型（标记了 <see cref="IpcPublicAttribute"/> 的接口）到代理对接对象的创建器。
    /// </summary>
    internal static ConcurrentDictionary<Type, (Func<GeneratedIpcProxy> ProxyFactory, Func<GeneratedIpcJoint> JointFactory)> IpcPublicFactories { get; } = [];

    /// <summary>
    /// 编译期 IPC 类型（标记了 <see cref="IpcShapeAttribute"/> 的形状代理类型）到代理对接对象的创建器。
    /// </summary>
    private static ConcurrentDictionary<Type, Func<GeneratedIpcProxy>> IpcShapeFactories { get; } = [];

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
        IpcPublicFactories[typeof(TPublic)] = (proxyFactory, jointFactory);
    }

    /// <summary>
    /// 由源生成器调用，注册 IPC 对象的形状代理。
    /// </summary>
    /// <param name="shapeFactory">IPC 形状代理创建器。</param>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <typeparam name="TShape">IPC 对象的形状类型。</typeparam>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static void RegisterIpcShape<TPublic, TShape>(Func<GeneratedIpcProxy<TPublic>> shapeFactory)
        where TPublic : class
        where TShape : class
    {
        IpcShapeFactories[typeof(TShape)] = shapeFactory;
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
        if (SafeGetIpcPublicFactories<TPublic>().ProxyFactory is not { } proxyFactory)
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
        if (SafeGetIpcPublicFactories<TPublic>().ProxyFactory is not { } proxyFactory)
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
        if (SafeGetIpcShapeFactory<TPublic, TShape>() is not { } proxyFactory)
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
        if (SafeGetIpcPublicFactories<TPublic>().JointFactory is not { } jointFactory)
        {
            throw new ArgumentException(
                $"类型 {typeof(TPublic).Name} 上没有找到 {nameof(IpcPublicAttribute)} 特性，因此不知道如何创建 {typeof(TPublic).Name} 的 IPC 对接。",
                nameof(realInstance));
        }

        var context = ipcProvider.GetGeneratedContext();
        var joint = (GeneratedIpcJoint<TPublic>)jointFactory();
        joint.Context = context;
        joint.SetInstance(realInstance);
        context.JointManager.AddPublicIpcObject(joint, ipcObjectId);
        return realInstance;
    }

    /// <summary>
    /// 安全地获取 IPC 公共类型的代理与对接创建器。
    /// </summary>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <returns>代理与对接创建器。</returns>
    /// <remarks>
    /// 此方法虽然可能在线程并发时多次创建 IPC 公共类型的代理与对接创建器，但最终只会缓存并返回一个创建器。<br/>
    /// 又由于此方法是幂等的，无论执行多少次，都不会往字典里存放多余的创建器；所以总体而言是线程安全的。
    /// </remarks>
    private static (Func<GeneratedIpcProxy>? ProxyFactory, Func<GeneratedIpcJoint>? JointFactory) SafeGetIpcPublicFactories<TPublic>()
        where TPublic : class
    {
        if (IpcPublicFactories.TryGetValue(typeof(TPublic), out var factories))
        {
            return factories;
        }

#if !NET5_0_OR_GREATER
        // 兼容旧版本 .NET 的反射实现。
        foreach (var attribute in typeof(TPublic).Assembly.GetCustomAttributes<AssemblyIpcProxyJointAttribute>())
        {
            IpcPublicFactories.TryAdd(attribute.IpcType, (
                () => (GeneratedIpcProxy)Activator.CreateInstance(attribute.ProxyType)!,
                () => (GeneratedIpcJoint)Activator.CreateInstance(attribute.JointType)!
            ));
        }
        if (IpcPublicFactories.TryGetValue(typeof(TPublic), out factories))
        {
            return factories;
        }
#endif

        return default;
    }

    /// <summary>
    /// 安全地获取 IPC 形状类型的代理创建器。
    /// </summary>
    /// <typeparam name="TPublic">IPC 对象的契约类型。</typeparam>
    /// <typeparam name="TShape">IPC 对象的形状类型。</typeparam>
    /// <returns>代理创建器。</returns>
    /// <remarks>
    /// 此方法虽然可能在线程并发时多次创建 IPC 公共类型的代理与对接创建器，但最终只会缓存并返回一个创建器。<br/>
    /// 又由于此方法是幂等的，无论执行多少次，都不会往字典里存放多余的创建器；所以总体而言是线程安全的。
    /// </remarks>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Func<GeneratedIpcProxy>? SafeGetIpcShapeFactory<TPublic, TShape>()
        where TPublic : class
    {
        if (IpcShapeFactories.TryGetValue(typeof(TShape), out var factories))
        {
            return factories;
        }

#if !NET5_0_OR_GREATER
        // 兼容旧版本 .NET 的反射实现。
        foreach (var attribute in typeof(TShape).Assembly.GetCustomAttributes<AssemblyIpcProxyAttribute>())
        {
            IpcShapeFactories.TryAdd(attribute.IpcType, () => (GeneratedIpcProxy)Activator.CreateInstance(attribute.ProxyType)!);
        }
        if (IpcShapeFactories.TryGetValue(typeof(TShape), out factories))
        {
            return factories;
        }
#endif

        return null;
    }

    private static GeneratedProxyJointIpcContext GetGeneratedContext(this IIpcProvider ipcProvider)
        => ipcProvider.IpcContext.GeneratedProxyJointIpcContext;
}
