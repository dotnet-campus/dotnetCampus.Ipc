namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

partial class GeneratedIpcJoint<TContract>
{
    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2>(ulong memberId, Action<T1, T2> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2>(ulong memberId, Func<T1, T2, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, TReturn>(ulong memberId, Func<T1, T2, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, TReturn>(ulong memberId, Func<T1, T2, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3>(ulong memberId, Action<T1, T2, T3> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3>(ulong memberId, Func<T1, T2, T3, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, TReturn>(ulong memberId, Func<T1, T2, T3, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, TReturn>(ulong memberId, Func<T1, T2, T3, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4>(ulong memberId, Action<T1, T2, T3, T4> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4>(ulong memberId, Func<T1, T2, T3, T4, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, TReturn>(ulong memberId, Func<T1, T2, T3, T4, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, TReturn>(ulong memberId, Func<T1, T2, T3, T4, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5>(ulong memberId, Action<T1, T2, T3, T4, T5> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5>(ulong memberId, Func<T1, T2, T3, T4, T5, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6>(ulong memberId, Action<T1, T2, T3, T4, T5, T6> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10)], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10)], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10)], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10)], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11),
                ], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11),
                ], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11),
        ], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11),
        ], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12),
                ], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12),
                ], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12),
        ], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12),
        ], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
                ], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
                ], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
        ], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13),
        ], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14),
                ], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14),
                ], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14),
        ], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14),
        ], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15),
                ], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15),
                ], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15),
        ], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15),
        ], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!).ConfigureAwait(false)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(ulong memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16),
                ], args =>
        {
            methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!, CastArg<T16>(args[15])!);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16),
                ], async args =>
        {
            await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!, CastArg<T16>(args[15])!).ConfigureAwait(false);
            return DefaultGarm;
        }
        ));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Garm<TReturn>> methodInvoker)
    {
        _methods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16),
        ], args => methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!, CastArg<T16>(args[15])!)));
    }

    /// <summary>
    /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
    /// </summary>
    /// <param name="memberId">方法签名的 Id。</param>
    /// <param name="methodInvoker">对接实现。</param>
    protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, TReturn>(ulong memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, T11, T12, T13, T14, T15, T16, Task<Garm<TReturn>>> methodInvoker)
    {
        _asyncMethods.Add(memberId, ([typeof(T1), typeof(T2), typeof(T3), typeof(T4), typeof(T5), typeof(T6), typeof(T7), typeof(T8), typeof(T9), typeof(T10), typeof(T11), typeof(T12), typeof(T13), typeof(T14), typeof(T15), typeof(T16),
        ], async args => await methodInvoker(CastArg<T1>(args[0])!, CastArg<T2>(args[1])!, CastArg<T3>(args[2])!, CastArg<T4>(args[3])!, CastArg<T5>(args[4])!, CastArg<T6>(args[5])!, CastArg<T7>(args[6])!, CastArg<T8>(args[7])!, CastArg<T9>(args[8])!, CastArg<T10>(args[9])!, CastArg<T11>(args[10])!, CastArg<T12>(args[11])!, CastArg<T13>(args[12])!, CastArg<T14>(args[13])!, CastArg<T15>(args[14])!, CastArg<T16>(args[15])!).ConfigureAwait(false)));
    }
}
