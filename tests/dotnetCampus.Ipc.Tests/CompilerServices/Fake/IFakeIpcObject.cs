using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal interface IFakeIpcObject
    {
        Task<(double a, uint b, int c, byte d)> ConstructAsync(double a, uint b, int c, byte d);
        Task<FakeIpcObjectSubModelA> ConvertObjectAsync(FakeIpcObjectSubModelA model);
        Task<string> ConvertStringAsync(string source);
    }
}
