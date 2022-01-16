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
        public IpcNamedPipeClientHandler(PeerProxy client, IpcProvider? clientIpcProvider)
        {
            Client = client;
            ClientIpcProvider = clientIpcProvider;
        }

        private PeerProxy Client { get; }

        /// <summary>
        /// 客户端的 IPC 服务
        /// </summary>
        /// 只是引用对象，不然 IpcProvider 将被回收
        /// 可空，不给就不给咯，只是做引用
        private IpcProvider? ClientIpcProvider { get; }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var message = HttpMessageSerializer.Serialize(request);

            // 创建 IPC 消息的 Tag 内容，此 Tag 内容仅用来调试和记录日志
            var ipcMessageTag = request.RequestUri?.ToString() ?? request.Method.ToString();

            // 通过 PeerProxy 发送 IPC 请求，此时的 IPC 请求将会被 PipeMvcServer 处理
            // 在 PipeMvcServer 里面，将通过 ASP.NET Core MVC 框架层进行调度，分发到对应的控制器处理
            // 控制器处理完成之后，将由 MVC 框架层将控制器的输出交给 PipeMvcServer 层
            // 在 PipeMvcServer 层收到控制器的输出之后，将通过 IPC 框架，将输出返回给 PipeMvcClient 端
            // 当 PipeMvcClient 收到输出返回值后，以下的 await 方法将会返回
            var response = await Client.GetResponseAsync(new IpcMessage(ipcMessageTag, message));

            return HttpMessageSerializer.DeserializeToResponse(response.Body);
        }

        protected override void Dispose(bool disposing)
        {
            ClientIpcProvider?.Dispose();
            base.Dispose(disposing);
        }
    }
}
