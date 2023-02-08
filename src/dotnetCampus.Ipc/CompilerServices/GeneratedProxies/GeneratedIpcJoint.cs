using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
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
        internal abstract Garm<object?> GetProperty(string memberId, string propertyName);
        internal abstract Garm<object?> SetProperty(string memberId, string propertyName, object? value);
        internal abstract Garm<object?> CallMethod(string memberId, string methodName, object?[]? args);
        internal abstract Task<Garm<object?>> CallMethodAsync(string memberId, string methodName, object?[]? args);
    }

    /// <summary>
    /// 为自动生成的 IPC 对接类提供基类。
    /// </summary>
    /// <typeparam name="TContract">应该对接的契约类型（必须是一个接口）。</typeparam>
    public abstract class GeneratedIpcJoint<TContract> : GeneratedIpcJoint where TContract : class
    {
        /// <summary>
        /// 获取属性值的方法集合。
        /// </summary>
        private readonly Dictionary<string, Func<Garm<object?>>> _propertyGetters = new();

        /// <summary>
        /// 设置属性值的方法集合（Key 为 MemberId，用于标识一个接口内的唯一一个成员，其中属性的 get 和 set 分别是两个不同的成员）。
        /// </summary>
        private readonly Dictionary<string, Action<object?>> _propertySetters = new();

        /// <summary>
        /// 调用方法的方法集合（Key 为 MemberId，用于标识一个接口内的唯一一个成员，其中属性的 get 和 set 分别是两个不同的成员）。
        /// </summary>
        private readonly Dictionary<string, Func<object?[]?, Garm<object?>>> _methods = new();

        /// <summary>
        /// 调用异步方法的方法集合。
        /// </summary>
        private readonly Dictionary<string, Func<object?[]?, Task<Garm<object?>>>> _asyncMethods = new();

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

        protected void MatchMethod(string memberId, Action methodInvoker)
        {
            _methods.Add(memberId, _ =>
            {
                methodInvoker();
                return default;
            });
        }

        protected void MatchMethod(string memberId, Func<Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async _ =>
            {
                await methodInvoker().ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T>(string memberId, Action<T> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T>(args![0])!);
                return default;
            });
        }

        protected void MatchMethod<T>(string memberId, Func<T, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T>(args![0])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T1, T2>(string memberId, Action<T1, T2> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!);
                return default;
            });
        }

        protected void MatchMethod<T1, T2>(string memberId, Func<T1, T2, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3>(string memberId, Action<T1, T2, T3> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3>(string memberId, Func<T1, T2, T3, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4>(string memberId, Action<T1, T2, T3, T4> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4>(string memberId, Func<T1, T2, T3, T4, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5>(string memberId, Action<T1, T2, T3, T4, T5> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5>(string memberId, Func<T1, T2, T3, T4, T5, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6>(string memberId, Action<T1, T2, T3, T4, T5, T6> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6>(string memberId, Func<T1, T2, T3, T4, T5, T6, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7>(string memberId, Action<T1, T2, T3, T4, T5, T6, T7> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7>(string memberId, Func<T1, T2, T3, T4, T5, T6, T7, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8>(string memberId, Action<T1, T2, T3, T4, T5, T6, T7, T8> methodInvoker)
        {
            _methods.Add(memberId, args =>
            {
                methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!);
                return default;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8>(string memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args =>
            {
                await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!).ConfigureAwait(false);
                return default;
            });
        }

        protected void MatchMethod<TReturn>(string memberId, Func<Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, _ => CastReturn(methodInvoker()));
        }

        protected void MatchMethod<TReturn>(string memberId, Func<Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async _ => CastReturn(await methodInvoker().ConfigureAwait(false)));
        }

        protected void MatchMethod<T, TReturn>(string memberId, Func<T, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T>(args![0])!)));
        }

        protected void MatchMethod<T, TReturn>(string memberId, Func<T, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T>(args![0])!).ConfigureAwait(false)));
        }

        protected void MatchMethod<T1, T2, TReturn>(string memberId, Func<T1, T2, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!)));
        }

        protected void MatchMethod<T1, T2, TReturn>(string memberId, Func<T1, T2, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!).ConfigureAwait(false)));
        }

        protected void MatchMethod<T1, T2, T3, TReturn>(string memberId, Func<T1, T2, T3, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!)));
        }

        protected void MatchMethod<T1, T2, T3, TReturn>(string memberId, Func<T1, T2, T3, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!).ConfigureAwait(false)));
        }

        protected void MatchMethod<T1, T2, T3, T4, TReturn>(string memberId, Func<T1, T2, T3, T4, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!)));
        }

        protected void MatchMethod<T1, T2, T3, T4, TReturn>(string memberId, Func<T1, T2, T3, T4, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!).ConfigureAwait(false)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!).ConfigureAwait(false)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, T6, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, T6, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!).ConfigureAwait(false)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, T6, T7, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, T6, T7, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!).ConfigureAwait(false)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, args => CastReturn(methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!)));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, T6, T7, T8, TReturn>(string memberId, Func<T1, T2, T3, T4, T5, T6, T7, T8, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, async args => CastReturn(await methodInvoker(CastArg<T1>(args![0])!, CastArg<T2>(args![1])!, CastArg<T3>(args![2])!, CastArg<T4>(args![3])!, CastArg<T5>(args![4])!, CastArg<T6>(args![5])!, CastArg<T7>(args![6])!, CastArg<T8>(args![7])!).ConfigureAwait(false)));
        }

        protected void MatchProperty<T>(string getPropertyId, Func<Garm<T>> getter)
        {
            _propertyGetters.Add(getPropertyId, () => CastReturn(getter()));
        }

        protected void MatchProperty<T>(string getPropertyId, string setPropertyId, Func<Garm<T>> getter, Action<T> setter)
        {
            _propertyGetters.Add(getPropertyId, () => CastReturn(getter()));
            _propertySetters.Add(setPropertyId, value => setter(CastArg<T>(value)!));
        }

        internal sealed override Garm<object?> GetProperty(string memberId, string propertyName)
        {
            if (_propertyGetters.TryGetValue(memberId, out var getter))
            {
                return getter();
            }
            throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }

        internal sealed override Garm<object?> SetProperty(string memberId, string propertyName, object? value)
        {
            if (_propertySetters.TryGetValue(memberId, out var setter))
            {
                setter(value);
                return default;
            }
            throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }

        internal sealed override Garm<object?> CallMethod(string memberId, string methodName, object?[]? args)
        {
            var count = args is null ? 0 : args.Length;
            if (_methods.TryGetValue(memberId, out var method))
            {
                return method(args);
            }
            throw CreateMethodNotMatchException(memberId, methodName);
        }

        internal sealed override async Task<Garm<object?>> CallMethodAsync(string memberId, string methodName, object?[]? args)
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

        private Garm<object?> CastReturn<T>(Garm<T> argModel)
        {
            return new Garm<object?>(argModel.Value, argModel.IpcType);
        }

        private Exception CreateMethodNotMatchException(string memberId, string methodName)
        {
            return new NotImplementedException($"无法对接 Id 为 {memberId} 的 {typeof(TContract).FullName}.{methodName} 方法，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }
    }
}
