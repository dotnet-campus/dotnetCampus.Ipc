using System.Diagnostics;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Pipes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests;

[TestClass]
public class IpcProviderTests
{
    [TestMethod("使用 TryConnectToExistingPeerAsync 尝试连接不存在的对方，可以立刻返回连接失败")]
    public async Task TestTryConnectToExistingPeerAsync1()
    {
        var ipcProvider = new IpcProvider();
        var result = await ipcProvider.TryConnectToExistingPeerAsync("The_Not_Exists_Peer_Name_E6EE8975-EF9A-480B-912D-B3C4530294E0");
        Assert.IsFalse(result.IsSuccess);
    }

    [TestMethod("使用 TryConnectToExistingPeerAsync 尝试连接存在的对方，可以返回连接成功")]
    public async Task TestTryConnectToExistingPeerAsync2()
    {
        var peerName = "IpcProvider1_The_Exists_Peer_Name_E6EE8975-EF9A-480B-912D-B3C4530294E0";
        var testLogger = new TestLogger();
        var ipcProvider1 = new IpcProvider(peerName, new IpcConfiguration()
        {
            IpcLoggerProvider = _ => testLogger
        });
        ipcProvider1.StartServer();
        var ipcProvider2 = new IpcProvider(peerName.Replace("IpcProvider1", "IpcProvider2"), new IpcConfiguration()
        {
            IpcLoggerProvider = _ => testLogger
        });
        ipcProvider2.StartServer();

        try
        {
            var result = await ipcProvider2.TryConnectToExistingPeerAsync(peerName).WaitAsync(TimeSpan.FromSeconds(5));

            Assert.IsTrue(result.IsSuccess);
            Assert.IsNotNull(result.PeerProxy);
        }
        catch
        {
            Console.WriteLine(testLogger.GetAllLogMessage());
            throw;
        }
    }

    // 预期这一条是可能过也可能不过的，取决于时机，于是默认不加入单元测试去跑了
    // 尽管提供了 TryConnectToExistingPeerAsync 方法，但仍然有一些边界的情况。正常也没有开发者会想着去连接自己吧
    //[TestMethod("使用 TryConnectToExistingPeerAsync 尝试错误地连接自身，可能出现无限等待")]
    public async Task TestTryConnectToExistingPeerAsync3()
    {
        var peerName = "The_Self_IpcProvider_E6EE8975-EF9A-480B-912D-B3C4530294E0";
        var ipcProvider1 = new IpcProvider(peerName);
        ipcProvider1.StartServer();

        var result = await ipcProvider1.TryConnectToExistingPeerAsync(peerName).WaitAsync(TimeSpan.FromSeconds(5));
        _ = result;
    }
}
