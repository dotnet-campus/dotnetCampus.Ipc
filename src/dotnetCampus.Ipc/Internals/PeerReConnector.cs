using System.IO;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.Internals
{
    /// <summary>
    /// 重新连接器
    /// </summary>
    class PeerReConnector
    {
        public PeerReConnector(PeerProxy peerProxy, IpcProvider ipcProvider)
        {
            _peerProxy = peerProxy;
            _ipcProvider = ipcProvider;

            peerProxy.PeerConnectionBroken += PeerProxy_PeerConnectionBroken;
        }

        private void PeerProxy_PeerConnectionBroken(object? sender, IPeerConnectionBrokenArgs e)
        {
            Reconnect();
        }

        private readonly PeerProxy _peerProxy;
        private readonly IpcProvider _ipcProvider;

        private async void Reconnect()
        {
            var ipcClientService = _ipcProvider.CreateIpcClientService(_peerProxy.PeerName);
            for (int i = 0; i < 16; i++)
            {
                try
                {
                    await ipcClientService.Start();
                    break;
                }
                // ## 此异常有两种
                catch (FileNotFoundException)
                {
                    // 1. 一种来自于 namedPipeClientStream.ConnectAsync()，刚调用时还能获取到管道句柄，但马上与之连接时便已断开。
                    await Task.Delay(16);
                }
                catch (IOException)
                {
                    // 2. 另一种来自 RegisterToPeer()，前面已经连上了，但试图发消息时便已断开。
                    await Task.Delay(16);
                }
                // ## 然而，为什么一连上就断开了呢？
                //
                // 这是因为每个端有两条管道，各自作为服务端和客户端。
                // 当重连时，靠的是服务端管道读到末尾来判断的；但此时重连的却是客户端。
                // 有极少情况下，这两条的断开时间间隔足够长到本方法的客户端已开始重连。
                // 那么，本方法的客户端在一开始试图连接对方时连上了，但随即就完成了之前没完成的断开，于是出现 FileNotFoundException。
                //
                // ## 那么，如何解决呢？
                //
                // 通过重连，我们可以缓解因对方正在断开导致的我们误连。通过重连多次，可以更大概率缓解以至于解决此异常。
                //
                // ## 是否有后续问题？
                //
                // 有可能本方法已全部完成之后才断开吗？不可能，因为 RegisterToPeer() 会发消息的，如果是对方进程退出等原因导致的断连，那么消息根本就无法发送。
                // 因为本调用内会置一个 TaskCompleteSource，所以也会导致一起等待此任务的其他发送全部失败，而解决方法就是在其他发送处也重试。
            }
            // 实在没办法连上，就抛异常吧。
            _peerProxy.Reconnect(ipcClientService);
        }
    }
}
