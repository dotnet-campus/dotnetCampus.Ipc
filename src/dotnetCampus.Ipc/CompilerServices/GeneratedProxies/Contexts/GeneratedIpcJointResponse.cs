using System.Threading.Tasks;

using dotnetCampus.Ipc.Messages;

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Contexts
{
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

        internal static async Task<GeneratedIpcJointResponse> FromAsyncReturnModel(Task<GeneratedProxyMemberReturnModel?> asyncReturnModel)
        {
            var returnModel = await asyncReturnModel.ConfigureAwait(false);
            var message = returnModel is null
                ? new IpcMessage()
                : GeneratedProxyMemberReturnModel.Serialize(returnModel);
            return new GeneratedIpcJointResponse(message);
        }
    }
}
