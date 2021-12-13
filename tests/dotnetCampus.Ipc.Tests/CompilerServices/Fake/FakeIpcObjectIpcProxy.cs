using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal class FakeIpcObjectIpcProxy : GeneratedIpcProxy<IFakeIpcObject>, IFakeIpcObject
    {
        public Task<(double a, uint b, int c, byte d)> ConstructAsync(double a, uint b, int c, byte d)
        {
            return CallMethodAsync<(double a, uint b, int c, byte d)>(new object[] { a, b, c, d });
        }

        public Task<FakeIpcObjectSubModelA> ConvertObjectAsync(FakeIpcObjectSubModelA model)
        {
            return CallMethodAsync<FakeIpcObjectSubModelA>(new object[] { model });
        }

        public Task<string> ConvertStringAsync(string source)
        {
            return CallMethodAsync<string>(new object[] { source });
        }
    }
}
