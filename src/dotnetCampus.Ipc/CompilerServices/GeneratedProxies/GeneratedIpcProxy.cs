using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Messages;

using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// 提供给自动生成的代理对象使用，以便能够生成通过 IPC 方式访问目标成员的能力。
    /// </summary>
    public abstract class GeneratedIpcProxy
    {
        private GeneratedProxyJointIpcContext? _context;
        private string? _typeName;

        /// <summary>
        /// 提供基于 .NET 类型的 IPC 传输上下文信息。
        /// </summary>
        internal GeneratedProxyJointIpcContext Context
        {
            get => _context ?? throw new IpcLocalException($"基于 .NET 类型的 IPC 传输机制应使用 {typeof(GeneratedIpcFactory)} 工厂类型来构造。");
            set => _context = value ?? throw new ArgumentNullException(nameof(value));
        }

        /// <summary>
        /// 获取或设置目标 IPC 节点。
        /// 如果设定为 null，则所有请求将返回默认值。（未来可能运行在抛出异常和返回默认值之间进行选择。）
        /// </summary>
        public IPeerProxy? PeerProxy { get; set; }

        /// <summary>
        /// 如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。
        /// </summary>
        public string TypeName
        {
            get => _typeName ?? throw new IpcLocalException($"基于 .NET 类型的 IPC 传输机制应使用 {typeof(GeneratedIpcFactory)} 工厂类型来构造。");
            internal set => _typeName = value;
        }

        /// <summary>
        /// 如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。
        /// </summary>
        public string? ObjectId { get; set; }
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

        /// <summary>
        /// 通过 IPC 访问目标对象上某属性的值。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>可异步等待的属性的值。</returns>
        protected async Task<T?> GetValueAsync<T>([CallerMemberName] string propertyName = "")
        {
            return await IpcInvokeAsync<T>(MemberInvokingType.GetProperty, propertyName, null).ConfigureAwait(false);
        }

        /// <summary>
        /// 通过 IPC 访问目标对象上某标记了 <see cref="IpcReadonlyAttribute"/> 的属性。如果曾访问过，会将这个值缓存下来供下次无 IPC 访问。
        /// 当发生并发时，可能导致多次通过 IPC 访问此属性的值，但此方法依然是线程安全的。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>可异步等待的属性的值。</returns>
        protected async Task<T?> GetReadonlyValueAsync<T>([CallerMemberName] string propertyName = "")
        {
            if (_readonlyPropertyValues.TryGetValue(propertyName, out var cachedValue))
            {
                // 当只读字典中存在此属性的缓存时，直接取缓存。
                return (T?) cachedValue;
            }
            // 否则，通过 IPC 访问获取此属性的值后设入缓存。（这里可能存在并发情况，会导致浪费的 IPC 访问，但能确保数据一致性）。
            var value = await IpcInvokeAsync<T>(MemberInvokingType.GetProperty, propertyName, null).ConfigureAwait(false);
            _readonlyPropertyValues.TryAdd(propertyName, value);
            return value;
        }

        /// <summary>
        /// 通过 IPC 设置目标对象上某属性的值。
        /// </summary>
        /// <typeparam name="T">属性类型。</typeparam>
        /// <param name="value">要设置的属性的值。</param>
        /// <param name="propertyName">属性名称。</param>
        /// <returns>可异步等待的属性设置。</returns>
        protected async Task SetValueAsync<T>(T value, [CallerMemberName] string propertyName = "")
        {
            await IpcInvokeAsync<object>(MemberInvokingType.SetProperty, propertyName, new object?[] { value }).ConfigureAwait(false);
        }

        protected async Task CallMethod([CallerMemberName] string methodName = "")
        {
            await IpcInvokeAsync<object>(MemberInvokingType.Method, methodName, new object?[0]).ConfigureAwait(false);
        }

        protected async Task<T?> CallMethod<T>([CallerMemberName] string methodName = "")
        {
            return await IpcInvokeAsync<T>(MemberInvokingType.Method, methodName, new object?[0]).ConfigureAwait(false);
        }

        protected async Task CallMethod(object?[]? args, [CallerMemberName] string methodName = "")
        {
            await IpcInvokeAsync<object>(MemberInvokingType.Method, methodName, args).ConfigureAwait(false);
        }

        protected async Task<T?> CallMethod<T>(object?[]? args, [CallerMemberName] string methodName = "")
        {
            return await IpcInvokeAsync<T>(MemberInvokingType.Method, methodName, args).ConfigureAwait(false);
        }

        protected async Task CallMethodAsync([CallerMemberName] string methodName = "")
        {
            await IpcInvokeAsync<object>(MemberInvokingType.AsyncMethod, methodName, new object?[0]).ConfigureAwait(false);
        }

        protected async Task<T?> CallMethodAsync<T>([CallerMemberName] string methodName = "")
        {
            return await IpcInvokeAsync<T>(MemberInvokingType.AsyncMethod, methodName, new object?[0]).ConfigureAwait(false);
        }

        protected async Task CallMethodAsync(object?[]? args, [CallerMemberName] string methodName = "")
        {
            await IpcInvokeAsync<object>(MemberInvokingType.AsyncMethod, methodName, args).ConfigureAwait(false);
        }

        protected async Task<T?> CallMethodAsync<T>(object?[]? args, [CallerMemberName] string methodName = "")
        {
            return await IpcInvokeAsync<T>(MemberInvokingType.AsyncMethod, methodName, args).ConfigureAwait(false);
        }

        private async Task<T?> IpcInvokeAsync<T>(MemberInvokingType callType, string memberName, object?[]? args)
        {
            if (PeerProxy is null)
            {
                return default;
            }

            var returnModel = await IpcInvokeAsync(new GeneratedProxyMemberInvokeModel
            {
                Id = ObjectId,
                ContractFullTypeName = TypeName,
                CallType = callType,
                MemberName = memberName,
                Args = args?.Select(SerializeArg).ToArray(),
            }).ConfigureAwait(false);

            if (returnModel is null)
            {
                // 如果远端返回 null，则本地代理返回 null。
                return default;
            }

            if (returnModel.Exception is { } exceptionModel)
            {
                // 如果远端抛出了异常，则本地代理抛出相同的异常。
                exceptionModel.Throw();
            }

            if (returnModel.Return is { } model
                && Context.TryCreateProxyFromSerializationInfo(PeerProxy,
                    model.AssemblyQualifiedName, model.Id, out var proxyInstance))
            {
                // 如果远端返回 IPC 公开的对象，则本地获取此对象的代理并返回。
                return (T) proxyInstance;
            }

            // 其他情况直接使用反序列化的值返回。
            return Cast<T>(returnModel.Return?.Value);
        }

        private async Task<GeneratedProxyMemberReturnModel?> IpcInvokeAsync(GeneratedProxyMemberInvokeModel model)
        {
            if (PeerProxy is null)
            {
                return null;
            }

            var requestMessage = GeneratedProxyMemberInvokeModel.Serialize(model);
            requestMessage = new IpcMessage(requestMessage.Tag, requestMessage.Body, CoreMessageType.JsonObject);
            var responseMessage = await PeerProxy.GetResponseAsync(requestMessage).ConfigureAwait(false);
            if (GeneratedProxyMemberReturnModel.TryDeserialize(responseMessage, out var returnModel))
            {
                return returnModel;
            }
            else
            {
                throw new NotSupportedException("请谨慎对待此异常！无法处理 IPC 代理调用的返回值。");
            }
        }

        private T? Cast<T>(object? arg)
        {
            if (arg is JValue jValue)
            {
                return KnownTypeConverter.ConvertBack<T>(jValue);
            }
            return (T?) arg;
        }

        private GeneratedProxyObjectModel? SerializeArg(object? arg)
        {
            if (PeerProxy is null)
            {
                return null;
            }

            if (Context.TryCreateSerializationInfoFromIpcRealInstance(arg, out var objectId, out var assemblyQualifiedName))
            {
                // 如果此参数是一个 IPC 对象。
                return new GeneratedProxyObjectModel
                {
                    Id = objectId,
                    AssemblyQualifiedName = assemblyQualifiedName,
                };
            }
            else
            {
                // 如果此参数只是一个普通对象。
                return new GeneratedProxyObjectModel
                {
                    Value = KnownTypeConverter.Convert(arg),
                };
            }
        }
    }
}
