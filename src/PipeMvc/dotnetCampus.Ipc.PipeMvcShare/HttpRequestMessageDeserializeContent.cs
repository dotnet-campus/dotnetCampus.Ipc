#nullable disable // 序列化的代码，不需要可空

using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class HttpRequestMessageDeserializeContent : HttpRequestMessageContentBase
    {
        public HttpRequestMessage ToHttpResponseMessage()
        {
            var result = new HttpRequestMessage()
            {
                Version = Version,
                VersionPolicy = VersionPolicy,
                Method = Method,
                RequestUri = RequestUri,
            };

            var memoryStream = new MemoryStream(Convert.FromBase64String(ContentBase64));
            //var text = Encoding.UTF8.GetString(memoryStream.ToArray());
            var streamContent = new StreamContent(memoryStream);
            result.Content = streamContent;

            var headerContentList = ContentHeaders.ToObject<List<HeaderContent>>();

            if (headerContentList != null)
            {
                foreach (var headerContent in headerContentList)
                {
                    result.Content.Headers.Add(headerContent.Key, headerContent.Value);
                }
            }

            headerContentList = Headers.ToObject<List<HeaderContent>>();
            if (headerContentList != null)
            {
                foreach (var headerContent in headerContentList)
                {
                    result.Headers.Add(headerContent.Key, headerContent.Value);
                }
            }


            return result;
        }

        public JContainer Headers { set; get; }

        public JContainer ContentHeaders { set; get; }
    }
}
