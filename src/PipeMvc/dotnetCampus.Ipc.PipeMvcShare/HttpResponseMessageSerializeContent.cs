using System.Net.Http;
using System.Net.Http.Headers;

#nullable disable // 序列化的代码，不需要可空

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class HttpResponseMessageSerializeContent : HttpResponseMessageContentBase
    {
        public HttpResponseMessageSerializeContent(HttpResponseMessage message) : base(message)
        {
            Headers = message.Headers;
            ContentHeaders = message.Content?.Headers;
        }
        public HttpResponseHeaders Headers { get; set; }
#nullable enable
        public HttpContentHeaders? ContentHeaders { get; set; }
    }
}
