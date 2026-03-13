using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;

namespace dotnetCampus.Ipc.Internals
{
    /// <summary>
    /// 管理所有连接方
    /// </summary>
    public interface IPeerManager
    {
        /// <summary>
        /// 和 <see cref="IpcProvider"/> 不同的是，无论是主动连接还是被动连过来的，都会触发此事件
        /// </summary>
        event EventHandler<PeerConnectedArgs>? PeerConnected;

        /// <summary>
        /// 当前连接的数量。无论是主动连接的还是被连接的对方都会记录在此。此属性仅有记日志的作用。由于 IPC 将会不断多进程连接和断开，所以这个数量是不断变化的。可能获取的一刻，实际情况就和此不相同
        /// </summary>
        int CurrentConnectedPeerProxyCount { get; }

        /// <summary>
        /// 获取当前连接到的 <see cref="PeerProxy"/> 列表。无论是主动连接的还是被连接的对方都会记录在此。仅表示获取时的状态，由于 IPC 将会不断多进程连接和断开，所以这个列表是不断变化的。可能获取的一刻，实际情况就和此不相同
        /// </summary>
        /// <returns></returns>
        IReadOnlyList<PeerProxy> GetCurrentConnectedPeerProxyList();
    }

    class PeerManager : IPeerManager, IDisposable
    {
        public PeerManager(IpcProvider ipcProvider)
        {
            _ipcProvider = ipcProvider;
        }

        public bool TryAdd(PeerProxy peerProxy)
        {
            peerProxy.PeerConnectionBroken += PeerProxy_PeerConnectionBroken;
            OnAdd(peerProxy);
            return ConnectedPeerProxyDictionary.TryAdd(peerProxy.PeerName, peerProxy);
        }

        public bool TryGetValue(string key, [NotNullWhen(true)] out PeerProxy? peer)
        {
            return ConnectedPeerProxyDictionary.TryGetValue(key, out peer);
        }

        /// <summary>
        /// 删除断开的对方
        /// </summary>
        /// <param name="peerProxy"></param>
        public void RemovePeerProxy(PeerProxy peerProxy)
        {
            if (!peerProxy.IsBroken)
            {
                throw new ArgumentException($"Must remove the Broken peer. PeerName={peerProxy.PeerName}");
            }

            ConnectedPeerProxyDictionary.TryRemove(peerProxy.PeerName, out var value);

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

                ConnectedPeerProxyDictionary.TryAdd(value.PeerName, value);
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
            ConnectedPeerProxyDictionary.AddOrUpdate(peerProxy.PeerName, peerProxy, (s, proxy) => proxy);
        }

        private void OnAdd(PeerProxy peerProxy)
        {
            if (AutoReconnectPeers)
            {
                peerProxy.PeerReConnector ??= new PeerReConnector(peerProxy, _ipcProvider);

                peerProxy.PeerReConnector.ReconnectFail -= PeerReConnector_ReconnectFail;
                peerProxy.PeerReConnector.ReconnectFail += PeerReConnector_ReconnectFail;
            }

            if (!ConnectedPeerProxyDictionary.ContainsKey(peerProxy.PeerName))
            {
                // 没有从字典找到，证明是首次连接到的。或曾经断开过的，此后再连接的
                PeerConnected?.Invoke(this, new PeerConnectedArgs(peerProxy));
            }
        }

        /// <inheritdoc />
        public event EventHandler<PeerConnectedArgs>? PeerConnected;

        /// <inheritdoc />
        public int CurrentConnectedPeerProxyCount => ConnectedPeerProxyDictionary.Count;

        /// <inheritdoc />
        public IReadOnlyList<PeerProxy> GetCurrentConnectedPeerProxyList()
        {
            // 这里是线程安全的，但只会返回当前的状态
            return ConnectedPeerProxyDictionary.Values.ToList();
        }

        private void PeerReConnector_ReconnectFail(object? sender, ReconnectFailEventArgs e)
        {
            // 重新连接失败，此时需要清理
            // 如果不清理，将会导致过了一会，有新的连接进来，使用和上次需要重连相同的 PeerName 的不会再次触发事件
            RemovePeerProxy(e.PeerProxy);
        }

        private bool AutoReconnectPeers => _ipcProvider.IpcServerService.IpcContext.IpcConfiguration.AutoReconnectPeers;

        public void Dispose()
        {
            foreach (var pair in ConnectedPeerProxyDictionary)
            {
                var peer = pair.Value;
                // 为什么 PeerProxy 不加上 IDisposable 方法
                // 因为这个类在上层业务使用，被上层业务调释放就可以让框架不能使用
                peer.DisposePeer();
            }
        }

        private ConcurrentDictionary<string/*PeerName*/, PeerProxy> ConnectedPeerProxyDictionary { get; } =
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
