namespace dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

public class JsonIpcDirectRoutedContext
{
    public JsonIpcDirectRoutedContext(string peerName)
    {
        this.PeerName = peerName;
    }

    public string PeerName { get; }
}
