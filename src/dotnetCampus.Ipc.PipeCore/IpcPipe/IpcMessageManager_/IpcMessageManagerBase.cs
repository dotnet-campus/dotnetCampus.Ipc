using System.IO;

namespace dotnetCampus.Ipc.PipeCore.IpcPipe
{
    class IpcMessageManagerBase
    {
        private static bool CheckHeader(Stream stream, byte[] header)
        {
            for (var i = 0; i < header.Length; i++)
            {
                if (stream.ReadByte() == header[i])
                {
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        protected static bool CheckResponseHeader(Stream stream)
        {
            var header = ResponseMessageHeader;

            return CheckHeader(stream, header);
        }
        protected static bool CheckRequestHeader(Stream stream)
        {
            var header = RequestMessageHeader;
            return CheckHeader(stream, header);
        }


        /// <summary>
        /// 用于标识请求消息
        /// </summary>
        /// 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74 0x00 就是 Request 字符
        protected static byte[] RequestMessageHeader { get; } = { 0x52, 0x65, 0x71, 0x75, 0x65, 0x73, 0x74, 0x00 };

        protected static byte[] ResponseMessageHeader { get; } = { 0x52, 0x65, 0x73, 0x70, 0x6F, 0x6E, 0x73, 0x65 };
    }
}
