﻿using dotnetCampus.Ipc.Tests.CompilerServices;
using System.Threading.Tasks;
using System.Xml.Linq;

using MSTest.Extensions.Contracts;
using dotnetCampus.Ipc.Pipes;
using dotnetCampus.Ipc.Messages;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using dotnetCampus.Ipc.Context;
using System.Threading;
using dotnetCampus.Ipc.Internals;

namespace dotnetCampus.Ipc.Tests.Threading;

[TestClass]
public class IpcThreadPoolTests : IIpcRequestHandler
{
    [ContractTestCase]
    public void RunRecursively()
    {
        "线程池满，等待新任务调度时，本递归调用不应出现 StackOverflowException。".Test(async () =>
        {
            var aName = $"IpcObjectTests.IpcTests.IpcThreadPoolTests.{nameof(RunRecursively)}.A";
            var bName = $"IpcObjectTests.IpcTests.IpcThreadPoolTests.{nameof(RunRecursively)}.B";
            var aProvider = new IpcProvider(aName, new IpcConfiguration
            {
                DefaultIpcRequestHandler = this,
            });
            var bProvider = new IpcProvider(bName);
            aProvider.StartServer();
            bProvider.StartServer();
            var aPeer = await bProvider.GetAndConnectToPeerAsync(aName);

            // 占满线程池，以便有机会等待新任务调度。
            var task1 = aPeer.GetResponseAsync(new IpcMessage("Test", Encoding.UTF8.GetBytes("Test")));
            var task2 = aPeer.GetResponseAsync(new IpcMessage("Test", Encoding.UTF8.GetBytes("Test")));
            var task3 = aPeer.GetResponseAsync(new IpcMessage("Test", Encoding.UTF8.GetBytes("Test")));
            var task4 = aPeer.GetResponseAsync(new IpcMessage("Test", Encoding.UTF8.GetBytes("Test")));
            var task5 = aPeer.GetResponseAsync(new IpcMessage("Test", Encoding.UTF8.GetBytes("Test")));
            var task6 = aPeer.GetResponseAsync(new IpcMessage("Test", Encoding.UTF8.GetBytes("Test")));
            var task7 = aPeer.GetResponseAsync(new IpcMessage("Test", Encoding.UTF8.GetBytes("Test")));

            await Task.WhenAll(task1, task2, task3, task4, task5);
        });
    }

    public Task<IIpcResponseMessage> HandleRequest(IIpcRequestContext requestContext)
    {
        Thread.Sleep(100);
        return Task.FromResult<IIpcResponseMessage>(
            new IpcHandleRequestMessageResult(
                new IpcMessage(
                    "TestR",
                    Encoding.UTF8.GetBytes("TestR")
                )
            )
        );
    }
}
