using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;
using dotnetCampus.Ipc.Tests.CompilerServices.Fake;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal class FakeIpcObject : IFakeIpcObject
    {
        private BindingFlags _enumProperty = BindingFlags.Public;
        private bool _ipcReadonlyProperty = true;
#nullable enable
        private INestedFakeIpcArgumentOrReturn? _nestedIpcObject;
#nullable restore

        public FakeIpcObject()
        {
        }

        public FakeIpcObject(INestedFakeIpcArgumentOrReturn jointSideObject)
        {
            _nestedIpcObject = jointSideObject;
        }

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

        public bool IpcReadonlyProperty => _ipcReadonlyProperty;

        public void SetIpcReadonlyProperty(bool value)
        {
            _ipcReadonlyProperty = value;
        }

        public IntPtr IntPtrProperty { get; } = new IntPtr(1);

        public IntPtr? NullableIntPtrProperty { get; }

        public INestedFakeIpcArgumentOrReturn NestedIpcProperty
        {
            get => _nestedIpcObject;
            set => _nestedIpcObject = value;
        }

        public List<string>? ListProperty { get; set; } = new List<string>() { "List1", "List2" };

        public IList<string>? CollectionProperty { get; set; } = new List<string>() { "Collection1", "Collection2" };

        public string[]? ArrayProperty { get; set; } = new string[] { "Array1", "Array2" };

        public void WaitsVoidMethod()
        {
            Thread.Sleep(100);
            EnumProperty = BindingFlags.Public | BindingFlags.Instance;
        }

        public void NonWaitsVoidMethod()
        {
            Thread.Sleep(100);
            EnumProperty = BindingFlags.Public | BindingFlags.Instance;
        }

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

        public Task MethodThatHasTimeout()
        {
            return Task.Delay(150);
        }

#nullable enable
        public async Task<string?> MethodThatHasAsyncNullableReturn()
        {
            return null;
        }
#nullable restore

#nullable enable
        public async Task<FakeIpcObjectSubModelA?> MethodThatHasAsyncNullableComplexReturn()
        {
            return null;
        }
#nullable restore

        public async Task<string> MethodThatHasDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        public async Task<object> MethodThatHasObjectWithObjectDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        public async Task<object> MethodThatHasObjectWithStringDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        // 请不要将这里的 String 改为 string，这是为了测试代码生成器能否处理类型而非关键字。
        public async Task<String> MethodThatHasStringDefaultReturn()
        {
            await Task.Delay(250);
            return "xxx";
        }

        public async Task<IntPtr> MethodThatHasCustomDefaultReturn()
        {
            await Task.Delay(150);
            return IntPtr.Zero;
        }

        public async Task<string> MethodThatCannotBeCompiled_MustSetOtherAttributes()
        {
            await Task.Delay(150);
            return "xxx";
        }

        public async Task<List<string>> MethodWithListParametersAndListReturn(List<string> a, List<string> b)
        {
            await Task.Delay(1);
            return a.Concat(b).ToList();
        }

        public async Task<IList<string>> MethodWithCollectionParametersAndCollectionReturn(IList<string> a, IList<string> b)
        {
            await Task.Delay(1);
            return a.Concat(b).ToList();
        }

        public async Task<string[]> MethodWithArrayParametersAndArrayReturn(string[] a, string[] b)
        {
            await Task.Delay(1);
            return a.Concat(b).ToArray();
        }

        public void MethodWithStructParameters(BindingFlags flags)
        {
        }

        public bool MethodWithStructReturn()
        {
            return IpcReadonlyProperty;
        }

        public int MethodWithSameParameterCountOverloading(int a, int b)
        {
            return a + b;
        }

        public long MethodWithSameParameterCountOverloading(long a, long b)
        {
            return a + b;
        }

        public IFakeIpcObject.NestedEnum MethodWithNestedEnumReturn()
        {
            return IFakeIpcObject.NestedEnum.None;
        }

        public Task<IFakeIpcObject.NestedEnum> AsyncMethodWithNestedEnumReturn()
        {
            return Task.FromResult(IFakeIpcObject.NestedEnum.None);
        }

        public async Task AsyncMethod()
        {
            await Task.Delay(2000);
        }

        public async Task<INestedFakeIpcArgumentOrReturn> AsyncMethodWithIpcPublicObjectParametersAndIpcPublicObjectReturn(INestedFakeIpcArgumentOrReturn nested, string changeValue)
        {
            // 修改来自参数所在进程的 IPC 对象的值。
            await Task.Run(() =>
            {
                nested.Value = changeValue;
            });

            // 返回自己进程的值给对方进程。
            return _nestedIpcObject;
        }

#nullable enable
        public Task<(double a, uint b, int? c, byte d)> AsyncMethodWithStructParametersAndStructReturn(double a, uint b, int? c, byte d)
        {
            return Task.FromResult((a, b, c, d));
        }
#nullable restore

#nullable enable
        public Task<(FakeIpcObjectSubModelA a, FakeIpcObjectSubModelA? b, IntPtr c, IntPtr? d)> AsyncMethodWithComplexValueTupleParametersAndComplexValueTupleReturn(FakeIpcObjectSubModelA a, FakeIpcObjectSubModelA? b, IntPtr c, IntPtr? d)
        {
            return Task.FromResult((new FakeIpcObjectSubModelA(), (FakeIpcObjectSubModelA?) null, IntPtr.Zero, (IntPtr?) null));
        }
#nullable restore

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
