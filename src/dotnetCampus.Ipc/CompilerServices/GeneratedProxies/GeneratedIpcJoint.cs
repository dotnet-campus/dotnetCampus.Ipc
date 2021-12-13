using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
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
        internal abstract object? GetProperty(string propertyName);
        internal abstract object? SetProperty(string propertyName, object? value);
        internal abstract object? CallMethod(string methodName, object?[]? args);
        internal abstract Task<object?> CallMethodAsync(string methodName, object?[]? args);
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
        private readonly Dictionary<string, Func<object?>> _propertyGetters = new();

        /// <summary>
        /// 设置属性值的方法集合。
        /// </summary>
        private readonly Dictionary<string, Action<object?>> _propertySetters = new();

        /// <summary>
        /// 调用方法的方法集合。
        /// </summary>
        private readonly Dictionary<(string methodName, int parameterCount), Func<object?[]?, object?>> _methods = new();

        /// <summary>
        /// 调用异步方法的方法集合。
        /// </summary>
        private readonly Dictionary<(string methodName, int parameterCount), Func<object?[]?, Task<object?>>> _asyncMethods = new();

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

        protected void MatchMethod(string methodName, Action methodInvoker)
        {
            _methods.Add((methodName, 0), _ =>
            {
                methodInvoker();
                return null;
            });
        }

        protected void MatchMethod(string methodName, Func<Task> methodInvoker)
        {
            _asyncMethods.Add((methodName, 0), async _ =>
            {
                await methodInvoker().ConfigureAwait(false);
                return null;
            });
        }

        protected void MatchMethod<T>(string methodName, Action<T> methodInvoker)
        {
            _methods.Add((methodName, 1), args =>
            {
                methodInvoker(Cast<T>(args![0])!);
                return null;
            });
        }

        protected void MatchMethod<T>(string methodName, Func<T, Task> methodInvoker)
        {
            _asyncMethods.Add((methodName, 1), async args =>
            {
                await methodInvoker(Cast<T>(args![0])!).ConfigureAwait(false);
                return null;
            });
        }

        protected void MatchMethod<T1, T2>(string methodName, Action<T1, T2> methodInvoker)
        {
            _methods.Add((methodName, 2), args =>
            {
                methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!);
                return null;
            });
        }

        protected void MatchMethod<T1, T2>(string methodName, Func<T1, T2, Task> methodInvoker)
        {
            _asyncMethods.Add((methodName, 2), async args =>
            {
                await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!).ConfigureAwait(false);
                return null;
            });
        }

        protected void MatchMethod<T1, T2, T3>(string methodName, Action<T1, T2, T3> methodInvoker)
        {
            _methods.Add((methodName, 3), args =>
            {
                methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!);
                return null;
            });
        }

        protected void MatchMethod<T1, T2, T3>(string methodName, Func<T1, T2, T3, Task> methodInvoker)
        {
            _asyncMethods.Add((methodName, 3), async args =>
            {
                await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!).ConfigureAwait(false);
                return null;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4>(string methodName, Action<T1, T2, T3, T4> methodInvoker)
        {
            _methods.Add((methodName, 4), args =>
            {
                methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!);
                return null;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4>(string methodName, Func<T1, T2, T3, T4, Task> methodInvoker)
        {
            _asyncMethods.Add((methodName, 4), async args =>
            {
                await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!).ConfigureAwait(false);
                return null;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5>(string methodName, Action<T1, T2, T3, T4, T5> methodInvoker)
        {
            _methods.Add((methodName, 5), args =>
            {
                methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!, Cast<T5>(args![4])!);
                return null;
            });
        }

        protected void MatchMethod<T1, T2, T3, T4, T5>(string methodName, Func<T1, T2, T3, T4, T5, Task> methodInvoker)
        {
            _asyncMethods.Add((methodName, 5), async args =>
            {
                await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!, Cast<T5>(args![4])!).ConfigureAwait(false);
                return null;
            });
        }

        protected void MatchMethod<TReturn>(string methodName, Func<TReturn> methodInvoker)
        {
            _methods.Add((methodName, 0), _ => methodInvoker());
        }

        protected void MatchMethod<TReturn>(string methodName, Func<Task<TReturn>> methodInvoker)
        {
            _asyncMethods.Add((methodName, 0), async _ => await methodInvoker().ConfigureAwait(false));
        }

        protected void MatchMethod<T, TReturn>(string methodName, Func<T, TReturn> methodInvoker)
        {
            _methods.Add((methodName, 1), args => methodInvoker(Cast<T>(args![0])!));
        }

        protected void MatchMethod<T, TReturn>(string methodName, Func<T, Task<TReturn>> methodInvoker)
        {
            _asyncMethods.Add((methodName, 1), async args => await methodInvoker(Cast<T>(args![0])!).ConfigureAwait(false));
        }

        protected void MatchMethod<T1, T2, TReturn>(string methodName, Func<T1, T2, TReturn> methodInvoker)
        {
            _methods.Add((methodName, 2), args => methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!));
        }

        protected void MatchMethod<T1, T2, TReturn>(string methodName, Func<T1, T2, Task<TReturn>> methodInvoker)
        {
            _asyncMethods.Add((methodName, 2), async args => await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!).ConfigureAwait(false));
        }

        protected void MatchMethod<T1, T2, T3, TReturn>(string methodName, Func<T1, T2, T3, TReturn> methodInvoker)
        {
            _methods.Add((methodName, 3), args => methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!));
        }

        protected void MatchMethod<T1, T2, T3, TReturn>(string methodName, Func<T1, T2, T3, Task<TReturn>> methodInvoker)
        {
            _asyncMethods.Add((methodName, 3), async args => await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!).ConfigureAwait(false));
        }

        protected void MatchMethod<T1, T2, T3, T4, TReturn>(string methodName, Func<T1, T2, T3, T4, TReturn> methodInvoker)
        {
            _methods.Add((methodName, 4), args => methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!));
        }

        protected void MatchMethod<T1, T2, T3, T4, TReturn>(string methodName, Func<T1, T2, T3, T4, Task<TReturn>> methodInvoker)
        {
            _asyncMethods.Add((methodName, 4), async args => await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!).ConfigureAwait(false));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(string methodName, Func<T1, T2, T3, T4, T5, TReturn> methodInvoker)
        {
            _methods.Add((methodName, 5), args => methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!, Cast<T5>(args![4])!));
        }

        protected void MatchMethod<T1, T2, T3, T4, T5, TReturn>(string methodName, Func<T1, T2, T3, T4, T5, Task<TReturn>> methodInvoker)
        {
            _asyncMethods.Add((methodName, 5), async args => await methodInvoker(Cast<T1>(args![0])!, Cast<T2>(args![1])!, Cast<T3>(args![2])!, Cast<T4>(args![3])!, Cast<T5>(args![4])!).ConfigureAwait(false));
        }

        protected void MatchProperty<T>(string propertyName, Func<T> getter)
        {
            _propertyGetters.Add(propertyName, () => getter());
        }

        protected void MatchProperty<T>(string propertyName, Func<T> getter, Action<T> setter)
        {
            _propertyGetters.Add(propertyName, () => getter());
            _propertySetters.Add(propertyName, value => setter(Cast<T>(value)!));
        }

        internal sealed override object? GetProperty(string propertyName)
        {
            if (_propertyGetters.TryGetValue(propertyName, out var getter))
            {
                return getter();
            }
            throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }

        internal sealed override object? SetProperty(string propertyName, object? value)
        {
            if (_propertySetters.TryGetValue(propertyName, out var setter))
            {
                setter(value);
                return null;
            }
            throw new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{propertyName} 属性，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
        }

        internal sealed override object? CallMethod(string methodName, object?[]? args)
        {
            var count = args is null ? 0 : args.Length;
            if (_methods.TryGetValue((methodName, count), out var method))
            {
                return method(args);
            }
            throw CreateMethodNotMatchException(methodName, count);
        }

        internal sealed override async Task<object?> CallMethodAsync(string methodName, object?[]? args)
        {
            var count = args is null ? 0 : args.Length;
            if (_asyncMethods.TryGetValue((methodName, count), out var asyncMethod))
            {
                return await asyncMethod(args).ConfigureAwait(false);
            }
            throw CreateMethodNotMatchException(methodName, count);
        }

        private T? Cast<T>(object? arg)
        {
            if (arg is JToken jToken)
            {
                return KnownTypeConverter.ConvertBack<T>(jToken);
            }
            return (T?) arg;
        }

        private Exception CreateMethodNotMatchException(string methodName, int count)
        {
            var methodPair = _methods.FirstOrDefault(pair => pair.Key.methodName == methodName);
            var asyncMethodPair = _asyncMethods.FirstOrDefault(pair => pair.Key.methodName == methodName);
            if (methodPair.Key.parameterCount != count)
            {
                return new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{methodName}({count} 个参数) 方法，在 {GetType().FullName} 中能找到的 IPC 对接类中最接近的是 {methodPair.Key.parameterCount} 个参数的重载。");
            }
            else if (asyncMethodPair.Key.parameterCount != count)
            {
                return new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{methodName}({count} 个参数) 方法，在 {GetType().FullName} 中能找到的 IPC 对接类中最接近的是 {asyncMethodPair.Key.parameterCount} 个参数的异步重载。");
            }
            else
            {
                return new NotImplementedException($"无法对接 {typeof(TContract).FullName}.{methodName} 方法，因为没有在 {GetType().FullName} 的 IPC 对接类中进行匹配。");
            }
        }
    }
}
