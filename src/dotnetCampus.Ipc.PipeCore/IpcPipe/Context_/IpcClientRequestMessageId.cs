namespace dotnetCampus.Ipc.PipeCore.IpcPipe
{
    public readonly struct IpcClientRequestMessageId
    {
        public IpcClientRequestMessageId(ulong messageIdValue)
        {
            MessageIdValue = messageIdValue;
        }

        public ulong MessageIdValue { get; }
    }
}
