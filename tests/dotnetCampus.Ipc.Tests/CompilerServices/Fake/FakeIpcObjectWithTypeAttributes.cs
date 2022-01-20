using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Tests.CompilerServices.Fake;

[IpcPublic(IgnoresIpcException = true, Timeout = 100)]
internal interface IFakeIpcObjectWithTypeAttributes : IFakeIpcObject
{
}

internal class FakeIpcObjectWithTypeAttributes : FakeIpcObject, IFakeIpcObjectWithTypeAttributes
{
}
