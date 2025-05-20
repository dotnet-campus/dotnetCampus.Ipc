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
        var result = await ipcProvider.TryGetOrConnectExistsPeerAsync("The_Not_Exists_Peer_Name_E6EE8975-EF9A-480B-912D-B3C4530294E0");
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

        var result = await ipcProvider1.TryGetOrConnectExistsPeerAsync(peerName);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsNotNull(result.PeerProxy);
    }
}
