using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Tests.CompilerServices.Fake;

[IpcPublic]
internal interface INestedFakeIpcArgumentOrReturn
{
    string Value { get; set; }
}

internal sealed class FakeNestedIpcArgumentOrReturn : INestedFakeIpcArgumentOrReturn
{
    public FakeNestedIpcArgumentOrReturn(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
}
