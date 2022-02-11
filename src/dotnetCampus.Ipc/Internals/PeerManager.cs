using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.Internals
{
    class PeerManager : IDisposable
    {
        public PeerManager(IpcProvider ipcProvider)
        {
            _ipcProvider = ipcProvider;
        }

        public bool TryAdd(PeerProxy peerProxy)
        {
            peerProxy.PeerConnectionBroken += PeerProxy_PeerConnectionBroken;
            OnAdd(peerProxy);
            return ConnectedServerManagerList.TryAdd(peerProxy.PeerName, peerProxy);
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out PeerProxy? peer)
        {
            return ConnectedServerManagerList.TryGetValue(key, out peer);
        }

        /// <summary>
        /// 删除断开的对方
        /// </summary>
        /// <param name="peerProxy"></param>
        public void RemovePeerProxy(PeerProxy peerProxy)
        {
            if (!peerProxy.IsBroken)
            {
                throw new ArgumentException($"Must remove the Broken peer");
            }

            ConnectedServerManagerList.TryRemove(peerProxy.PeerName, out var value);

            if (ReferenceEquals(peerProxy, value) || value is null)
            {
                // 这是预期的
            }
            else
            {
                // 居然放在列表里面的，和当前断开连接的不是相同的 Peer 那么将此加入回去
                if (Debugger.IsAttached)
                {
                    // 请将德熙叫过来，理论上不会进入这个分支
                    Debugger.Break();
                    throw new InvalidOperationException(
                        $"Peer 断开之后，从已有列表删除时发现列表里面记录的 Peer 和当前的不是相同的一个。仅调试下抛出。PeerName={peerProxy.PeerName}");
                }

                ConnectedServerManagerList.TryAdd(value.PeerName, value);
            }
        }

        /// <summary>
        /// 等待对方连接完成
        /// </summary>
        /// <param name="peerProxy"></param>
        /// <returns></returns>
        public async Task WaitForPeerConnectFinishedAsync(PeerProxy peerProxy)
        {
            await peerProxy.WaitForFinishedTaskCompletionSource.Task;

            OnAdd(peerProxy);
            // 更新或注册，用于解决之前注册的实际上是断开的连接
            ConnectedServerManagerList.AddOrUpdate(peerProxy.PeerName, peerProxy, (s, proxy) => proxy);
        }

        private void OnAdd(PeerProxy peerProxy)
        {
            if (AutoReconnectPeers)
            {
                peerProxy.PeerReConnector ??= new PeerReConnector(peerProxy, _ipcProvider);
            }
        }

        private bool AutoReconnectPeers => _ipcProvider.IpcServerService.IpcContext.IpcConfiguration.AutoReconnectPeers;

        public void Dispose()
        {
            foreach (var pair in ConnectedServerManagerList)
            {
                var peer = pair.Value;
                // 为什么 PeerProxy 不加上 IDisposable 方法
                // 因为这个类在上层业务使用，被上层业务调释放就可以让框架不能使用
                peer.DisposePeer();
            }
        }


        private ConcurrentDictionary<string, PeerProxy> ConnectedServerManagerList { get; } =
            new ConcurrentDictionary<string, PeerProxy>();
        private readonly IpcProvider _ipcProvider;

        private void PeerProxy_PeerConnectionBroken(object? sender, IPeerConnectionBrokenArgs e)
        {
            if (AutoReconnectPeers)
            {
                // 如果需要自动连接，那么就不需要删除记录
                return;
            }

            var peerProxy = (PeerProxy) sender!;
            RemovePeerProxy(peerProxy);
        }
    }
}
