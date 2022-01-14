namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class PublicIpcObjectPropertyInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectJointMatchGenerator
{
    /// <summary>
    /// 此成员在类型实现中的语义符号。
    /// </summary>
    private readonly IPropertySymbol _property;

    /// <summary>
    /// 契约接口的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _contractType;

    /// <summary>
    /// 真实类型的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _realType;

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
        _property = implementationMember ?? throw new ArgumentNullException(nameof(implementationMember));
    }

    /// <summary>
    /// 生成此属性在 IPC 代理中的源代码。
    /// </summary>
    /// <returns>属性源代码。</returns>
    public string GenerateProxyMember()
    {
        var namedValues = _property.GetIpcNamedValues(_realType);
        if (_property.GetMethod is { } getMethod && _property.SetMethod is { } setMethod)
        {
            var sourceCode = $@"        public {_property.Type} {_property.Name}
        {{
            get => GetValueAsync<{_property.Type}>({namedValues}).Result;
            set => SetValueAsync(value, {namedValues}).Wait();
        }}";
            return sourceCode;
        }
        else if (_property.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"        public {_property.Type} {_property.Name} => GetValueAsync<{_property.Type}>({namedValues}).Result;";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC004_OnlyGetOrGetSetPropertyIsSupported,
                _property.Locations.FirstOrDefault(),
                _property.Name);
        }
    }

    /// <summary>
    /// 生成此属性在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="real">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>属性源代码。</returns>
    public string GenerateJointMatch(string real)
    {
        if (_property.GetMethod is { } getMethod && _property.SetMethod is { } setMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_contractType}.{_property.Name}), new System.Func<{_property.Type}>(() => {real}.{_property.Name}), new System.Action<{_property.Type}>(value => {real}.{_property.Name} = value));";
            return sourceCode;
        }
        else if (_property.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_contractType}.{_property.Name}), new System.Func<{_property.Type}>(() => {real}.{_property.Name}));";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC004_OnlyGetOrGetSetPropertyIsSupported,
                _property.Locations.FirstOrDefault(),
                _property.Name);
        }
    }
}
