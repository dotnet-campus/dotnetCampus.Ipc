using System;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal class FakeIpcObjectIpcJoint : GeneratedIpcJoint<IFakeIpcObject>
    {
        protected override void MatchMembers(IFakeIpcObject real)
        {
            MatchMethod(nameof(IFakeIpcObject.ConstructAsync), new Func<double, uint, int, byte, Task<(double a, uint b, int c, byte d)>>((a, b, c, d) => real.ConstructAsync(a, b, c, d)));
            MatchMethod(nameof(IFakeIpcObject.ConvertObjectAsync), new Func<FakeIpcObjectSubModelA, Task<FakeIpcObjectSubModelA>>((a) => real.ConvertObjectAsync(a)));
            MatchMethod(nameof(IFakeIpcObject.ConvertStringAsync), new Func<string, Task<string>>((a) => real.ConvertStringAsync(a)));
        }
    }
}
