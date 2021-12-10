using System;
using System.Threading;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Messages;
using dotnetCampus.Ipc.PipeMvcServer.HostFramework;
using dotnetCampus.Ipc.Pipes;

using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.Extensions.DependencyInjection;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class IpcPipeMvcServerCore
    {
        public IpcPipeMvcServerCore(IServiceProvider serviceProvider, string? ipcServerName)
        {
            ipcServerName ??= "IpcPipeMvcServer" + Guid.NewGuid().ToString("N");

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
    }
}
