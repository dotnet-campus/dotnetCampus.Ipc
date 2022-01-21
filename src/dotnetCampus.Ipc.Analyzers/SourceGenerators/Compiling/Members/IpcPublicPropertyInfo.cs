using dotnetCampus.Ipc.SourceGenerators.Models;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class IpcPublicPropertyInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectShapeMemberGenerator, IPublicIpcObjectJointMatchGenerator
{
    /// <summary>
    /// IPC 类型的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _ipcType;

    /// <summary>
    /// 此属性的语义符号。
    /// </summary>
    private readonly IPropertySymbol _property;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 的接口类型）的语义符号。</param>
    /// <param name="property">此属性的语义符号。</param>
    public IpcPublicPropertyInfo(INamedTypeSymbol ipcType, IPropertySymbol property)
    {
        _ipcType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        _property = property ?? throw new ArgumentNullException(nameof(property));
    }

    /// <summary>
    /// 生成此属性在 IPC 代理中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>属性源代码。</returns>
    public MemberDeclarationSourceTextBuilder GenerateProxyMember(SourceTextBuilder builder)
    {
        var propertyTypeName = builder.SimplifyNameByAddUsing(_property.Type);
        var containingTypeName = builder.SimplifyNameByAddUsing(_property.ContainingType);
        var namedValues = _property.GetIpcNamedValues(_ipcType);
        var (hasGet, hasSet) = (_property.GetMethod is not null, _property.SetMethod is not null);
        return new(
            builder,
            (hasGet, hasSet) switch
            {
                // get/set 属性。
                (true, true) => $@"

{propertyTypeName} {containingTypeName}.{_property.Name}
{{
    get => GetValueAsync<{propertyTypeName}>({namedValues}).Result;
    set => SetValueAsync<{propertyTypeName}>(value, {namedValues}).Wait();
}}

                ",
                // get 属性。
                (true, false) => $@"

{propertyTypeName} {containingTypeName}.{_property.Name} => GetValueAsync<{propertyTypeName}>({namedValues}).Result;

                ",
                // 不支持 set 属性。
                _ => throw new DiagnosticException(IPC002_KnownDiagnosticError),
            }
);
    }

    /// <summary>
    /// 生成此成员在 IPC 代理壳中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>成员源代码。</returns>
    public MemberDeclarationSourceTextBuilder GenerateShapeMember(SourceTextBuilder builder)
    {
        var propertyTypeName = builder.SimplifyNameByAddUsing(_property.Type);
        var containingTypeName = builder.SimplifyNameByAddUsing(_property.ContainingType);
        var (hasGet, hasSet) = (_property.GetMethod is not null, _property.SetMethod is not null);
        return new(
            builder,
            (hasGet, hasSet) switch
            {
                // get/set 属性。
                (true, true) => $@"

[IpcProperty]
{propertyTypeName} {containingTypeName}.{_property.Name} {{ get; set; }}

                ",
                // get 属性。
                (true, false) => $@"

[IpcProperty]
{propertyTypeName} {containingTypeName}.{_property.Name} {{ get; }}

                ",
                // 不支持 set 属性。
                _ => throw new DiagnosticException(IPC002_KnownDiagnosticError),
            }
);
    }

    /// <summary>
    /// 生成此属性在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="real">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>属性源代码。</returns>
    public string GenerateJointMatch(SourceTextBuilder builder, string real)
    {
        var containingTypeName = builder.SimplifyNameByAddUsing(_property.ContainingType);
        var propertyTypeName = builder.SimplifyNameByAddUsing(_property.Type);
        var garmPropertyTypeName = $"Garm<{propertyTypeName}>";
        var (hasGet, hasSet) = (_property.GetMethod is not null, _property.SetMethod is not null);
        if (hasGet && hasSet)
        {
            var sourceCode = $"MatchProperty(nameof({containingTypeName}.{_property.Name}), new Func<{garmPropertyTypeName}>(() => {real}.{_property.Name}), new Action<{propertyTypeName}>(value => {real}.{_property.Name} = value));";
            return sourceCode;
        }
        else if (hasGet)
        {
            var sourceCode = $"MatchProperty(nameof({containingTypeName}.{_property.Name}), new Func<{garmPropertyTypeName}>(() => {real}.{_property.Name}));";
            return sourceCode;
        }
        else
        {
            // 不支持只写属性。
            throw new DiagnosticException(IPC002_KnownDiagnosticError);
        }
    }
}
