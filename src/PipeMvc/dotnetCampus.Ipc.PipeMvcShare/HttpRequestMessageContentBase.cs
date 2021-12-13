#nullable disable // 序列化的代码，不需要可空

using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class HttpRequestMessageContentBase
    {
        public HttpRequestMessageContentBase(HttpRequestMessage message)
        {
            Version = message.Version;
            VersionPolicy = message.VersionPolicy;
            Method = message.Method;
            RequestUri = message.RequestUri;
            Options = message.Options;

            if (message.Content != null)
            {
                ContentHeaders = message.Content.Headers;
                using var memoryStream = new MemoryStream();
                message.Content.CopyTo(memoryStream, null, CancellationToken.None);
                ContentBase64 = Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        public HttpRequestMessageContentBase()
        {
        }


        public string ContentBase64 { set; get; }

        public Version Version { set; get; }
        public HttpVersionPolicy VersionPolicy { set; get; }

        public HttpMethod Method { set; get; }
        public Uri? RequestUri { set; get; }
        public HttpRequestOptions Options { set; get; }
        public HttpContentHeaders ContentHeaders { get; set; }
    }
}
