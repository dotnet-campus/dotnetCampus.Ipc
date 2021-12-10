using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.PipeMvcServer.HostFramework;
using dotnetCampus.Ipc.Pipes;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class IpcCore
    {
        public IpcCore(IServiceProvider serviceProvider, string? ipcServerName)
        {
            ipcServerName ??= IpcServerName;

            IpcServer = new IpcProvider(ipcServerName, new IpcConfiguration()
            {
                DefaultIpcRequestHandler = new DelegateIpcRequestHandler(async context =>
                {
                    var server = (IpcServer) serviceProvider.GetRequiredService<IServer>();

                    var requestMessage = HttpMessageSerializer.DeserializeToRequest(context.IpcBufferMessage.Body.Buffer);

                    var clientHandler = (ClientHandler) server.CreateHandler();
                    var response = await clientHandler.SendInnerAsync(requestMessage, CancellationToken.None);

                    var responseByteList = HttpMessageSerializer.Serialize(response);

                    return new IpcResponseMessageResult(new IpcMessage($"[Response][{requestMessage.Method}] {requestMessage.RequestUri}", responseByteList));
                })
            });
        }

        public void Start() => IpcServer.StartServer();
        public IpcProvider IpcServer { set; get; }

        public static readonly string IpcServerName = Guid.NewGuid().ToString("N");


        public static PeerProxy Client { set; get; }

        public static IpcProvider? IpcClient { set; get; }

        public static async Task<HttpClient> GetHttpClientAsync()
        {
            if (IpcClient == null)
            {
                IpcClient = new IpcProvider();
                IpcClient.StartServer();
                var peer = await IpcClient.GetAndConnectToPeerAsync(IpcServerName);
                Client = peer;
            }

            return new HttpClient(new IpcNamedPipeClientHandler(Client))
            {
                BaseAddress = BaseAddress,
            };
        }

        public static Uri BaseAddress { get; set; } = new Uri("http://localhost/");
    }
}
