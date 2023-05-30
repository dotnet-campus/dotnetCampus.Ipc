using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.FakeTests.FakeApis;
[IpcPublic]
public interface IRemoteFakeIpcArgumentOrReturn
{
    string Value { get; set; }
}
