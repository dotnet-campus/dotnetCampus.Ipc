using dotnetCampus.Ipc.Pipes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests;

[TestClass]
public class PeerManagerTest
{
    [TestMethod("无论是主动连接还是被动连接，都能触发 PeerManager.PeerConnected 事件")]
    public async Task TestPeerManager1()
    {
        var nameA = "PeerManagerTest_1";
        var nameB = "PeerManagerTest_2";

        var a = new IpcProvider(nameA);
        var b = new IpcProvider(nameB);

        var aPeerManagerConnectedCount = 0;
        var bPeerManagerConnectedCount = 0;

        a.PeerManager.PeerConnected += (sender, args) =>
        {
            aPeerManagerConnectedCount++;
        };

        b.PeerManager.PeerConnected += (sender, args) =>
        {
            bPeerManagerConnectedCount++;
        };

        var aIpcProviderConnectedCount = 0;
        var bIpcProviderConnectedCount = 0;

        a.PeerConnected += (sender, args) =>
        {
            aIpcProviderConnectedCount++;
        };
        var bConnectedTaskCompletionSource = new TaskCompletionSource();
        b.PeerConnected += (sender, args) =>
        {
            bIpcProviderConnectedCount++;
            bConnectedTaskCompletionSource.SetResult();
        };

        a.StartServer();

        Task<PeerProxy> connectTask = a.GetAndConnectToPeerAsync(nameB);

        Assert.IsFalse(connectTask.IsCompleted, "由于此时 B 服务还没启动，必定现在还没连接完成");
        // 这里有一个小争议点，那就是事件名为 PeerConnected 但实际上可能没有完全完成连接的建立
        Assert.AreEqual(1, aPeerManagerConnectedCount, "已经主动在 a 发起连接，此时有一个记录");
        Assert.AreEqual(0, bPeerManagerConnectedCount, "还没有完成建立连接，必定现在是 0 的值");

        Assert.AreEqual(0, aIpcProviderConnectedCount, "还没有完成建立连接，必定现在是 0 的值");
        Assert.AreEqual(0, bIpcProviderConnectedCount, "还没有完成建立连接，必定现在是 0 的值");

        // 开启 B 服务了，预期现在能够完成连接
        b.StartServer();

        // 等待连接
        await connectTask;

        // 由于 b 是在另一个线程\进程跑的，不是在当前单元测试所在的线程跑的。需要等待一下，避免多线程执行顺序，导致单元测试概率不通过
        await Task.WhenAny(bConnectedTaskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(3)));

        // 连接完成之后，预期现在无论是 a 还是 b 的 PeerManager 都有一次连接触发。但 IpcProvider 的 PeerConnected 只有 b 被动连接的一次触发
        Assert.AreEqual(1, aPeerManagerConnectedCount, "连接完成之后，无论主动连接，都能让 PeerManager.PeerConnected 事件触发");
        Assert.AreEqual(1, bPeerManagerConnectedCount, "连接完成之后，无论主动连接，都能让 PeerManager.PeerConnected 事件触发");

        Assert.AreEqual(0, aIpcProviderConnectedCount, "连接完成之后，只有被动连接才能让 IpcProvider.PeerConnected 事件触发。由于是 a 主动连接 b 的，因此 a 的 IpcProvider.PeerConnected 事件没有触发");
        Assert.AreEqual(1, bIpcProviderConnectedCount, "连接完成之后，只有被动连接才能让 IpcProvider.PeerConnected 事件触发。由于是 a 主动连接 b 的，因此 b 的 IpcProvider.PeerConnected 事件触发");

        // 当前连接只有一项
        Assert.AreEqual(1, a.PeerManager.CurrentConnectedPeerProxyCount);
        Assert.AreEqual(1, b.PeerManager.CurrentConnectedPeerProxyCount);

        Assert.AreEqual(1, a.PeerManager.GetCurrentConnectedPeerProxyList().Count);
        Assert.AreEqual(1, b.PeerManager.GetCurrentConnectedPeerProxyList().Count);

        // 现在 a 所连接的就是 b 的
        Assert.AreEqual(nameB, a.PeerManager.GetCurrentConnectedPeerProxyList()[0].PeerName);
        // 现在 b 所连接的就是 a 的
        Assert.AreEqual(nameA, b.PeerManager.GetCurrentConnectedPeerProxyList()[0].PeerName);
    }

    [TestMethod("a 主动连接 b 和 c，测试三端的 PeerManager.PeerConnected 事件和连接数量")]
    public async Task TestPeerManager2()
    {
        var nameA = "PeerManagerTest2_A";
        var nameB = "PeerManagerTest2_B";
        var nameC = "PeerManagerTest2_C";

        var a = new IpcProvider(nameA);
        var b = new IpcProvider(nameB);
        var c = new IpcProvider(nameC);

        var aPeerManagerConnectedCount = 0;
        var bPeerManagerConnectedCount = 0;
        var cPeerManagerConnectedCount = 0;

        a.PeerManager.PeerConnected += (sender, args) =>
        {
            aPeerManagerConnectedCount++;
        };

        b.PeerManager.PeerConnected += (sender, args) =>
        {
            bPeerManagerConnectedCount++;
        };

        c.PeerManager.PeerConnected += (sender, args) =>
        {
            cPeerManagerConnectedCount++;
        };

        var aIpcProviderConnectedCount = 0;
        var bIpcProviderConnectedCount = 0;
        var cIpcProviderConnectedCount = 0;

        a.PeerConnected += (sender, args) =>
        {
            aIpcProviderConnectedCount++;
        };

        var bConnectedTaskCompletionSource = new TaskCompletionSource();
        b.PeerConnected += (sender, args) =>
        {
            bIpcProviderConnectedCount++;
            bConnectedTaskCompletionSource.SetResult();
        };

        var cConnectedTaskCompletionSource = new TaskCompletionSource();
        c.PeerConnected += (sender, args) =>
        {
            cIpcProviderConnectedCount++;
            cConnectedTaskCompletionSource.SetResult();
        };

        a.StartServer();

        Task<PeerProxy> connectToBTask = a.GetAndConnectToPeerAsync(nameB);
        Task<PeerProxy> connectToCTask = a.GetAndConnectToPeerAsync(nameC);

        Assert.IsFalse(connectToBTask.IsCompleted, "由于此时 B 服务还没启动，必定现在还没连接完成");
        Assert.IsFalse(connectToCTask.IsCompleted, "由于此时 C 服务还没启动，必定现在还没连接完成");
        // 这里有一个小争议点，那就是事件名为 PeerConnected 但实际上可能没有完全完成连接的建立
        Assert.AreEqual(2, aPeerManagerConnectedCount, "a 主动发起连接 b 和 c，此时已有两个记录");
        Assert.AreEqual(0, bPeerManagerConnectedCount, "还没有完成建立连接，必定现在是 0 的值");
        Assert.AreEqual(0, cPeerManagerConnectedCount, "还没有完成建立连接，必定现在是 0 的值");

        Assert.AreEqual(0, aIpcProviderConnectedCount, "还没有完成建立连接，必定现在是 0 的值");
        Assert.AreEqual(0, bIpcProviderConnectedCount, "还没有完成建立连接，必定现在是 0 的值");
        Assert.AreEqual(0, cIpcProviderConnectedCount, "还没有完成建立连接，必定现在是 0 的值");

        // 开启 B 和 C 服务了，预期现在能够完成连接
        b.StartServer();
        c.StartServer();

        // 等待连接
        await Task.WhenAll(connectToBTask, connectToCTask);

        // 由于 b 和 c 是在另一个线程跑的，需要等待一下，避免多线程执行顺序，导致单元测试概率不通过
        await Task.WhenAny(
            Task.WhenAll(bConnectedTaskCompletionSource.Task, cConnectedTaskCompletionSource.Task),
            Task.Delay(TimeSpan.FromSeconds(3)));

        Assert.AreEqual(2, aPeerManagerConnectedCount, "连接完成之后，a 主动连接 b 和 c，PeerManager.PeerConnected 触发两次");
        Assert.AreEqual(1, bPeerManagerConnectedCount, "连接完成之后，b 被 a 连接，PeerManager.PeerConnected 触发一次");
        Assert.AreEqual(1, cPeerManagerConnectedCount, "连接完成之后，c 被 a 连接，PeerManager.PeerConnected 触发一次");

        Assert.AreEqual(0, aIpcProviderConnectedCount, "a 是主动连接方，IpcProvider.PeerConnected 事件不触发");
        Assert.AreEqual(1, bIpcProviderConnectedCount, "b 是被动连接方，IpcProvider.PeerConnected 事件触发一次");
        Assert.AreEqual(1, cIpcProviderConnectedCount, "c 是被动连接方，IpcProvider.PeerConnected 事件触发一次");

        Assert.AreEqual(2, a.PeerManager.CurrentConnectedPeerProxyCount);
        Assert.AreEqual(1, b.PeerManager.CurrentConnectedPeerProxyCount);
        Assert.AreEqual(1, c.PeerManager.CurrentConnectedPeerProxyCount);

        Assert.AreEqual(2, a.PeerManager.GetCurrentConnectedPeerProxyList().Count);
        Assert.AreEqual(1, b.PeerManager.GetCurrentConnectedPeerProxyList().Count);
        Assert.AreEqual(1, c.PeerManager.GetCurrentConnectedPeerProxyList().Count);

        // a 连接了 b 和 c，顺序不保证，用名称集合验证
        var aConnectedPeerNames = a.PeerManager.GetCurrentConnectedPeerProxyList().Select(p => p.PeerName).ToHashSet();
        Assert.IsTrue(aConnectedPeerNames.Contains(nameB), "a 连接的列表中应包含 b");
        Assert.IsTrue(aConnectedPeerNames.Contains(nameC), "a 连接的列表中应包含 c");

        // b 和 c 各自只连了 a
        Assert.AreEqual(nameA, b.PeerManager.GetCurrentConnectedPeerProxyList()[0].PeerName);
        Assert.AreEqual(nameA, c.PeerManager.GetCurrentConnectedPeerProxyList()[0].PeerName);
    }

    [TestMethod("b 和 c 主动连接 a，测试三端的 PeerManager.PeerConnected 事件和连接数量")]
    public async Task TestPeerManager3()
    {
        var nameA = "PeerManagerTest3_A";
        var nameB = "PeerManagerTest3_B";
        var nameC = "PeerManagerTest3_C";

        var a = new IpcProvider(nameA);
        var b = new IpcProvider(nameB);
        var c = new IpcProvider(nameC);

        var aPeerManagerConnectedCount = 0;
        var bPeerManagerConnectedCount = 0;
        var cPeerManagerConnectedCount = 0;

        a.PeerManager.PeerConnected += (sender, args) =>
        {
            aPeerManagerConnectedCount++;
        };

        b.PeerManager.PeerConnected += (sender, args) =>
        {
            bPeerManagerConnectedCount++;
        };

        c.PeerManager.PeerConnected += (sender, args) =>
        {
            cPeerManagerConnectedCount++;
        };

        var aIpcProviderConnectedCount = 0;
        var bIpcProviderConnectedCount = 0;
        var cIpcProviderConnectedCount = 0;

        var aAllConnectedTaskCompletionSource = new TaskCompletionSource();
        a.PeerConnected += (sender, args) =>
        {
            aIpcProviderConnectedCount++;
            if (aIpcProviderConnectedCount >= 2)
            {
                aAllConnectedTaskCompletionSource.TrySetResult();
            }
        };

        b.PeerConnected += (sender, args) =>
        {
            bIpcProviderConnectedCount++;
        };

        c.PeerConnected += (sender, args) =>
        {
            cIpcProviderConnectedCount++;
        };

        a.StartServer();
        b.StartServer();
        c.StartServer();

        Task<PeerProxy> bConnectToATask = b.GetAndConnectToPeerAsync(nameA);
        Task<PeerProxy> cConnectToATask = c.GetAndConnectToPeerAsync(nameA);

        // 这里有一个小争议点，那就是事件名为 PeerConnected 但实际上可能没有完全完成连接的建立
        Assert.AreEqual(1, bPeerManagerConnectedCount, "b 主动发起连接 a，此时 b 有一个记录");
        Assert.AreEqual(1, cPeerManagerConnectedCount, "c 主动发起连接 a，此时 c 有一个记录");

        Assert.AreEqual(0, bIpcProviderConnectedCount, "b 是主动连接方，IpcProvider.PeerConnected 事件不触发");
        Assert.AreEqual(0, cIpcProviderConnectedCount, "c 是主动连接方，IpcProvider.PeerConnected 事件不触发");

        // 等待连接
        await Task.WhenAll(bConnectToATask, cConnectToATask);

        // 由于 a 是在另一个线程跑的，需要等待一下，避免多线程执行顺序，导致单元测试概率不通过
        await Task.WhenAny(aAllConnectedTaskCompletionSource.Task, Task.Delay(TimeSpan.FromSeconds(3)));

        Assert.AreEqual(2, aPeerManagerConnectedCount, "连接完成之后，a 被 b 和 c 连接，PeerManager.PeerConnected 触发两次");
        Assert.AreEqual(1, bPeerManagerConnectedCount, "b 主动连接 a，PeerManager.PeerConnected 触发一次");
        Assert.AreEqual(1, cPeerManagerConnectedCount, "c 主动连接 a，PeerManager.PeerConnected 触发一次");

        Assert.AreEqual(2, aIpcProviderConnectedCount, "a 是被动连接方，IpcProvider.PeerConnected 事件触发两次");
        Assert.AreEqual(0, bIpcProviderConnectedCount, "b 是主动连接方，IpcProvider.PeerConnected 事件不触发");
        Assert.AreEqual(0, cIpcProviderConnectedCount, "c 是主动连接方，IpcProvider.PeerConnected 事件不触发");

        Assert.AreEqual(2, a.PeerManager.CurrentConnectedPeerProxyCount);
        Assert.AreEqual(1, b.PeerManager.CurrentConnectedPeerProxyCount);
        Assert.AreEqual(1, c.PeerManager.CurrentConnectedPeerProxyCount);

        Assert.AreEqual(2, a.PeerManager.GetCurrentConnectedPeerProxyList().Count);
        Assert.AreEqual(1, b.PeerManager.GetCurrentConnectedPeerProxyList().Count);
        Assert.AreEqual(1, c.PeerManager.GetCurrentConnectedPeerProxyList().Count);

        // a 被 b 和 c 连接，顺序不保证，用名称集合验证
        var aConnectedPeerNames = a.PeerManager.GetCurrentConnectedPeerProxyList().Select(p => p.PeerName).ToHashSet();
        Assert.IsTrue(aConnectedPeerNames.Contains(nameB), "a 连接的列表中应包含 b");
        Assert.IsTrue(aConnectedPeerNames.Contains(nameC), "a 连接的列表中应包含 c");

        // b 和 c 各自只连了 a
        Assert.AreEqual(nameA, b.PeerManager.GetCurrentConnectedPeerProxyList()[0].PeerName);
        Assert.AreEqual(nameA, c.PeerManager.GetCurrentConnectedPeerProxyList()[0].PeerName);
    }
}
