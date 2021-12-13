using System;
using System.Net.Http;
using System.Net.Sockets;
using System.Threading.Tasks;

using dotnetCampus.Ipc.PipeMvcServer.IpcFramework;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.PipeMvcClient
{
    /// <summary>
    /// 提供给客户端调用 MVC 的 Ipc 服务的功能
    /// </summary>
    public static class IpcPipeMvcClientProvider
    {
        /// <summary>
        /// 获取访问 Mvc 的 Ipc 服务的对象
        /// </summary>
        /// <param name="ipcPipeMvcServerName">对方 Ipc 服务名</param>
        /// <param name="clientIpcProvider">可选，用来进行 Ipc 连接的本地服务。如不传或是空，将创建新的 Ipc 连接服务</param>
        /// <returns></returns>
        public static async Task<HttpClient> CreateIpcMvcClient(string ipcPipeMvcServerName, IpcProvider? clientIpcProvider = null)
        {
            if (clientIpcProvider == null)
            {
                clientIpcProvider = new IpcProvider();
                clientIpcProvider.StartServer();
            }

            var peer = await clientIpcProvider.GetAndConnectToPeerAsync(ipcPipeMvcServerName);

            return new HttpClient(new IpcNamedPipeClientHandler(peer, clientIpcProvider))
            {
                BaseAddress = new Uri(IpcPipeMvcContext.BaseAddressUrl),
            };
        }

    }
}
