namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal interface IFakeIpcObjectBase
    {
        string? NullableStringProperty { get; set; }

        string? NonNullableStringProperty { get; set; }
    }
}
