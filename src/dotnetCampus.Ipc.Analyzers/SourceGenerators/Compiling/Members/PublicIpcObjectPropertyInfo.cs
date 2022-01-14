namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class PublicIpcObjectPropertyInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectJointMatchGenerator
{
    /// <summary>
    /// 此成员在类型实现中的语义符号。
    /// </summary>
    private readonly IPropertySymbol _implementedProperty;

    /// <summary>
    /// 契约接口的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _contractType;

    /// <summary>
    /// 真实类型中的属性语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _realType;

    /// <summary>
    /// 接口中的属性语义符号。
    /// </summary>
    private readonly IPropertySymbol _interfaceProperty;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="contractType">契约接口的语义符号。</param>
    /// <param name="realType">真实类型的语义符号。</param>
    /// <param name="interfaceMember">此成员在接口定义中的语义符号。</param>
    /// <param name="implementationMember">此成员在类型实现中的语义符号。</param>
    public PublicIpcObjectPropertyInfo(INamedTypeSymbol contractType, INamedTypeSymbol realType, IPropertySymbol interfaceMember, IPropertySymbol implementationMember)
    {
        _contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        _realType = realType ?? throw new ArgumentNullException(nameof(realType));
        _interfaceProperty = interfaceMember ?? throw new ArgumentNullException(nameof(interfaceMember));
        _implementedProperty = implementationMember ?? throw new ArgumentNullException(nameof(implementationMember));
    }

    /// <summary>
    /// 生成此属性在 IPC 代理中的源代码。
    /// </summary>
    /// <returns>属性源代码。</returns>
    public string GenerateProxyMember()
    {
        var namedValues = _implementedProperty.GetIpcNamedValues(_realType);
        if (_interfaceProperty.GetMethod is { } getMethod && _interfaceProperty.SetMethod is { } setMethod)
        {
            var sourceCode = $@"{_interfaceProperty.Type} {_interfaceProperty.ContainingType.Name}.{_interfaceProperty.Name}
        {{
            get => GetValueAsync<{_interfaceProperty.Type}>({namedValues}).Result;
            set => SetValueAsync(value, {namedValues}).Wait();
        }}";
            return sourceCode;
        }
        else if (_interfaceProperty.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"{_interfaceProperty.Type} {_interfaceProperty.ContainingType.Name}.{_interfaceProperty.Name} => GetValueAsync<{_implementedProperty.Type}>({namedValues}).Result;";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC004_OnlyGetOrGetSetPropertyIsSupported,
                _implementedProperty.Locations.FirstOrDefault(),
                _implementedProperty.Name);
        }
    }

    /// <summary>
    /// 生成此属性在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="real">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>属性源代码。</returns>
    public string GenerateJointMatch(string real)
    {
        if (_interfaceProperty.GetMethod is { } getMethod && _interfaceProperty.SetMethod is { } setMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_contractType}.{_interfaceProperty.Name}), new System.Func<{_interfaceProperty.Type}>(() => {real}.{_interfaceProperty.Name}), new System.Action<{_implementedProperty.Type}>(value => {real}.{_implementedProperty.Name} = value));";
            return sourceCode;
        }
        else if (_interfaceProperty.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_contractType}.{_interfaceProperty.Name}), new System.Func<{_interfaceProperty.Type}>(() => {real}.{_interfaceProperty.Name}));";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC004_OnlyGetOrGetSetPropertyIsSupported,
                _implementedProperty.Locations.FirstOrDefault(),
                _implementedProperty.Name);
        }
    }
}
