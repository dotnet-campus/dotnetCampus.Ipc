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

            if (ContentBase64 is not null)
            {
                var memoryStream = new MemoryStream(Convert.FromBase64String(ContentBase64));
                //var text = Encoding.UTF8.GetString(memoryStream.ToArray());
                var streamContent = new StreamContent(memoryStream);
                result.Content = streamContent;
            }

            var headerContentList = ContentHeaders?.ToObject<List<HeaderContent>>();

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

        /// <summary>
        /// 使用 <see cref="JContainer"/> 表示的 <see cref="ContentHeaders"/> 内容。特意命名和基类型相同，这样序列化时可以自动转换
        /// </summary>
        public new JContainer ContentHeaders { set; get; }
    }
}
