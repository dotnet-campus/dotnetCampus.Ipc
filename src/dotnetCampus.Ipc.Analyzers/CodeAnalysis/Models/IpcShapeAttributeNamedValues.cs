namespace dotnetCampus.Ipc.CodeAnalysis.Models;

/// <summary>
/// 仅供 GeneratedIpcProxy 的自动生成的派生类与基类传递参数使用，包含参数传递所需的各种个性化需求。
/// </summary>
internal class IpcShapeAttributeNamedValues : IpcPublicAttributeNamedValues
{
    public IpcShapeAttributeNamedValues(INamedTypeSymbol? contractType, INamedTypeSymbol? ipcType)
        : base(ipcType)
    {
        ContractType = contractType;
    }

    public IpcShapeAttributeNamedValues(INamedTypeSymbol? contractType, INamedTypeSymbol? ipcType, ISymbol? member, ITypeSymbol? memberReturnType)
        : base(ipcType, member, memberReturnType)
    {
        ContractType = contractType;
    }

    public INamedTypeSymbol? ContractType { get; }

    public override string ToString()
    {
        return $@"new()
{{
    {Format(nameof(DefaultReturn), DefaultReturn, x => Format(x, MemberReturnType))}
    {Format(nameof(Timeout), Timeout)}
    {Format(nameof(IgnoresIpcException), IgnoresIpcException)}
    {Format(nameof(IsReadonly), IsReadonly)}
    {Format(nameof(WaitsVoid), WaitsVoid)}
}}";
    }
}
