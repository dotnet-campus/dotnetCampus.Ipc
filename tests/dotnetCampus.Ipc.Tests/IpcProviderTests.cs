using dotnetCampus.Ipc.Pipes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests;

[TestClass]
public class IpcProviderTests
{
    [TestMethod("使用 TryGetOrConnectExistsPeerAsync 尝试连接不存在的对方，可以立刻返回连接失败")]
    public async Task TestTryGetOrConnectExistsPeerAsync1()
    {
        var ipcProvider = new IpcProvider();
        var result = await ipcProvider.TryConnectToExistingPeerAsync("The_Not_Exists_Peer_Name_E6EE8975-EF9A-480B-912D-B3C4530294E0");
        Assert.IsFalse(result.IsSuccess);
    }

    [TestMethod("使用 TryGetOrConnectExistsPeerAsync 尝试连接存在的对方，可以返回连接成功")]
    public async Task TestTryGetOrConnectExistsPeerAsync2()
    {
        var peerName = "The_Exists_Peer_Name_E6EE8975-EF9A-480B-912D-B3C4530294E0";
        var ipcProvider1 = new IpcProvider(peerName);
        ipcProvider1.StartServer();
        var ipcProvider2 = new IpcProvider();
        ipcProvider2.StartServer();

        var result = await ipcProvider2.TryConnectToExistingPeerAsync(peerName).WaitAsync(TimeSpan.FromSeconds(5));
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.PeerProxy);
    }

    // 预期这一条是可能过也可能不过的，取决于时机，于是默认不加入单元测试去跑了
    // 尽管提供了 TryGetOrConnectExistsPeerAsync 方法，但仍然有一些边界的情况。正常也没有开发者会想着去连接自己吧
    //[TestMethod("使用 TryGetOrConnectExistsPeerAsync 尝试错误地连接自身，可能出现无限等待")]
    public async Task TestTryGetOrConnectExistsPeerAsync3()
    {
        var peerName = "The_Self_IpcProvider_E6EE8975-EF9A-480B-912D-B3C4530294E0";
        var ipcProvider1 = new IpcProvider(peerName);
        ipcProvider1.StartServer();

        var result = await ipcProvider1.TryConnectToExistingPeerAsync(peerName).WaitAsync(TimeSpan.FromSeconds(5));
        _ = result;
    }
}
