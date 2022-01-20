namespace dotnetCampus.Ipc.SourceGenerators.Compiling;

/// <summary>
/// 辅助生成契约接口中每一个成员对应的 IPC 代理和对接。
/// </summary>
internal class IpcPublicMemberProxyJointGenerator
{
    private readonly IPublicIpcObjectProxyMemberGenerator _proxyMemberGenerator;
    private readonly IPublicIpcObjectJointMatchGenerator _jointMatchGenerator;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 的接口类型）的语义符号。</param>
    /// <param name="member">此成员的语义符号。</param>
    public IpcPublicMemberProxyJointGenerator(INamedTypeSymbol ipcType, ISymbol member)
    {
        ipcType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        member = member ?? throw new ArgumentNullException(nameof(member));

        _proxyMemberGenerator = member switch
        {
            IMethodSymbol methodSymbol => new IpcPublicMethodInfo(ipcType, methodSymbol),
            IPropertySymbol propertySymbol => new IpcPublicPropertyInfo(ipcType, propertySymbol),
            _ => throw new DiagnosticException(
                DIPC020_OnlyPropertiesAndMethodsAreSupportedForIpcObject,
                member.Locations.FirstOrDefault(),
                member.Name),
        };
        _jointMatchGenerator = (IPublicIpcObjectJointMatchGenerator) _proxyMemberGenerator;
    }

    /// <summary>
    /// 生成此成员在 IPC 代理中的源代码。
    /// </summary>
    /// <returns>成员源代码。</returns>
    public string GenerateProxyMember() => _proxyMemberGenerator.GenerateProxyMember();

    /// <summary>
    /// 生成此成员在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="realInstanceVariableName">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>成员源代码。</returns>
    public string GenerateJointMatch(string realInstanceVariableName) => _jointMatchGenerator.GenerateJointMatch(realInstanceVariableName);
}
