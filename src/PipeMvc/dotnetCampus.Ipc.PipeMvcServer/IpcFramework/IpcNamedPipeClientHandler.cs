using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    public class IpcNamedPipeClientHandler : HttpMessageHandler
    {


        public IpcNamedPipeClientHandler(PeerProxy client)
        {
            Client = client;
        }

        public PeerProxy Client { get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = HttpMessageSerializer.Serialize(request);

            var response = await Client.GetResponseAsync(new IpcMessage(request.RequestUri?.ToString() ?? request.Method.ToString(),
                message));

            return HttpMessageSerializer.DeserializeToResponse(response.Body);
        }
    }
}