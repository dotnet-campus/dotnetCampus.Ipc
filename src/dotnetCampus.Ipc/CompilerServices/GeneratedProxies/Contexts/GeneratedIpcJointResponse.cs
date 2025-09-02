using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Serialization;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Contexts;

internal class GeneratedIpcJointResponse : IIpcResponseMessage
{
    internal static GeneratedIpcJointResponse Empty { get; } = new GeneratedIpcJointResponse();

    private GeneratedIpcJointResponse()
    {
    }

    private GeneratedIpcJointResponse(IpcMessage message)
    {
        ResponseMessage = message;
    }

    public IpcMessage ResponseMessage { get; }

    internal static async Task<GeneratedIpcJointResponse> FromAsyncReturnModel(
        Task<GeneratedProxyMemberReturnModel?> asyncReturnModel,
        IIpcObjectSerializer serializer)
    {
        var returnModel = await asyncReturnModel.ConfigureAwait(false);
        var message = returnModel is null
            ? new IpcMessage()
            : serializer.SerializeToIpcMessage(returnModel, "Return");
        return new GeneratedIpcJointResponse(message);
    }
}
