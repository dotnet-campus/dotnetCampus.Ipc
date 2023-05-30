using dotnetCampus.Ipc.FakeTests.FakeApis;

namespace dotnetCampus.Ipc.Tests.CompilerServices.FakeRemote;
public class RemoteIpcArgument : IRemoteFakeIpcArgumentOrReturn
{
    public RemoteIpcArgument(string value)
    {
        Value = value;
    }

    public string Value { get; set; }
}
