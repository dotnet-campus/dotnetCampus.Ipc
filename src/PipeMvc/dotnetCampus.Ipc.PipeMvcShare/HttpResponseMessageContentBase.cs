#nullable disable // 序列化的代码，不需要可空

using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;

namespace dotnetCampus.Ipc.PipeMvcServer.IpcFramework
{
    class HttpResponseMessageContentBase
    {
        public HttpResponseMessageContentBase()
        {
        }

        public HttpResponseMessageContentBase(HttpResponseMessage message)
        {
            if (message.Content != null)
            {
                using var memoryStream = new MemoryStream();
                message.Content.CopyTo(memoryStream, null, CancellationToken.None);
                ContentBase64 = Convert.ToBase64String(memoryStream.ToArray());

            }

            Version = message.Version;
            StatusCode = message.StatusCode;
        }

        public string ContentBase64 { set; get; }
        public Version Version { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}
