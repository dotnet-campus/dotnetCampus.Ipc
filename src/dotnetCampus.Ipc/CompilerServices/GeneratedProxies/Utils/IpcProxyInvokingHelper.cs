using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Serialization;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Utils;

internal class IpcProxyInvokingHelper
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
    internal IPeerProxy? PeerProxy { get; set; }

    /// <summary>
    /// 如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。
    /// </summary>
    internal string TypeName
    {
        get => _typeName ?? throw new IpcLocalException($"基于 .NET 类型的 IPC 传输机制应使用 {typeof(GeneratedIpcFactory)} 工厂类型来构造。");
        set => _typeName = value;
    }

    /// <summary>
    /// 如果要调用的远端对象有多个实例，请设置此 Id 值以找到期望的实例。
    /// </summary>
    internal string? ObjectId { get; set; }

    internal async Task<T?> IpcInvokeAsync<T>(MemberInvokingType callType, ulong memberId, string memberName, IGarmObject[]? args)
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
            MemberId = memberId,
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

        if (returnModel.Return is { } model)
        {
            var ipcType = string.IsNullOrWhiteSpace(model.IpcTypeFullName)
                ? null
                : typeof(T);
            if (ipcType is not null
                && Context.TryCreateProxyFromSerializationInfo(PeerProxy,
                    ipcType, model.Id, out var proxyInstance))
            {
                // 如果远端返回 IPC 公开的对象，则本地获取此对象的代理并返回。
                return (T?) proxyInstance;
            }
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

        var header = (ulong)KnownMessageHeaders.RemoteObjectMessageHeader;
        var requestMessage = Context.ObjectSerializer.SerializeToIpcMessage(header, model, model.ToString());
        //requestMessage = new IpcMessage(requestMessage.Tag, requestMessage.Body, CoreMessageType.JsonObject);
        var responseMessage = await PeerProxy.GetResponseAsync(requestMessage).ConfigureAwait(false);
        if (Context.ObjectSerializer.TryDeserializeFromIpcMessage<GeneratedProxyMemberReturnModel>(responseMessage, header, out var returnModel))
        {
            return returnModel;
        }
        else
        {
            throw new NotSupportedException($"请谨慎对待此异常！无法处理 IPC 代理调用的返回值。RequestMessage 为：{requestMessage}");
        }
    }

    private T? Cast<T>(IpcJsonElement? arg) => arg is { } jsonElement
        ? IpcJsonElement.Deserialize<T>(jsonElement, Context.ObjectSerializer)
        : default!;

    private GeneratedProxyObjectModel? SerializeArg(IGarmObject argModel)
    {
        if (PeerProxy is null)
        {
            return null;
        }

        if (Context.TryCreateSerializationInfoFromIpcRealInstance(argModel, out var objectId, out var ipcTypeFullName))
        {
            // 如果此参数是一个 IPC 对象。
            return new GeneratedProxyObjectModel
            {
                Id = objectId,
                IpcTypeFullName = ipcTypeFullName,
            };
        }
        else
        {
            // 如果此参数只是一个普通对象。
            return new GeneratedProxyObjectModel
            {
                Value = IpcJsonElement.Serialize(argModel.Value, Context.ObjectSerializer),
            };
        }
    }
}
