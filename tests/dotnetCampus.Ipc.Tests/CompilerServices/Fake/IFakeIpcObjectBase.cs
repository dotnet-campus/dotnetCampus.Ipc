namespace dotnetCampus.Ipc.Tests.CompilerServices
{
    internal interface IFakeIpcObjectBase
    {
#nullable enable
        string? NullableStringProperty { get; set; }
#nullable restore
#nullable disable
        string? NonNullableStringProperty { get; set; }
#nullable restore
    }
}
