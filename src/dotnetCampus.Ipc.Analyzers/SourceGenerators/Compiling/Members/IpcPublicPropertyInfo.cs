using dotnetCampus.Ipc.SourceGenerators.Models;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class IpcPublicPropertyInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectShapeMemberGenerator, IPublicIpcObjectJointMatchGenerator
{
    /// <summary>
    /// 契约接口类型的语义符号。
    /// </summary>
    /// <remarks>
    /// 如果需要获取接口定义相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly INamedTypeSymbol _contractType;

    /// <summary>
    /// IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）的语义符号。（可能与 <see cref="_ipcType"/> 是相同类型。）
    /// </summary>
    /// <remarks>
    /// 如果需要获取接口特性（<see cref="Attribute"/>）相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly INamedTypeSymbol _ipcType;

    /// <summary>
    /// 此属性原始定义（即在 <see cref="_contractType"/> 中所定义的方法）的语义符号。
    /// </summary>
    /// <remarks>
    /// 如果需要获取属性签名相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly IPropertySymbol _contractProperty;

    /// <summary>
    /// 标记了 <see cref="IpcMemberAttribute"/> 的此属性实现的语义符号。（可能与 <see cref="_contractProperty"/> 是相同实例。）
    /// </summary>
    /// <remarks>
    /// 如果需要获取属性特性（<see cref="Attribute"/>）相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly IPropertySymbol _ipcProperty;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="contractType">契约接口类型的语义符号。</param>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）的语义符号。（可能与 <paramref name="contractType"/> 是相同类型。）</param>
    /// <param name="contractProperty">此成员原始定义（即在 <paramref name="contractType"/> 中所定义的方法）的语义符号。</param>
    /// <param name="ipcProperty">标记了 <see cref="IpcMemberAttribute"/> 的此成员实现的语义符号。（可能与 <paramref name="contractProperty"/> 是相同实例。）</param>
    public IpcPublicPropertyInfo(INamedTypeSymbol contractType, INamedTypeSymbol ipcType, IPropertySymbol contractProperty, IPropertySymbol ipcProperty)
    {
        _contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        _ipcType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        _contractProperty = contractProperty ?? throw new ArgumentNullException(nameof(contractProperty));
        _ipcProperty = ipcProperty ?? throw new ArgumentNullException(nameof(ipcProperty));
    }

    /// <summary>
    /// 生成此属性在 IPC 代理中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>属性源代码。</returns>
    public MemberDeclarationSourceTextBuilder GenerateProxyMember(SourceTextBuilder builder)
    {
        var propertyTypeName = builder.SimplifyNameByAddUsing(_contractProperty.Type);
        var containingTypeName = builder.SimplifyNameByAddUsing(_contractProperty.ContainingType);
        var getMemberId = MemberIdGenerator.GeneratePropertyId("get", _ipcProperty.Name);
        var setMemberId = MemberIdGenerator.GeneratePropertyId("set", _ipcProperty.Name);
        var valueArgumentName = GenerateGarmArgument(builder, _contractProperty.Type, "value");
        var namedValues = _ipcProperty.GetIpcNamedValues(_ipcType);
        var (hasGet, hasSet) = (_contractProperty.GetMethod is not null, _contractProperty.SetMethod is not null);
        return new(
            builder,
            (hasGet, hasSet) switch
            {
                // get/set 属性。
                (true, true) => $@"

{propertyTypeName} {containingTypeName}.{_contractProperty.Name}
{{
    get => GetValueAsync<{propertyTypeName}>({getMemberId}, {namedValues}).Result;
    set => SetValueAsync<{propertyTypeName}>({setMemberId}, {valueArgumentName}, {namedValues}).Wait();
}}

                ",
                // get 属性。
                (true, false) => $@"

{propertyTypeName} {containingTypeName}.{_contractProperty.Name} => GetValueAsync<{propertyTypeName}>({getMemberId}, {namedValues}).Result;

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
        var propertyTypeName = builder.SimplifyNameByAddUsing(_contractProperty.Type);
        var containingTypeName = builder.SimplifyNameByAddUsing(_contractProperty.ContainingType);
        var (hasGet, hasSet) = (_contractProperty.GetMethod is not null, _contractProperty.SetMethod is not null);
        return new(
            builder,
            (hasGet, hasSet) switch
            {
                // get/set 属性。
                (true, true) => $@"

[IpcProperty]
{propertyTypeName} {containingTypeName}.{_contractProperty.Name} {{ get; set; }}

                ",
                // get 属性。
                (true, false) => $@"

[IpcProperty]
{propertyTypeName} {containingTypeName}.{_contractProperty.Name} {{ get; }}

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
        var containingTypeName = builder.SimplifyNameByAddUsing(_contractProperty.ContainingType);
        var propertyTypeName = builder.SimplifyNameByAddUsing(_contractProperty.Type);
        var getMemberId = MemberIdGenerator.GeneratePropertyId("get", _ipcProperty.Name);
        var setMemberId = MemberIdGenerator.GeneratePropertyId("set", _ipcProperty.Name);
        var garmPropertyTypeName = $"Garm<{propertyTypeName}>";
        var garmPropertyArgumentName = GenerateGarmArgument(builder, _contractProperty.Type, $"{real}.{_contractProperty.Name}");
        var (hasGet, hasSet) = (_contractProperty.GetMethod is not null, _contractProperty.SetMethod is not null);
        if (hasGet && hasSet)
        {
            var sourceCode = $"MatchProperty({getMemberId}, {setMemberId}, new Func<{garmPropertyTypeName}>(() => {garmPropertyArgumentName}), new Action<{propertyTypeName}>(value => {real}.{_contractProperty.Name} = value));";
            return sourceCode;
        }
        else if (hasGet)
        {
            var sourceCode = $"MatchProperty({getMemberId}, new Func<{garmPropertyTypeName}>(() => {garmPropertyArgumentName}));";
            return sourceCode;
        }
        else
        {
            // 不支持只写属性。
            throw new DiagnosticException(IPC002_KnownDiagnosticError);
        }
    }

    /// <summary>
    /// 使用原参数生成 Garm 类型的参数，以支持 IPC 对象的跨进程传输。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameterType"></param>
    /// <param name="argumentName"></param>
    /// <returns></returns>
    private string GenerateGarmArgument(SourceTextBuilder builder, ITypeSymbol parameterType, string argumentName)
    {
        return parameterType.GetIsIpcType()
                ? $"new Garm<{builder.SimplifyNameByAddUsing(parameterType)}>({argumentName}, typeof({parameterType.Name}))"
                : $"new Garm<{builder.SimplifyNameByAddUsing(parameterType)}>({argumentName})";
    }
}
