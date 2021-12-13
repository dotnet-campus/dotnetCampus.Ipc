#nullable disable // 序列化的代码，不需要可空

using System.Net.Http;
using System.Net.Http.Headers;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class HttpRequestMessageSerializeContent : HttpRequestMessageContentBase
    {
        public HttpRequestMessageSerializeContent(HttpRequestMessage message) : base(message)
        {
            Headers = message.Headers;
        }

        public HttpRequestHeaders Headers { get; set; }
    }
}
