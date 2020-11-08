using System;
using dotnetCampus.Ipc.Abstractions;
using dotnetCampus.Ipc.PipeCore.Context;

namespace dotnetCampus.Ipc.PipeCore.IpcPipe
{
    class IpcMessageResponseManager:IpcMessageManagerBase
    {
        public IpcBufferMessageContext CreateResponseMessage(IpcClientRequestMessageId messageId, IpcRequestMessage response)
        {
            /*
           * MessageHeader
           * MessageId
            * Response Message Length
           * Response Message
           */
            var currentMessageIdByteList = BitConverter.GetBytes(messageId.MessageIdValue);

            var responseMessageLengthByteList = BitConverter.GetBytes(response.RequestMessage.Count);
            return new IpcBufferMessageContext
            (
                response.Summary,
                IpcMessageCommandType.ResponseMessage,
                new IpcBufferMessage(ResponseMessageHeader),
                new IpcBufferMessage(currentMessageIdByteList),
                new IpcBufferMessage(responseMessageLengthByteList),
                response.RequestMessage
            );
        }
    }
}
