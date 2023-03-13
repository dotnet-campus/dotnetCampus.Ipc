using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Utils;
using dotnetCampus.Ipc.Exceptions;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// 提供给自动生成的代理对象使用，以便能够生成通过 IPC 方式访问目标成员的能力。
    /// </summary>
    public abstract partial class GeneratedIpcProxy
    {
        /// <summary>
        /// 为库内派生类提供 IPC 代理调用的辅助方法。
        /// </summary>
        private protected IpcProxyInvokingHelper Invoker { get; } = new IpcProxyInvokingHelper();

        /// <summary>
        /// 提供基于 .NET 类型的 IPC 传输上下文信息。
        /// </summary>
        public GeneratedProxyJointIpcContext Context
        {
            get => Invoker.Context;
            internal set => Invoker.Context = value;
        }

        /// <summary>
        /// 获取或设置目标 IPC 节点。
        /// 如果设定为 null，则所有请求将返回默认值。（未来可能运行在抛出异常和返回默认值之间进行选择。）
        /// </summary>
        public IPeerProxy? PeerProxy
        {
            get => Invoker.PeerProxy;
            internal set => Invoker.PeerProxy = value;
        }

        /// <summary>
        /// 如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。
        /// </summary>
        public string TypeName
        {
            get => Invoker.TypeName;
            internal set => Invoker.TypeName = value;
        }

        /// <summary>
        /// 如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。
        /// </summary>
        public string? ObjectId
        {
            get => Invoker.ObjectId;
            internal set => Invoker.ObjectId = value;
        }

        /// <summary>
        /// 除了编译时确定的 IPC 代理访问配置之外，还可以额外指定一个运行时的配置。优先级最低，在编译时配置没有设的时候使用。
        /// </summary>
        internal IpcProxyConfigs? RuntimeConfigs { get; set; }


        /// <summary>
        /// 远端名称
        /// </summary>
        internal string PeerName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        internal IIpcProvider InnerIpcProvider { get; set; }
    }

    /// <summary>
    /// 提供给自动生成的代理对象使用，以便能够生成通过 IPC 方式访问目标成员的能力。
    /// </summary>
    public abstract class GeneratedIpcProxy<TContract> : GeneratedIpcProxy where TContract : class
    {
        private readonly ConcurrentDictionary<string, object?> _readonlyPropertyValues = new(StringComparer.Ordinal);

        /// <summary>
        /// 创建 <typeparamref name="TContract"/> 类型的 IPC 代理对象。
        /// </summary>
        protected GeneratedIpcProxy()
        {
            TypeName = typeof(TContract).FullName!;
        }


        private void ChackPeerProxy()
        {
            if (PeerProxy is null)
            {
                if (InnerIpcProvider is dotnetCampus.Ipc.Pipes.IpcProvider provider)
                {
                    if (provider.PeerManager.TryGetValue(PeerName, out var peerProxy))
                    {
                        PeerProxy = peerProxy;
                    }
                }
            }

        }

        /// <summary>
        /// 通过 IPC 访问目标对象上某属性的值。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="namedValues">包含属性上标记的调用此 IPC 属性的个性化方式。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>可异步等待的属性的值。</returns>
        protected async Task<T?> GetValueAsync<T>(ulong memberId, IpcProxyMemberNamedValues namedValues, [CallerMemberName] string propertyName = "")
        {
            if (namedValues.IsReadonly ?? false)
            {
                // 通过 IPC 访问目标对象上标记了 IpcPropertyAttribute 的属性。如果曾访问过，会将这个值缓存下来供下次无 IPC 访问。
                // 当发生并发时，可能导致多次通过 IPC 访问此属性的值，但此方法依然是线程安全的。
                if (_readonlyPropertyValues.TryGetValue(propertyName, out var cachedValue))
                {
                    // 当只读字典中存在此属性的缓存时，直接取缓存。
                    return (T?) cachedValue;
                }
                // 否则，通过 IPC 访问获取此属性的值后设入缓存。（这里可能存在并发情况，会导致浪费的 IPC 访问，但能确保数据一致性）。
                var value = await IpcInvokeAsync<T>(MemberInvokingType.GetProperty, memberId, propertyName, null, namedValues).ConfigureAwait(false);
                _readonlyPropertyValues.TryAdd(propertyName, value);
                return value;
            }
            else
            {
                return await IpcInvokeAsync<T>(MemberInvokingType.GetProperty, memberId, propertyName, null, namedValues).ConfigureAwait(false);
            }
        }

        /// <summary>
        /// 通过 IPC 设置目标对象上某属性的值。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="value">要设置的属性的值。</param>
        /// <param name="namedValues">包含属性上标记的调用此 IPC 属性的个性化方式。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>可异步等待的属性设置。</returns>
        protected Task SetValueAsync<T>(ulong memberId, Garm<T> value, IpcProxyMemberNamedValues namedValues, [CallerMemberName] string propertyName = "")
        {
            return IpcInvokeAsync<object>(MemberInvokingType.SetProperty, memberId, propertyName, new Garm<object?>[] { CastArg(value) }, namedValues);
        }

        /// <summary>
        /// 通过 IPC 调用目标对象上的某个方法。
        /// </summary>
        /// <param name="args">方法参数列表。</param>
        /// <param name="namedValues">包含方法上标记的调用此 IPC 方法的个性化方式。</param>
        /// <param name="methodName">方法名。</param>
        /// <returns>可异步等待方法返回值的可等待对象。</returns>
        protected Task CallMethod(ulong memberId, Garm<object?>[]? args, IpcProxyMemberNamedValues namedValues, [CallerMemberName] string methodName = "")
        {
            return IpcInvokeAsync<object>(MemberInvokingType.Method, memberId, methodName, args, namedValues);
        }

        /// <summary>
        /// 通过 IPC 调用目标对象上的某个方法。
        /// </summary>
        /// <param name="args">方法参数列表。</param>
        /// <param name="namedValues">包含方法上标记的调用此 IPC 方法的个性化方式。</param>
        /// <param name="methodName">方法名。</param>
        /// <returns>可异步等待方法返回值的可等待对象。</returns>
        protected Task<T?> CallMethod<T>(ulong memberId, Garm<object?>[]? args, IpcProxyMemberNamedValues namedValues, [CallerMemberName] string methodName = "")
        {
            return IpcInvokeAsync<T>(MemberInvokingType.Method, memberId, methodName, args, namedValues);
        }

        /// <summary>
        /// 通过 IPC 调用目标对象上的某个异步方法。
        /// </summary>
        /// <param name="args">方法参数列表。</param>
        /// <param name="namedValues">包含方法上标记的调用此 IPC 方法的个性化方式。</param>
        /// <param name="methodName">方法名。</param>
        /// <returns>可异步等待方法返回值的可等待对象。</returns>
        protected Task CallMethodAsync(ulong memberId, Garm<object?>[]? args, IpcProxyMemberNamedValues namedValues, [CallerMemberName] string methodName = "")
        {
            return IpcInvokeAsync<object>(MemberInvokingType.AsyncMethod, memberId, methodName, args, namedValues);
        }

        /// <summary>
        /// 通过 IPC 调用目标对象上的某个异步方法。
        /// </summary>
        /// <param name="args">方法参数列表。</param>
        /// <param name="namedValues">包含方法上标记的调用此 IPC 方法的个性化方式。</param>
        /// <param name="methodName">方法名。</param>
        /// <returns>可异步等待方法返回值的可等待对象。</returns>
        protected Task<T?> CallMethodAsync<T>(ulong memberId, Garm<object?>[]? args, IpcProxyMemberNamedValues namedValues, [CallerMemberName] string methodName = "")
        {
            return IpcInvokeAsync<T>(MemberInvokingType.AsyncMethod, memberId, methodName, args, namedValues);
        }

        /// <summary>
        /// 在 <see cref="IpcMethodAttribute"/> 和 <see cref="IpcPropertyAttribute"/> 中标记的访问属性和调用方法相关的个性化方式，大多数将在此方法中被实践，少数在自动生成代理类时被实践。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="callType">调用类型（属性还是方法）。</param>
        /// <param name="memberName">成员名。</param>
        /// <param name="args">调用参数。</param>
        /// <param name="namedValues">包含属性上标记的调用此 IPC 成员的个性化方式。</param>
        /// <returns>可异步等待方法返回值的可等待对象。</returns>
        private async Task<T?> IpcInvokeAsync<T>(MemberInvokingType callType, ulong memberId, string memberName, Garm<object?>[]? args, IpcProxyMemberNamedValues namedValues)
        {
            ChackPeerProxy();
            var ignoresIpcException = namedValues.IgnoresIpcException ?? RuntimeConfigs?.IgnoresIpcException ?? false;
            try
            {
                return (namedValues.Timeout ?? RuntimeConfigs?.Timeout) is int timeout && timeout > 0
                    ? await InvokeWithTimeoutAsync<T>(
                        callType,
                        memberId,
                        memberName,
                        args,
                        timeout,
                        ignoresIpcException,
                        namedValues.DefaultReturn).ConfigureAwait(false)
                    : await Invoker.IpcInvokeAsync<T>(
                        callType,
                        memberId,
                        memberName,
                        args).ConfigureAwait(false);
            }
            catch (IpcRemoteException) when (ignoresIpcException)
            {
                // 如果目标要求忽略异常，则返回指定值或默认值。
                return namedValues.DefaultReturn is { } defaultReturn ? (T) defaultReturn : default;
            }
            catch (AggregateException ex) when (ex.InnerExceptions.Count >= 1)
            {
                var innerException = ex.Flatten().InnerExceptions[0];
                if (innerException is IpcRemoteException)
                {
                    // 如果目标要求忽略异常，则返回指定值或默认值。
                    return namedValues.DefaultReturn is { } defaultReturn ? (T) defaultReturn : default;
                }
                else
                {
                    // 尽量不抛出难以捕获的 AggregateException 而改用内部异常。
                    ExceptionDispatchInfo.Capture(innerException).Throw();
                    throw;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        private async Task<T?> InvokeWithTimeoutAsync<T>(MemberInvokingType callType, ulong memberId, string memberName, Garm<object?>[]? args,
            int millisecondsTimeout, bool ignoreException, object? defaultReturn)
        {
            var ipcTask = Invoker.IpcInvokeAsync<T>(callType, memberId, memberName, args);
            var timeoutTask = Task.Delay(millisecondsTimeout);
            var task = await Task.WhenAny(ipcTask, timeoutTask).ConfigureAwait(false);
            if (task == ipcTask)
            {
                // 任务正常完成。
                return ipcTask.Result;
            }
            else if (ignoreException)
            {
                // 任务超时（不抛异常）。
                IgnoreTaskExceptionsAsync(ipcTask);
                return defaultReturn is null ? default : (T) defaultReturn;
            }
            else
            {
                // 任务超时（抛异常）。
                IgnoreTaskExceptionsAsync(ipcTask);
                throw new IpcInvokingTimeoutException(memberName, TimeSpan.FromMilliseconds(millisecondsTimeout));
            }
        }

        private Garm<object?> CastArg<T>(Garm<T> argModel)
        {
            return new Garm<object?>(argModel.Value, argModel.IpcType);
        }

        /// <summary>
        /// 吞掉业务上已不再需要返回值的异常，防止异常泄漏到后台线程或 Task 中。
        /// </summary>
        /// <param name="task">要吞掉异常的任务。</param>
        private async void IgnoreTaskExceptionsAsync(Task task)
        {
            try
            {
                await task;
            }
            catch
            {
                // 吞掉业务上已不再需要返回值的异常，防止异常泄漏到后台线程或 Task 中。
            }
        }
    }
}
