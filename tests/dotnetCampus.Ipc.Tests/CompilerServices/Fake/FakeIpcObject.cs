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
        private BindingFlags _enumProperty = BindingFlags.Public;
        private bool _ipcReadonlyProperty = true;

#nullable enable
        public string? NullableStringProperty { get; set; } = "Title";
#nullable restore

#nullable disable
        public string? NonNullableStringProperty { get; set; } = "Description";
#nullable restore

        public BindingFlags EnumProperty
        {
            get => _enumProperty;
            set => _enumProperty = value;
        }

        [IpcProperty(IsReadonly = true)]
        public bool IpcReadonlyProperty => _ipcReadonlyProperty;

        public void SetIpcReadonlyProperty(bool value)
        {
            _ipcReadonlyProperty = value;
        }

        public IntPtr IntPtrProperty { get; } = new IntPtr(1);

        [IpcMethod(WaitsVoid = true)]
        public void WaitsVoidMethod()
        {
            Thread.Sleep(100);
            EnumProperty = BindingFlags.Public | BindingFlags.Instance;
        }

        [IpcMethod(WaitsVoid = false)]
        public void NonWaitsVoidMethod()
        {
            Thread.Sleep(100);
            EnumProperty = BindingFlags.Public | BindingFlags.Instance;
        }

        [IpcMethod(IgnoresIpcException = true)]
        public Task MethodThatIgnoresIpcException()
        {
            Thread.Sleep(100);
            return Task.CompletedTask;
        }

        public Task MethodThatThrowsIpcException()
        {
            Thread.Sleep(100);
            return Task.CompletedTask;
        }

        [IpcMethod(Timeout = 100)]
        public Task MethodThatHasTimeout()
        {
            return Task.Delay(150);
        }

        [IpcMethod(DefaultReturn = "default1", IgnoresIpcException = true, Timeout = 100)]
        public async Task<string> MethodThatHasDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        [IpcMethod(DefaultReturn = "default", IgnoresIpcException = true, Timeout = 100)]
        public async Task<object> MethodThatHasObjectWithObjectDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        [IpcMethod(DefaultReturn = @"""default1""", IgnoresIpcException = true, Timeout = 100)]
        public async Task<object> MethodThatHasObjectWithStringDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        // 请不要将这里的 String 改为 string，这是为了测试代码生成器能否处理类型而非关键字。
        [IpcMethod(DefaultReturn = "default1", IgnoresIpcException = true, Timeout = 100)]
        public async Task<String> MethodThatHasStringDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        [IpcMethod(DefaultReturn = "new IntPtr(1)", IgnoresIpcException = true, Timeout = 100)]
        public async Task<IntPtr> MethodThatHasCustomDefaultReturn()
        {
            await Task.Delay(150);
            return IntPtr.Zero;
        }

        [IpcMethod(DefaultReturn = "default1")]
        public async Task<string> MethodThatCannotBeCompiled_MustSetOtherAttributes()
        {
            await Task.Delay(150);
            return "xxx";
        }

        public void MethodWithStructParameters(BindingFlags flags)
        {
        }

        public bool MethodWithStructReturn()
        {
            return IpcReadonlyProperty;
        }

        public Task AsyncMethod()
        {
            return Task.CompletedTask;
        }

        public Task<(double a, uint b, int c, byte d)> AsyncMethodWithStructParametersAndStructReturn(double a, uint b, int c, byte d)
        {
            return Task.FromResult((a, b, c, d));
        }

        public Task<FakeIpcObjectSubModelA> AsyncMethodWithComplexParametersAndComplexReturn(FakeIpcObjectSubModelA model)
        {
            return Task.FromResult(new FakeIpcObjectSubModelA(model.A, model.B, model.C, model.D));
        }

        public Task<string> AsyncMethodWithPrimaryParametersAndPrimaryReturn(string source)
        {
            return Task.FromResult(source);
        }
    }
}
