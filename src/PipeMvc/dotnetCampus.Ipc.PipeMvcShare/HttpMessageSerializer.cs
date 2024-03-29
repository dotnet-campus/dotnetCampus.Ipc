﻿#nullable disable // 序列化的代码，不需要可空

using System.Net.Http;
using System.Text;

using dotnetCampus.Ipc.Messages;

using Newtonsoft.Json;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
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

        public static HttpRequestMessage DeserializeToRequest(IpcMessageBody body)
        {
            var span = body.AsSpan();
            var json = Encoding.UTF8.GetString(span);
            var httpRequestMessageDeserializeContent = JsonConvert.DeserializeObject<HttpRequestMessageDeserializeContent>(json);
            return httpRequestMessageDeserializeContent.ToHttpResponseMessage();
        }
    }
}
