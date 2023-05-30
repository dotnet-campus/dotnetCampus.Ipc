using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
/// <summary>
/// 为自动生成的 IPC 对接类提供基类。
/// </summary>
public abstract class GeneratedIpcJoint
{
    /// <summary>
    /// 设置此对接对象的真实实例。
    /// </summary>
    /// <param name="realInstance">真实实例。</param>
    internal abstract void SetInstance(object realInstance);
    internal abstract IGarmObject GetProperty(ulong memberId, string propertyName);
    internal abstract IGarmObject SetProperty(ulong memberId, string propertyName, IGarmObject value);
    internal abstract IGarmObject CallMethod(ulong memberId, string methodName, IGarmObject[]? args);
    internal abstract Task<IGarmObject> CallMethodAsync(ulong memberId, string methodName, IGarmObject[]? args);
}

/// <summary>
/// 为自动生成的 IPC 对接类提供基类。
/// </summary>
/// <typeparam name="TContract">应该对接的契约类型（必须是一个接口）。</typeparam>
public abstract class GeneratedIpcJoint<TContract> : GeneratedIpcJoint where TContract : class
{
    /// <summary>
    /// 获取默认的 GARM 对象。
    /// </summary>
    private static IGarmObject DefaultGarm =>
#if NET6_0_OR_GREATER
        IGarmObject.Default;
#else
        GarmObjectExtensions.Default;
#endif

    /// <summary>
    /// 获取属性值的方法集合。
    /// </summary>
    private readonly Dictionary<ulong, Func<IGarmObject>> _propertyGetters = new();

    /// <summary>
    /// 设置属性值的方法集合（Key 为 MemberId，用于标识一个接口内的唯一一个成员，其中属性的 get 和 set 分别是两个不同的成员）。
    /// </summary>
    private readonly Dictionary<ulong, Action<IGarmObject>> _propertySetters = new();

    /// <summary>
    /// 调用方法的方法集合（Key 为 MemberId，用于标识一个接口内的唯一一个成员，其中属性的 get 和 set 分别是两个不同的成员）。
    /// </summary>
    private readonly Dictionary<ulong, Func<IGarmObject[]?, IGarmObject>> _methods = new();

    /// <summary>
    /// 调用异步方法的方法集合。
    /// </summary>
    private readonly Dictionary<ulong, Func<IGarmObject[]?, Task<IGarmObject>>> _asyncMethods = new();

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
        _methods.Add(memberId, _ =>
        {
            methodInvoker();
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod(ulong memberId, Func<Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async _ =>
        {
            await methodInvoker().ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T>(ulong memberId, Action<T> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T>(args![0])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T>(ulong memberId, Func<T, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T>(args![0])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2>(ulong memberId, Action<T1, T2> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2>(ulong memberId, Func<T1, T2, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3>(ulong memberId, Action<T1, T2, T3> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3>(ulong memberId, Func<T1, T2, T3, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4>(ulong memberId, Action<T1, T2, T3, T4> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4>(ulong memberId, Func<T1, T2, T3, T4, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5>(ulong memberId, Action<T1, T2, T3, T4, T5> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5>(ulong memberId, Func<T1, T2, T3, T4, T5, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6>(ulong memberId, Action<T1, T2, T3, T4, T5, T6> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8> methodInvoker)
    {
        _methods.Add(memberId, args =>
        {
            methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args =>
        {
            await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!).ConfigureAwait(false);
            return DefaultGarm;
        });
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<TReturn>(ulong memberId, Func<Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, _ => methodInvoker());
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<TReturn>(ulong memberId, Func<Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async _ => await methodInvoker().ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T, TReturn>(ulong memberId, Func<T, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T>(args![0])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T, TReturn>(ulong memberId, Func<T, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T>(args![0])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, TReturn>(ulong memberId, Func<T1, T2, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, TReturn>(ulong memberId, Func<T1, T2, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, TReturn>(ulong memberId, Func<T1, T2, T3, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, TReturn>(ulong memberId, Func<T1, T2, T3, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, TReturn>(ulong memberId, Func<T1, T2, T3, T4, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, TReturn>(ulong memberId, Func<T1, T2, T3, T4, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, args => methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, async args => await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!).ConfigureAwait(false));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个属性，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="getter"/> 所指向的具体实现。
    /// </summary>
    /// <param name="getPropertyId">属性 get 的签名的 Id。</param>
    /// <param name="getter">get 的对接实现。</param>
    protected void MatchProperty<T>(ulong getPropertyId, Func<Garm<T>> getter)
    {
        _propertyGetters.Add(getPropertyId, () => getter());
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
        _propertyGetters.Add(getPropertyId, () => getter());
        _propertySetters.Add(setPropertyId, value => setter(CastArg<T>(value)!));
    }

    internal sealed override IGarmObject GetProperty(ulong memberId, string propertyName)
    {
        if (_propertyGetters.TryGetValue(memberId, out var getter))
        {
            return getter();
        }
        throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
    }

    internal sealed override IGarmObject SetProperty(ulong memberId, string propertyName, IGarmObject value)
    {
        if (_propertySetters.TryGetValue(memberId, out var setter))
        {
            setter(value);
            return DefaultGarm;
        }
        throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
    }

    internal sealed override IGarmObject CallMethod(ulong memberId, string methodName, IGarmObject[]? args)
    {
        var count = args is null ? 0 : args.Length;
        if (_methods.TryGetValue(memberId, out var method))
        {
            return method(args);
        }
        throw CreateMethodNotMatchException(memberId, methodName);
    }

    internal sealed override async Task<IGarmObject> CallMethodAsync(ulong memberId, string methodName, IGarmObject[]? args)
    {
        var count = args is null ? 0 : args.Length;
        if (_asyncMethods.TryGetValue(memberId, out var asyncMethod))
        {
            return await asyncMethod(args).ConfigureAwait(false);
        }
        throw CreateMethodNotMatchException(memberId, methodName);
    }

    private T? CastArg<T>(object? argModel)
    {
        if (argModel is JToken jToken)
        {
            return KnownTypeConverter.ConvertBack<T>(jToken);
        }
        return (T?) argModel;
    }

    private Exception CreateMethodNotMatchException(ulong memberId, string methodName)
    {
        return new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName}.{methodName} 方法，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
    }
}
