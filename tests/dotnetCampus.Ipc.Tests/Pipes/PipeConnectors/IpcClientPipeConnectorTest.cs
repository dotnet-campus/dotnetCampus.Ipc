using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Pipes.PipeConnectors;
using dotnetCampus.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests.Pipes.PipeConnectors
{
    [TestClass]
    public class IpcClientPipeConnectorTest
    {
        [ContractTestCase]
        public void ReConnectBreak()
        {
            "重连接时，调用 CanContinue 方法返回不支持再次重新连接，则不再次重新连接".Test(async () =>
            {
                int callCanContinueCount = 0;
                var asyncAutoResetEvent = new AsyncAutoResetEvent(false);
                var ipcConfiguration = new IpcConfiguration()
                {
                    AutoReconnectPeers = true,
                    IpcClientPipeConnector = new IpcClientPipeConnector(c =>
                    {
                        // 调用 CanContinue 方法返回不支持
                        callCanContinueCount++;
                        asyncAutoResetEvent.Set();
                        return false;
                    }, stepTimeout: TimeSpan.FromMilliseconds(100)),
                };

                var ipcProviderA = new IpcProvider(Guid.NewGuid().ToString("N"), ipcConfiguration);
                var ipcProviderB = new IpcProvider();
                ipcProviderA.StartServer();
                ipcProviderB.StartServer();

                var peer = await ipcProviderA.GetAndConnectToPeerAsync(ipcProviderB.IpcContext.PipeName);
                Assert.AreEqual(0, callCanContinueCount);
                Assert.IsNotNull(peer);

                // 断开，预期此时将会重新连接
                ipcProviderB.Dispose();

                await asyncAutoResetEvent.WaitOneAsync();
                await Task.Delay(TimeSpan.FromSeconds(1));
                Assert.AreEqual(true, peer.IsBroken);
                Assert.AreEqual(1, callCanContinueCount);
            });
        }

        [ContractTestCase]
        public void ConnectNamedPipeAsync()
        {
            "连接一个不存在的服务，会调用到 CanContinue 方法判断是否可以再次重新连接。如 CanContinue 返回不能继续连接，将抛出连接失败异常".Test(async () =>
            {
                int callCanContinueCount = 0;
                var ipcConfiguration = new IpcConfiguration()
                {
                    IpcClientPipeConnector = new IpcClientPipeConnector(c =>
                    {
                        callCanContinueCount++;
                        // 第一次返回可以继续连接，预期进来第二次
                        return callCanContinueCount == 1;
                    }, stepTimeout: TimeSpan.FromMilliseconds(100)),
                };

                var ipcProviderA = new IpcProvider(Guid.NewGuid().ToString("N"), ipcConfiguration);
                ipcProviderA.StartServer();

                await Assert.ThrowsExceptionAsync<IpcClientPipeConnectionException>(async () =>
                {
                    // 连接一个不存在的服务
                    var peer = await ipcProviderA.GetAndConnectToPeerAsync("NotExists_" + Guid.NewGuid().ToString("N"));
                    Assert.IsNull(peer);
                });

                // 调用两次，第一次返回可以继续连接
                Assert.AreEqual(2, callCanContinueCount);
            });

            "连接能立刻连上的服务，不会调用到 CanContinue 方法判断是否可以再次重新连接".Test(async () =>
            {
                int callCanContinueCount = 0;
                var ipcConfiguration = new IpcConfiguration()
                {
                    IpcClientPipeConnector = new IpcClientPipeConnector(c =>
                    {
                        callCanContinueCount++;
                        return true;
                    }),
                };

                var ipcProviderA = new IpcProvider(Guid.NewGuid().ToString("N"), ipcConfiguration);
                var ipcProviderB = new IpcProvider();
                ipcProviderA.StartServer();
                ipcProviderB.StartServer();

                var peer = await ipcProviderA.GetAndConnectToPeerAsync(ipcProviderB.IpcContext.PipeName);

                // 不会调用到 CanContinue 方法判断是否可以再次重新连接
                Assert.AreEqual(0, callCanContinueCount);
                Assert.IsNotNull(peer);
            });
        }
    }
}
