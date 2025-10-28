using dotnetCampus.Ipc.FakeTests.FakeApis;

namespace dotnetCampus.Ipc.Tests.CompilerServices.FakeRemote
{
    public class RemoteIpcReturn : IRemoteFakeIpcArgumentOrReturn
    {
        public RemoteIpcReturn(string value)
        {
            Value = value;
        }

        public string Value { get; set; }
    }
}
