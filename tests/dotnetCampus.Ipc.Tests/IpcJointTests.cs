using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.Pipes;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using Moq;

using MSTest.Extensions.Contracts;

namespace dotnetCampus.Ipc.Tests
{
    [TestClass]
    public class IpcJointTests
    {
        [ContractTestCase]
        public void IpcCallMethod()
        {
            "通过 CreateIpcJoint 方法，能够创建 IPC 关联逻辑，实现跨进程调用对象方法".Test(async () =>
            {
                // A 客户端
                var ipcProviderA = new IpcProvider();
                // B 服务端
                var ipcProviderB = new IpcProvider();

                var mock = new Mock<IIpcJointTestObject>();
                var ipcJointTestRealObject = new IpcJointTestRealObject(mock.Object);

                // 注册关联
                ipcProviderB.CreateIpcJoint<IIpcJointTestObject, IpcJointTestObjectIpcJoint>(ipcJointTestRealObject);

                ipcProviderA.StartServer();
                ipcProviderB.StartServer();

                // 先让 A 连接到 B 上，连接之后，才能向 B 获取到对象代理
                var peer = await ipcProviderA.GetAndConnectToPeerAsync(ipcProviderB.IpcContext.PipeName);

                // 通过在客户端里，创建对象代理，即可拿到代理。代理是给本机使用，但实际执行逻辑，是发送远程调用
                var ipcJointTestObjectProxy = ipcProviderA.CreateIpcProxy<IIpcJointTestObject, IpcJointTestObjectIpcProxy>(peer);

                // 无参，无返回值方法
                await Task.Run(() =>
                {
                    // 在客户端里调用无参，无返回值方法。实际可以是在服务端执行
                    ipcJointTestObjectProxy.TestMethod1();
                    // 判断是在服务端执行
                    mock.Verify(t => t.TestMethod1(), Times.Once);
                });

                // 带基础类型参数，无返回值方法
                await Task.Run(() =>
                {
                    string a0 = "lindexi is doubi";
                    int a1 = 2;

                    ipcJointTestObjectProxy.TestMethod2(a0, a1);
                    mock.Verify(t => t.TestMethod2(a0, a1), Times.Once);
                });
            });
        }
    }

    /// <summary>
    /// 定义 IPC 接口
    /// </summary>
    public interface IIpcJointTestObject
    {
        void TestMethod2(string arg1, int arg2);
        void TestMethod1();
    }

    /// <summary>
    /// 实际的对象，包含实际的业务逻辑
    /// </summary>
    [IpcPublic(typeof(IIpcJointTestObject), typeof(IpcJointTestObjectIpcProxy), typeof(IpcJointTestObjectIpcJoint))]
    class IpcJointTestRealObject : IIpcJointTestObject
    {
        public IpcJointTestRealObject(IIpcJointTestObject mockObject)
        {
            _mockObject = mockObject;
        }

        /// <summary>
        /// 用于给单元测试的假的对象
        /// </summary>
        private readonly IIpcJointTestObject _mockObject;

        public void TestMethod2(string arg1, int arg2)
        {
            _mockObject.TestMethod2(arg1, arg2);
        }

        public void TestMethod1()
        {
            _mockObject.TestMethod1();
        }
    }

    /// <summary>
    /// 这是在 IPC 服务端执行的代码，用于关联 IPC 客户端发过来的请求的处理方式，如何对应到实际的对象
    /// </summary>
    /// 此为生成代码
    class IpcJointTestObjectIpcJoint : GeneratedIpcJoint<IIpcJointTestObject>
    {
        protected override void MatchMembers(IIpcJointTestObject real)
        {
            MatchMethod(nameof(IIpcJointTestObject.TestMethod1), new Action(() => real.TestMethod1()));
            MatchMethod(nameof(IIpcJointTestObject.TestMethod2), new Action<string, int>((string a0, int a1) => real.TestMethod2(a0, a1)));
        }
    }

    /// <summary>
    /// 这是在 IPC 客户端执行的代码，给 IPC 客户端的各个业务使用的实际对象，此对象里面的所有方法都是调用 IPC 发送请求到 IPC 服务端执行
    /// </summary>
    /// 此为生成代码
    class IpcJointTestObjectIpcProxy : GeneratedIpcProxy<IIpcJointTestObject>, IIpcJointTestObject
    {
        public void TestMethod2(string arg1, int arg2)
        {
            CallMethod(new object[] { arg1, arg2 }).Wait();
        }

        public void TestMethod1()
        {
            CallMethod().Wait();
        }
    }
}
