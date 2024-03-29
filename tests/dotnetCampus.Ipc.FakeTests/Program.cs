﻿using System.Diagnostics;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
using dotnetCampus.Ipc.FakeTests.FakeApis;
using dotnetCampus.Ipc.Pipes;

Console.WriteLine("【IPC 远程进程单元测试】");
var ipcPeerName = $"IpcObjectTests.IpcTests.RemoteFakeIpcObject";
var ipcProvider = new IpcProvider(ipcPeerName);
Console.WriteLine("IPC 服务启动中...");
ipcProvider.StartServer();
Console.CursorTop--;
Console.WriteLine("IPC 服务已启动");
var jointObject = new RemoteFakeIpcObject();
Console.WriteLine("IPC 对接创建中...");
ipcProvider.CreateIpcJoint<IRemoteFakeIpcObject>(jointObject);
Console.CursorTop--;
Console.WriteLine("IPC 对接已创建");
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("IPC 交互中...");
Console.ResetColor();
await jointObject.WaitForShutdownAsync();
Console.WriteLine("IPC 收到退出信号");
Thread.Sleep(1000);
Console.WriteLine("IPC 远程进程单元测试已退出");
