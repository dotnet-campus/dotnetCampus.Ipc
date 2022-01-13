using System;
using System.Reflection;
using System.Threading.Tasks;

using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal interface IFakeIpcObject
    {
#nullable enable
        string? NullableStringProperty { get; set; }
#nullable restore
#nullable disable
        string? NonNullableStringProperty { get; set; }
#nullable restore
        BindingFlags EnumProperty { get; set; }
        bool IpcReadonlyProperty { get; }
        IntPtr IntPtrProperty { get; }

        Task AsyncMethod();
        Task<FakeIpcObjectSubModelA> AsyncMethodWithComplexParametersAndComplexReturn(FakeIpcObjectSubModelA model);
        Task<string> AsyncMethodWithPrimaryParametersAndPrimaryReturn(string source);
        Task<(double a, uint b, int c, byte d)> AsyncMethodWithStructParametersAndStructReturn(double a, uint b, int c, byte d);
        Task<string> MethodThatCannotBeCompiled_MustSetOtherAttributes();
        Task<IntPtr> MethodThatHasCustomDefaultReturn();
        Task<string> MethodThatHasDefaultReturn();
        Task<object> MethodThatHasObjectWithObjectDefaultReturn();
        Task<object> MethodThatHasObjectWithStringDefaultReturn();
        // 请不要将这里的 String 改为 string，这是为了测试代码生成器能否处理类型而非关键字。
        Task<String> MethodThatHasStringDefaultReturn();
        Task MethodThatHasTimeout();
        Task MethodThatIgnoresIpcException();
        Task MethodThatThrowsIpcException();
        void MethodWithStructParameters(BindingFlags flags);
        bool MethodWithStructReturn();
        void NonWaitsVoidMethod();
        void WaitsVoidMethod();
    }
}
