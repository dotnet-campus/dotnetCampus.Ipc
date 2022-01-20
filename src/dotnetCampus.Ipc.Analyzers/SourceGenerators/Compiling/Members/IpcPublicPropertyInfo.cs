namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class IpcPublicPropertyInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectJointMatchGenerator
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
    /// <returns>属性源代码。</returns>
    public string GenerateProxyMember()
    {
        var namedValues = _property.GetIpcNamedValues(_ipcType);
        if (_property.GetMethod is { } getMethod && _property.SetMethod is { } setMethod)
        {
            var sourceCode = $@"{_property.Type} {_property.ContainingType.Name}.{_property.Name}
        {{
            get => GetValueAsync<{_property.Type}>({namedValues}).Result;
            set => SetValueAsync<{_property.Type}>(value, {namedValues}).Wait();
        }}";
            return sourceCode;
        }
        else if (_property.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"{_property.Type} {_property.ContainingType.Name}.{_property.Name} => GetValueAsync<{_property.Type}>({namedValues}).Result;";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC022_SetOnlyPropertyIsNotSupportedForIpcObject,
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
        var propertyType = $"Garm<{_property.Type}>";
        if (_property.GetMethod is { } getMethod && _property.SetMethod is { } setMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_ipcType}.{_property.Name}), new System.Func<{propertyType}>(() => {real}.{_property.Name}), new System.Action<{_property.Type}>(value => {real}.{_property.Name} = value));";
            return sourceCode;
        }
        else if (_property.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_ipcType}.{_property.Name}), new System.Func<{propertyType}>(() => {real}.{_property.Name}));";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC022_SetOnlyPropertyIsNotSupportedForIpcObject,
                _property.Locations.FirstOrDefault(),
                _property.Name);
        }
    }
}
