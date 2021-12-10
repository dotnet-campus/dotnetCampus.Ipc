using System.Net.Http;
using System.Net.Http.Headers;

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
        public HttpContentHeaders? ContentHeaders { get; set; }
    }
}
