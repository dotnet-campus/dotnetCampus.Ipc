using System;
using System.Reflection;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    [IpcPublic]
    internal interface IFakeIpcObject : IFakeIpcObjectBase
    {
        BindingFlags EnumProperty { get; set; }

        [IpcProperty(IsReadonly = true)]
        bool IpcReadonlyProperty { get; }

        IntPtr IntPtrProperty { get; }

        [IpcMethod(WaitsVoid = true)]
        void WaitsVoidMethod();

        [IpcMethod(WaitsVoid = false)]
        void NonWaitsVoidMethod();

        [IpcMethod(IgnoresIpcException = true)]
        Task MethodThatIgnoresIpcException();

        Task MethodThatThrowsIpcException();

        [IpcMethod(Timeout = 100)]
        Task MethodThatHasTimeout();

        [IpcMethod(DefaultReturn = "default1", IgnoresIpcException = true, Timeout = 100)]
        Task<string> MethodThatHasDefaultReturn();

        [IpcMethod(DefaultReturn = "default", IgnoresIpcException = true, Timeout = 100)]
        Task<object> MethodThatHasObjectWithObjectDefaultReturn();

        [IpcMethod(DefaultReturn = @"""default1""", IgnoresIpcException = true, Timeout = 100)]
        Task<object> MethodThatHasObjectWithStringDefaultReturn();

        // 请不要将这里的 String 改为 string，这是为了测试代码生成器能否处理类型而非关键字。
        [IpcMethod(DefaultReturn = "default1", IgnoresIpcException = true, Timeout = 100)]
        Task<String> MethodThatHasStringDefaultReturn();

        [IpcMethod(DefaultReturn = "new System.IntPtr(1)", IgnoresIpcException = true, Timeout = 100)]
        Task<IntPtr> MethodThatHasCustomDefaultReturn();

        [IpcMethod(DefaultReturn = "default1")]
        Task<string> MethodThatCannotBeCompiled_MustSetOtherAttributes();

        void MethodWithStructParameters(BindingFlags flags);

        bool MethodWithStructReturn();

        Task AsyncMethod();

        Task<(double a, uint b, int c, byte d)> AsyncMethodWithStructParametersAndStructReturn(double a, uint b, int c, byte d);

        Task<FakeIpcObjectSubModelA> AsyncMethodWithComplexParametersAndComplexReturn(FakeIpcObjectSubModelA model);

        Task<string> AsyncMethodWithPrimaryParametersAndPrimaryReturn(string source);
    }
}
