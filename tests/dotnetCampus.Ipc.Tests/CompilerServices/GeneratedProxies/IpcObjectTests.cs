using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests.CompilerServices.GeneratedProxies
{
    [TestClass]
    public class IpcObjectTests
    {
        [ContractTestCase]
        public void IpcTests()
        {
            "发送基本类型，收回元组，可 IPC 通信。".Test(async () =>
            {
                // 准备。
                var (peer, proxy) = await CreateIpcPairAsync("001");

                // 安放。
                var result = await proxy.AsyncMethodWithStructParametersAndStructReturn(1, 2, 3, 4);

                // 植物。
                Assert.AreEqual(new ValueTuple<double, uint, int, byte>(1, 2, 3, 4), result);
            });

            "发送复杂类型，收回复杂类型，可 IPC 通信。".Test(async () =>
            {
                // 准备。
                var (peer, proxy) = await CreateIpcPairAsync("002");

                // 安放。
                var result = await proxy.AsyncMethodWithComplexParametersAndComplexReturn(new FakeIpcObjectSubModelA(1, 2, 3, 4));

                // 植物。
                Assert.AreEqual((double) 1, result.A);
                Assert.AreEqual((uint) 2, result.B);
                Assert.AreEqual((int) 3, result.C);
                Assert.AreEqual((byte) 4, result.D);
            });

            "发送字符串，收回字符串，可 IPC 通信。".Test(async () =>
            {
                // 准备。
                var (peer, proxy) = await CreateIpcPairAsync("003");

                // 安放。
                var result = await proxy.AsyncMethodWithPrimaryParametersAndPrimaryReturn("Test");

                // 植物。
                Assert.AreEqual("Test", result);
            });
        }

        private async Task<(IPeerProxy peer, IFakeIpcObject proxy)> CreateIpcPairAsync(string name)
        {
            var aName = $"IpcObjectTests.IpcTests.{name}.A";
            var bName = $"IpcObjectTests.IpcTests.{name}.B";
            var aProvider = new IpcProvider(aName);
            var bProvider = new IpcProvider(bName);
            aProvider.StartServer();
            bProvider.StartServer();
            var aJoint = aProvider.CreateIpcJoint<IFakeIpcObject>(new FakeIpcObject());
            var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);
            var bProxy = bProvider.CreateIpcProxy<IFakeIpcObject, FakeIpcObject>(aPeer);
            return (aPeer, bProxy);
        }
    }
}
