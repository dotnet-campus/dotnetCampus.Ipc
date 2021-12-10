using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using Newtonsoft.Json.Linq;

namespace dotnetCampus.Ipc.PipeMvcServer
{
    public class HttpResponseMessageDeserializeContent : HttpResponseMessageContentBase
    {
        public JContainer Headers { set; get; }
        public JContainer ContentHeaders { get; set; }

        public HttpResponseMessage ToHttpResponseMessage()
        {
            var httpResponseMessage = new HttpResponseMessage(StatusCode)
            {
                Version = Version,
            };

            //foreach (var httpResponseHeader in Headers)
            //{
            //    httpResponseMessage.Headers.Add(httpResponseHeader.Key, httpResponseHeader.Value);
            //}

            var memoryStream = new MemoryStream(Convert.FromBase64String(ContentBase64));
            var text = Encoding.UTF8.GetString(memoryStream.ToArray());
            var streamContent = new StreamContent(memoryStream);
            httpResponseMessage.Content = streamContent;

            var headerContentList = ContentHeaders.ToObject<List<HeaderContent>>();

            if (headerContentList != null)
            {
                foreach (var headerContent in headerContentList)
                {
                    httpResponseMessage.Content.Headers.Add(headerContent.Key, headerContent.Value);
                }
            }

            headerContentList = Headers.ToObject<List<HeaderContent>>();
            if (headerContentList != null)
            {
                foreach (var headerContent in headerContentList)
                {
                    httpResponseMessage.Headers.Add(headerContent.Key, headerContent.Value);
                }
            }

            return httpResponseMessage;
        }
    }
}