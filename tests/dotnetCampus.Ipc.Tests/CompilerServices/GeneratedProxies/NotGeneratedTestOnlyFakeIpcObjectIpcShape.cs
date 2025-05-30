﻿#nullable disable
using System.Reflection;
using dotnetCampus.Ipc.CompilerServices.Attributes;
using dotnetCampus.Ipc.Tests.CompilerServices.Fake;

namespace dotnetCampus.Ipc.Tests.CompilerServices.GeneratedProxies
{
    [IpcShape(typeof(IFakeIpcObject))]
    internal class NotGeneratedTestOnlyFakeIpcObjectIpcShape : IFakeIpcObject
    {
        [IpcProperty]
        string? IFakeIpcObjectBase.NullableStringProperty { get; set; }

        [IpcProperty]
        string? IFakeIpcObjectBase.NonNullableStringProperty { get; set; }

        [IpcProperty]
        BindingFlags IFakeIpcObject.EnumProperty { get; set; }

        [IpcProperty]
        bool IFakeIpcObject.IpcReadonlyProperty { get; }

        [IpcProperty]
        IntPtr IFakeIpcObject.IntPtrProperty { get; }

        [IpcProperty]
        IntPtr? IFakeIpcObject.NullableIntPtrProperty { get; }

        [IpcProperty]
        INestedFakeIpcArgumentOrReturn IFakeIpcObject.NestedIpcProperty { get; set; }

        [IpcProperty]
        public List<string> ListProperty { get; set; }

        [IpcProperty]
        public IList<string> CollectionProperty { get; set; }

        [IpcProperty]
        public string[] ArrayProperty { get; set; }

        [IpcMethod]
        void IFakeIpcObject.WaitsVoidMethod()
        {
            throw null;
        }

        [IpcMethod]
        void IFakeIpcObject.NonWaitsVoidMethod()
        {
            throw null;
        }

        [IpcMethod]
        Task IFakeIpcObject.MethodThatIgnoresIpcException()
        {
            throw null;
        }

        [IpcMethod]
        Task IFakeIpcObject.MethodThatThrowsIpcException()
        {
            throw null;
        }

        [IpcMethod]
        Task IFakeIpcObject.MethodThatHasTimeout()
        {
            throw null;
        }

        [IpcMethod]
        Task<string?> IFakeIpcObject.MethodThatHasAsyncNullableReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<FakeIpcObjectSubModelA?> IFakeIpcObject.MethodThatHasAsyncNullableComplexReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<string> IFakeIpcObject.MethodThatHasDefaultReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<object> IFakeIpcObject.MethodThatHasObjectWithObjectDefaultReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<object> IFakeIpcObject.MethodThatHasObjectWithStringDefaultReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<string> IFakeIpcObject.MethodThatHasStringDefaultReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<IntPtr> IFakeIpcObject.MethodThatHasCustomDefaultReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<string> IFakeIpcObject.MethodThatCannotBeCompiled_MustSetOtherAttributes()
        {
            throw null;
        }

        public Task<List<string>> MethodWithListParametersAndListReturn(List<string> a, List<string> b)
        {
            throw null;
        }

        public Task<IList<string>> MethodWithCollectionParametersAndCollectionReturn(IList<string> a, IList<string> b)
        {
            throw null;
        }

        public Task<string[]> MethodWithArrayParametersAndArrayReturn(string[] a, string[] b)
        {
            throw null;
        }

        [IpcMethod]
        void IFakeIpcObject.MethodWithStructParameters(BindingFlags flags)
        {
            throw null;
        }

        [IpcMethod]
        bool IFakeIpcObject.MethodWithStructReturn()
        {
            throw null;
        }

        public int MethodWithSameParameterCountOverloading(int a, int b)
        {
            throw null;
        }

        public long MethodWithSameParameterCountOverloading(long a, long b)
        {
            throw null;
        }

        [IpcMethod]
        IFakeIpcObject.NestedEnum IFakeIpcObject.MethodWithNestedEnumReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task<IFakeIpcObject.NestedEnum> IFakeIpcObject.AsyncMethodWithNestedEnumReturn()
        {
            throw null;
        }

        [IpcMethod]
        Task IFakeIpcObject.AsyncMethod()
        {
            throw null;
        }

        [IpcMethod]
        Task<INestedFakeIpcArgumentOrReturn> IFakeIpcObject.AsyncMethodWithIpcPublicObjectParametersAndIpcPublicObjectReturn(INestedFakeIpcArgumentOrReturn nested, string changeValue)
        {
            throw null;
        }

        [IpcMethod]
        Task<(double a, uint b, int? c, byte d)> IFakeIpcObject.AsyncMethodWithStructParametersAndStructReturn(double a, uint b, int? c, byte d)
        {
            throw null;
        }

        [IpcMethod]
        Task<(FakeIpcObjectSubModelA a, FakeIpcObjectSubModelA? b, IntPtr c, IntPtr? d)> IFakeIpcObject.AsyncMethodWithComplexValueTupleParametersAndComplexValueTupleReturn(FakeIpcObjectSubModelA a, FakeIpcObjectSubModelA? b, IntPtr c, IntPtr? d)
        {
            throw null;
        }

        [IpcMethod]
        Task<FakeIpcObjectSubModelA> IFakeIpcObject.AsyncMethodWithComplexParametersAndComplexReturn(FakeIpcObjectSubModelA model)
        {
            throw null;
        }

        [IpcMethod]
        Task<string> IFakeIpcObject.AsyncMethodWithPrimaryParametersAndPrimaryReturn(string source)
        {
            throw null;
        }
    }
}
