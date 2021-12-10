using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.PipeMvcServer.IpcFramework;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.PipeMvcClient
{
    class IpcNamedPipeClientHandler : HttpMessageHandler
    {
        public IpcNamedPipeClientHandler(PeerProxy client, IpcProvider clientIpcProvider)
        {
            Client = client;
            ClientIpcProvider = clientIpcProvider;
        }

        private PeerProxy Client { get; }

        /// <summary>
        /// 客户端的 IPC 服务
        /// </summary>
        /// 只是引用对象，不然 IpcProvider 将被回收
        private IpcProvider ClientIpcProvider { get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = HttpMessageSerializer.Serialize(request);

            var response = await Client.GetResponseAsync(new IpcMessage(request.RequestUri?.ToString() ?? request.Method.ToString(),
                message));

            return HttpMessageSerializer.DeserializeToResponse(response.Body);
        }

        protected override void Dispose(bool disposing)
        {
            ClientIpcProvider.Dispose();
            base.Dispose(disposing);
        }
    }
}
