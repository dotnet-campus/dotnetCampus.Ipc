using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Contexts;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Utils;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies
{
    /// <summary>
    /// 辅助管理 IPC 远程代理的自动生成的对接类。
    /// </summary>
    public class PublicIpcJointManager
    {
        /// <summary>
        /// 包含所有目前已公开的 IPC 实例。
        /// </summary>
        private readonly ConcurrentDictionary<(string typeFullName, string objectId), GeneratedIpcJoint> _joints = new();
        private readonly GeneratedProxyJointIpcContext _context;
        private readonly IpcContext _ipcContext;

        internal PublicIpcJointManager(GeneratedProxyJointIpcContext context, IpcContext ipcContext)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _ipcContext = ipcContext ?? throw new ArgumentNullException(nameof(ipcContext));
        }

        /// <summary>
        /// 以契约 <typeparamref name="TContract"/> 的方式公开对象 <paramref name="joint"/>，使其可被其他进程发现并使用。
        /// </summary>
        /// <typeparam name="TContract">契约类型。</typeparam>
        /// <param name="ipcObjectId">如果希望同一个契约类型公开多个实例，需用此 Id 加以区分。</param>
        /// <param name="joint">此契约类型的对接类。</param>
        public void AddPublicIpcObject<TContract>(GeneratedIpcJoint<TContract> joint, string? ipcObjectId = "") where TContract : class
        {
            if (!_joints.TryAdd((typeof(TContract).FullName!, ipcObjectId ?? ""), joint))
            {
                throw new InvalidOperationException($"不可重复公开相同契约 {typeof(TContract).FullName} 且相同 Id {ipcObjectId} 的多个 IPC 对接实例。");
            }
        }

        /// <summary>
        /// 以契约 <paramref name="contractType"/> 的方式设置对象 <paramref name="joint"/> 可被其他进程发现并使用。
        /// </summary>
        /// <param name="contractType">契约类型。</param>
        /// <param name="ipcObjectId">如果希望同一个契约类型公开多个实例，需用此 Id 加以区分。</param>
        /// <param name="joint">此契约类型的对接类。</param>
        public void AddPublicIpcObject(Type contractType, GeneratedIpcJoint joint, string? ipcObjectId = "")
        {
            if (!_joints.TryAdd((contractType.FullName!, ipcObjectId ?? ""), joint))
            {
                throw new InvalidOperationException($"不可重复公开相同契约 {contractType.FullName} 且相同 Id {ipcObjectId} 的多个 IPC 对接实例。");
            }
        }

        /// <summary>
        /// 当收到 IPC 消息时调用此方法可以尝试检查此消息是否是来自一个 IPC 类型的代理。如果是，那么会查询本进程中的对接类来对接此代理。
        /// </summary>
        /// <param name="request">IPC 消息的消息体。</param>
        /// <param name="responseTask">IPC 回应消息的消息体，可异步等待。</param>
        /// <returns>
        /// 当此消息来自 IPC 代理，并且本进程存在能对接此代理的类型，则返回 true；否则返回 false。
        /// </returns>
        public bool TryJoint(IIpcRequestContext request, out Task<IIpcResponseMessage> responseTask)
        {
            if (GeneratedProxyMemberInvokeModel.TryDeserialize(request.IpcBufferMessage, out var requestModel)
                && TryFindJoint(requestModel, out var joint)
                && requestModel.MemberName is { } memberName)
            {
                var returnModelTask = InvokeAndReturn(joint, requestModel, request.Peer);
                responseTask = GeneratedIpcJointResponse.FromAsyncReturnModel(returnModelTask)
                    .As<GeneratedIpcJointResponse, IIpcResponseMessage>();
                return true;
            }

            responseTask = Task.FromResult<IIpcResponseMessage>(GeneratedIpcJointResponse.Empty);
            return false;
        }

        internal IEnumerable<string> EnumerateJointNames()
        {
            foreach (var joint in _joints)
            {
                var (typeName, objectId) = joint.Key;
                if (string.IsNullOrEmpty(objectId))
                {
                    yield return typeName;
                }
                else
                {
                    yield return $"{typeName}({objectId})";
                }
            }
        }

        private bool TryFindJoint(GeneratedProxyMemberInvokeModel model, [NotNullWhen(true)] out GeneratedIpcJoint? joint)
        {
            var id = model.Id ?? "";
            if (string.IsNullOrWhiteSpace(model.ContractFullTypeName)
                || string.IsNullOrWhiteSpace(model.MemberName)
                || model.CallType is MemberInvokingType.Unknown)
            {
                joint = null;
                return false;
            }

            if (_joints.TryGetValue((model.ContractFullTypeName, id), out joint))
            {
                return true;
            }
            else
            {
#if DEBUG
                throw new InvalidOperationException($"没有发现针对 {model.ContractFullTypeName} 的对接，无法处理此消息。消息处理丢失可能导致对方端等死，必须调查。");
#else
                joint = null;
                return false;
#endif
            }
        }

        private async Task<GeneratedProxyMemberReturnModel?> InvokeAndReturn(
            GeneratedIpcJoint joint,
            GeneratedProxyMemberInvokeModel requestModel,
            IPeerProxy peer)
        {
            GeneratedProxyMemberReturnModel? @return;
            try
            {
                var parameterTypes = joint.GetParameterTypes(requestModel.CallType, requestModel.MemberId);
                var args = ExtractArgsFromArgsModel(parameterTypes, requestModel.Args, peer);
                var returnValue = await InvokeMember(joint, requestModel.CallType, requestModel.MemberId!, requestModel.MemberName!, args).ConfigureAwait(false);
                @return = CreateReturnModelFromReturnObject(returnValue);
#if DEBUG
                @return.Invoking = requestModel;
#endif
            }
            catch (Exception ex)
            {
                @return = new GeneratedProxyMemberReturnModel(ex);
            }
            return @return;
        }

        /// <summary>
        /// 预处理一组需要进行 IPC 代理访问的参数。
        /// </summary>
        /// <param name="parameterTypes">参数类型列表。</param>
        /// <param name="argModels">参数模型列表。</param>
        /// <param name="peer">如果某个参数的模型表示需要通过代理访问一个 IPC 远端对象，则会用到这个远端。</param>
        /// <returns>参数实例列表。</returns>
        private IGarmObject[]? ExtractArgsFromArgsModel(
            Type[] parameterTypes,
            GeneratedProxyObjectModel?[]? argModels,
            IPeerProxy peer)
        {
            if (argModels is null)
            {
                return null;
            }

            if (argModels.Length is 0)
            {
#if NETCOREAPP3_0_OR_GREATER
                return Array.Empty<IGarmObject>();
#else
                return new IGarmObject[0];
#endif
            }

            var args = new IGarmObject[argModels.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var parameterType = parameterTypes[i];
                var argModel = argModels[i];
                if (argModel is null)
                {
                    // 如果参数模型为 null，那么就是一个 null 参数。
                    args[i] = new Garm((object?) null, parameterType, _context.ObjectSerializer);
                }
                else
                {
                    var ipcType = parameterType;
                    if (_context.TryCreateProxyFromSerializationInfo(peer, ipcType, argModel.Id, out var proxy))
                    {
                        // 如果参数模型表示需要通过代理访问一个 IPC 远端对象，则创建一个代理对象。
                        args[i] = new Garm(proxy, ipcType, _context.ObjectSerializer);
                    }
                    else
                    {
                        // 如果参数模型表示是一个普通的值，则直接使用这个值。
                        args[i] = new Garm(argModel?.Value, parameterType, _context.ObjectSerializer);
                    }
                }
            }
            return args;
        }

        /// <summary>
        /// 处理一个 IPC 对接的返回值。
        /// 额外的，如果其返回的是一个 IPC 公开的类型，则将其加入到新的 IPC 对接管理中。
        /// </summary>
        /// <param name="returnModel">真实的返回值或返回实例。</param>
        /// <returns>可被序列化进行 IPC 传输的返回值模型。</returns>
        private GeneratedProxyMemberReturnModel CreateReturnModelFromReturnObject(IGarmObject returnModel)
        {
            if (_context.TryCreateSerializationInfoFromIpcRealInstance(returnModel, out var objectId, out var ipcTypeFullName))
            {
                return new GeneratedProxyMemberReturnModel
                {
                    Return = new GeneratedProxyObjectModel(_context.ObjectSerializer)
                    {
                        Id = objectId,
                        IpcTypeFullName = ipcTypeFullName,
                    }
                };
            }
            else
            {
                return new GeneratedProxyMemberReturnModel(returnModel.Value, _context.ObjectSerializer);
            }
        }

        private static async Task<IGarmObject> InvokeMember(GeneratedIpcJoint joint, MemberInvokingType callType, ulong memberId, string memberName, IGarmObject[]? args)
        {
            return callType switch
            {
                MemberInvokingType.GetProperty => joint.GetProperty(memberId, memberName),
                MemberInvokingType.SetProperty => joint.SetProperty(memberId, memberName, args?.FirstOrDefault() ?? GarmObjectExtensions.Default),
                MemberInvokingType.Method => joint.CallMethod(memberId, memberName, args),
                MemberInvokingType.AsyncMethod => await joint.CallMethodAsync(memberId, memberName, args).ConfigureAwait(false),
                _ => GarmObjectExtensions.Default,
            };
        }
    }
}
