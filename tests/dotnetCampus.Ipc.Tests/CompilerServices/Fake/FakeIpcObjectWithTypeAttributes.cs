using System;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Tests.CompilerServices.Fake;

[IpcPublic(typeof(IFakeIpcObject), IgnoresIpcException = true, Timeout = 100)]
internal class FakeIpcObjectWithTypeAttributes : FakeIpcObject
{
}
