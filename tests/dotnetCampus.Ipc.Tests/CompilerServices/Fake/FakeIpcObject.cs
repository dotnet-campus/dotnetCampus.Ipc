using System;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    [IpcPublic(typeof(IFakeIpcObject))]
    internal class FakeIpcObject : IFakeIpcObject
    {
        private BindingFlags _bindingFlags = BindingFlags.Public;

#nullable enable
        public string? Title { get; set; } = "Title";
#nullable restore

#nullable disable
        public string? Description { get; set; } = "Description";
#nullable restore

        public BindingFlags BindingFlags
        {
            get => _bindingFlags;
            set => _bindingFlags = value;
        }

        [IpcProperty(IsReadonly = true)]
        public bool IsFake { get; } = true;

        public IntPtr Handle { get; } = new IntPtr(1);

        [IpcMethod(WaitsVoid = true)]
        public void Activate()
        {
            Thread.Sleep(100);
            BindingFlags = BindingFlags.Public | BindingFlags.Instance;
        }

        [IpcMethod(WaitsVoid = false)]
        public void Deactivate()
        {
        }

        public void Call(BindingFlags flags)
        {
        }

        public bool CheckFake()
        {
            return IsFake;
        }

        public async Task DoSomethingAsync()
        {
        }

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
