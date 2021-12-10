using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

using dotnetCampus.Ipc.PipeMvcServer.IpcFramework;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.PipeMvcClient
{
    public static class PipeMvcClientProvider
    {
        public static async Task<HttpClient> CreateIpcMvcClient(string ipcPipeMvcServerName,IpcProvider? clientIpcProvider = null)
        {
            if (clientIpcProvider == null)
            {
                clientIpcProvider = new IpcProvider();
                clientIpcProvider.StartServer();
            }

            var peer = await clientIpcProvider.GetAndConnectToPeerAsync(ipcPipeMvcServerName);

            return new HttpClient(new IpcNamedPipeClientHandler(peer, clientIpcProvider))
            {
                BaseAddress = new Uri("http://localhost/"),
            };
        }
    }
}
