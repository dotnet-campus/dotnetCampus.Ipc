using System;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Pipes.PipeConnectors;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests.Pipes.PipeConnectors
{
    [TestClass]
    public class IpcClientPipeConnectorTest
    {
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

                await Assert.ThrowsExceptionAsync<IpcClientPipeConnectFailException>(async () =>
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
