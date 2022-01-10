using System;
using System.Reflection;
using System.Threading.Tasks;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal interface IFakeIpcObject
    {
#nullable enable
        string? Title { get; set; }
#nullable restore

#nullable disable
        string? Description { get; set; }
#nullable restore

        BindingFlags BindingFlags { get; set; }

        bool IsFake { get; }

        IntPtr Handle { get; }

        void Activate();

        void Deactivate();

        bool CheckFake();

        void Call(BindingFlags flags);

        Task DoSomethingAsync();

        Task<(double a, uint b, int c, byte d)> ConstructAsync(double a, uint b, int c, byte d);

        Task<FakeIpcObjectSubModelA> ConvertObjectAsync(FakeIpcObjectSubModelA model);

        Task<string> ConvertStringAsync(string source);
    }
}
