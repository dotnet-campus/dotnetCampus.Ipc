using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Exceptions;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
/// <summary>
/// 为自动生成的 IPC 对接类提供基类。
/// </summary>
public abstract class GeneratedIpcJoint
{
    private GeneratedProxyJointIpcContext? _context;

    /// <summary>
    /// 提供基于 .NET 类型的 IPC 传输上下文信息。
    /// </summary>
    internal GeneratedProxyJointIpcContext Context
    {
        get => _context ?? throw new IpcRemoteException($"基于 .NET 类型的 IPC 传输机制应使用 {typeof(GeneratedIpcFactory)} 工厂类型来构造。");
        set => _context = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// 设置此对接对象的真实实例。
    /// </summary>
    /// <param name="realInstance">真实实例。</param>
    internal abstract void SetInstance(object realInstance);
    internal abstract IGarmObject GetProperty(ulong memberId, string propertyName);
    internal abstract IGarmObject SetProperty(ulong memberId, string propertyName, IGarmObject value);
    internal abstract IGarmObject CallMethod(ulong memberId, string methodName, IGarmObject[]? args);
    internal abstract Task<IGarmObject> CallMethodAsync(ulong memberId, string methodName, IGarmObject[]? args);
    internal abstract Type[] GetParameterTypes(MemberInvokingType invokingType, ulong memberId);
}

/// <summary>
/// 为自动生成的 IPC 对接类提供基类。
/// </summary>
/// <typeparam name="TContract">应该对接的契约类型（必须是一个接口）。</typeparam>
public abstract partial class GeneratedIpcJoint<TContract> : GeneratedIpcJoint where TContract : class
{
    /// <summary>
    /// 获取默认的 GARM 对象。
    /// </summary>
    private static IGarmObject DefaultGarm => GarmObjectExtensions.Default;

    /// <summary>
    /// 获取属性值的方法集合。
    /// </summary>
    private readonly Dictionary<ulong, (Type[] types, Func<IGarmObject> getter)> _propertyGetters = new();

    /// <summary>
    /// 设置属性值的方法集合（Key 为 MemberId，用于标识一个接口内的唯一一个成员，其中属性的 get 和 set 分别是两个不同的成员）。
    /// </summary>
    private readonly Dictionary<ulong, (Type[] types, Action<IGarmObject> setter)> _propertySetters = new();

    /// <summary>
    /// 调用方法的方法集合（Key 为 MemberId，用于标识一个接口内的唯一一个成员，其中属性的 get 和 set 分别是两个不同的成员）。
    /// </summary>
    private readonly Dictionary<ulong, (Type[] types, Func<IGarmObject[]?, IGarmObject> method)> _methods = new();

    /// <summary>
    /// 调用异步方法的方法集合。
    /// </summary>
    private readonly Dictionary<ulong, (Type[] types, Func<IGarmObject[]?, Task<IGarmObject>> asyncMethod)> _asyncMethods = new();

    /// <summary>
    /// 设置此对接对象的真实实例。
    /// </summary>
    /// <param name="realInstance">真实实例。</param>
    internal sealed override void SetInstance(object realInstance) => SetInstance((TContract) realInstance);

    /// <summary>
    /// 设置此对接对象的真实实例。
    /// </summary>
    /// <param name="realInstance">真实实例。</param>
    internal void SetInstance(TContract realInstance)
    {
        _propertyGetters.Clear();
        _propertySetters.Clear();
        _methods.Clear();
        _asyncMethods.Clear();

        MatchMembers(realInstance);
    }

    /// <summary>
    /// 派生类重写此方法时，通过调用一系列 MatchXxx 方法来将 <typeparamref name="TContract"/> 接口中的所有成员与对接方法进行匹配。
    /// </summary>
    /// <param name="realInstance">当对接时，可使用此参数来访问真实对象。</param>
    protected abstract void MatchMembers(TContract realInstance);

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod(ulong memberId, Action methodInvoker)
    {
        _methods.Add(memberId, (EmptyTypeArray, _ =>
        {
            methodInvoker();
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<TReturn>(ulong memberId, Func<Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, (EmptyTypeArray, _ => methodInvoker()));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod(ulong memberId, Func<Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, (EmptyTypeArray, async _ =>
        {
            await methodInvoker().ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<TReturn>(ulong memberId, Func<Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, (EmptyTypeArray, async _ => await methodInvoker().ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T>(ulong memberId, Action<T> methodInvoker)
    {
        _methods.Add(memberId, (new[] { typeof(T) }, args =>
        {
            methodInvoker(CastArg<T>(args![0])!);
            return DefaultGarm;
        }
        ));
    }

    private static Type[] EmptyTypeArray

#if NET461_OR_GREATER || NETCOREAPP3_0_OR_GREATER
        => Array.Empty<Type>();
#else
        => new Type[0];
#endif


    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T>(ulong memberId, Func<T, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, (new[] { typeof(T) }, async args =>
        {
            await methodInvoker(CastArg<T>(args![0])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T, TReturn>(ulong memberId, Func<T, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, (new[] { typeof(T) }, args => methodInvoker(CastArg<T>(args![0])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T, TReturn>(ulong memberId, Func<T, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, (new[] { typeof(T) }, async args => await methodInvoker(CastArg<T>(args![0])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个属性，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="getter"/> 所指向的具体实现。
    /// </summary>
    /// <param name="getPropertyId">属性 get 的签名的 Id。</param>
    /// <param name="getter">get 的对接实现。</param>
    protected void MatchProperty<T>(ulong getPropertyId, Func<Garm<T>> getter)
    {
        _propertyGetters.Add(getPropertyId, (EmptyTypeArray, () => getter()));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个属性，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="getter"/> 所指向的具体实现。
    /// </summary>
    /// <param name="getPropertyId">属性 get 的签名的 Id。</param>
    /// <param name="setPropertyId">属性 set 的签名的 Id。</param>
    /// <param name="getter">get 的对接实现。</param>
    /// <param name="setter">set 的对接实现。</param>
    protected void MatchProperty<T>(ulong getPropertyId, ulong setPropertyId, Func<Garm<T>> getter, Action<T> setter)
    {
        _propertyGetters.Add(getPropertyId, (EmptyTypeArray, () => getter()));
        _propertySetters.Add(setPropertyId, (new[] { typeof(T) }, value => setter(CastArg<T>(value)!)));
    }

    internal sealed override IGarmObject GetProperty(ulong memberId, string propertyName)
    {
        if (_propertyGetters.TryGetValue(memberId, out var tuple))
        {
            return tuple.getter();
        }
        throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
    }

    internal sealed override IGarmObject SetProperty(ulong memberId, string propertyName, IGarmObject value)
    {
        if (_propertySetters.TryGetValue(memberId, out var tuple))
        {
            tuple.setter(value);
            return DefaultGarm;
        }
        throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
    }

    internal sealed override IGarmObject CallMethod(ulong memberId, string methodName, IGarmObject[]? args)
    {
        var count = args?.Length ?? 0;
        if (_methods.TryGetValue(memberId, out var tuple))
        {
            return tuple.method(args);
        }
        throw CreateMethodNotMatchException(memberId, methodName);
    }

    internal sealed override async Task<IGarmObject> CallMethodAsync(ulong memberId, string methodName, IGarmObject[]? args)
    {
        if (_asyncMethods.TryGetValue(memberId, out var tuple))
        {
            return await tuple.asyncMethod(args).ConfigureAwait(false);
        }
        throw CreateMethodNotMatchException(memberId, methodName);
    }

    private T? CastArg<T>(IGarmObject argModel) => argModel switch
    {
        Garm go => go.ToGeneric<T>(Context.ObjectSerializer).Value,
        Garm<T> go => go.Value,
        _ => throw new InvalidOperationException("不可能进入此分支，因为所有参数都应该是 GARM 对象。"),
    };

    internal override Type[] GetParameterTypes(MemberInvokingType invokingType, ulong memberId)
    {
        if (invokingType is MemberInvokingType.GetProperty)
        {
            return _propertyGetters.TryGetValue(memberId, out var tuple)
                ? tuple.types.ToArray()
                : throw new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }
        if (invokingType is MemberInvokingType.SetProperty)
        {
            return _propertySetters.TryGetValue(memberId, out var tuple)
                ? tuple.types.ToArray()
                : throw new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }
        if (invokingType is MemberInvokingType.Method)
        {
            return _methods.TryGetValue(memberId, out var tuple)
                ? tuple.types.ToArray()
                : throw new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName} 方法，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }
        if (invokingType is MemberInvokingType.AsyncMethod)
        {
            return _asyncMethods.TryGetValue(memberId, out var tuple)
                ? tuple.types.ToArray()
                : throw new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName} 方法，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }
        throw new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName} 成员，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
    }

    private Exception CreateMethodNotMatchException(ulong memberId, string methodName)
    {
        return new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName}.{methodName} 方法，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
    }
}
