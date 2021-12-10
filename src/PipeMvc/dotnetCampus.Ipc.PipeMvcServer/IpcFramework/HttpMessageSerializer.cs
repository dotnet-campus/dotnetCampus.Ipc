using System.Net.Http;
using System.Text;
using dotnetCampus.Ipc.Messages;
using Newtonsoft.Json;

namespace dotnetCampus.Ipc.PipeMvcServer
{
    static class HttpMessageSerializer
    {
        public static byte[] Serialize(HttpResponseMessage response)
        {
            var httpResponseMessageContentBase = new HttpResponseMessageSerializeContent(response);
            var json = JsonConvert.SerializeObject(httpResponseMessageContentBase);

            return Encoding.UTF8.GetBytes(json);
        }

        public static byte[] Serialize(HttpRequestMessage request)
        {
            var json = JsonConvert.SerializeObject(new HttpRequestMessageSerializeContent(request));

            return Encoding.UTF8.GetBytes(json);
        }

        internal static HttpResponseMessage DeserializeToResponse(IpcMessageBody body)
        {
            var span = body.AsSpan();
            var json = Encoding.UTF8.GetString(span);
            var content = JsonConvert.DeserializeObject<HttpResponseMessageDeserializeContent>(json);

            return content.ToHttpResponseMessage();
        }

        public static HttpRequestMessage DeserializeToRequest(byte[] d)
        {
            var json = Encoding.UTF8.GetString(d);
            var httpRequestMessageDeserializeContent = JsonConvert.DeserializeObject<HttpRequestMessageDeserializeContent>(json);
            return httpRequestMessageDeserializeContent.ToHttpResponseMessage();
        }
    }
}