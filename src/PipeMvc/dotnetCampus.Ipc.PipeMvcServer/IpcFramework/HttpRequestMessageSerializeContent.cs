using System.Net.Http;
using System.Net.Http.Headers;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    public class HttpRequestMessageSerializeContent : HttpRequestMessageContentBase
    {
        public HttpRequestMessageSerializeContent(HttpRequestMessage message) : base(message)
        {
            Headers = message.Headers;
        }

        public HttpRequestHeaders Headers { get; set; }
    }
}