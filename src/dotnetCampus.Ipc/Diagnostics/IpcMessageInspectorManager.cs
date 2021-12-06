using System;
using System.Collections.Concurrent;

namespace dotnetCampus.Ipc.Diagnostics
{
    /// <summary>
    /// 管理 IPC 消息收发时的检查器。
    /// </summary>
    public class IpcMessageInspectorManager
    {
        private static readonly ConcurrentDictionary<string, IpcMessageInspectorManager> Managers = new();

        /// <summary>
        /// 根据本地 Peer 名称查找 IPC 消息收发的检查器。
        /// </summary>
        /// <param name="peerName"></param>
        /// <returns></returns>
        public static IpcMessageInspectorManager FromLocalPeerName(string peerName)
        {
            return Managers.GetOrAdd(peerName, name => new IpcMessageInspectorManager(name));
        }

        private readonly string _localPeerName;

        private readonly ConcurrentDictionary<IIpcMessageInspector, IIpcMessageInspector> _inspectors = new();

        private IpcMessageInspectorManager(string peerName)
        {
            _localPeerName = peerName ?? throw new ArgumentNullException(nameof(peerName));
        }

        /// <summary>
        /// 注册一个 IPC 消息检查器。
        /// </summary>
        /// <param name="inspector"></param>
        public void RegisterInspector(IIpcMessageInspector inspector)
        {
            _inspectors.TryAdd(inspector, inspector);
        }

        internal void Call(Action<IIpcMessageInspector> caller)
        {
            foreach (var inspectorPair in _inspectors)
            {
                caller(inspectorPair.Key);
            }
        }
    }
}
