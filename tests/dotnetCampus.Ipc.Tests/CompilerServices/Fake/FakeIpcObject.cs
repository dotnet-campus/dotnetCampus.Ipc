using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    [IpcPublic(typeof(IFakeIpcObject), typeof(FakeIpcObjectIpcProxy), typeof(FakeIpcObjectIpcJoint))]
    internal class FakeIpcObject : IFakeIpcObject
    {
        public async Task<(double a, uint b, int c, byte d)> ConstructAsync(double a, uint b, int c, byte d)
        {
            return (a, b, c, d);
        }

        public async Task<FakeIpcObjectSubModelA> ConvertObjectAsync(FakeIpcObjectSubModelA model)
        {
            return new FakeIpcObjectSubModelA(model.A, model.B, model.C, model.D);
        }

        public async Task<string> ConvertStringAsync(string source)
        {
            return source;
        }
    }
}
