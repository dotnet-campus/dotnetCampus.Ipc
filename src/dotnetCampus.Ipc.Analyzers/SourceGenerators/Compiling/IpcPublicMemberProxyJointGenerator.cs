using dotnetCampus.Ipc.SourceGenerators.Models;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling;

/// <summary>
/// 辅助生成契约接口中每一个成员对应的 IPC 代理和对接。
/// </summary>
internal class IpcPublicMemberProxyJointGenerator
{
    private readonly IPublicIpcObjectProxyMemberGenerator _proxyMemberGenerator;
    private readonly IPublicIpcObjectShapeMemberGenerator _shapeMemberGenerator;
    private readonly IPublicIpcObjectJointMatchGenerator _jointMatchGenerator;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="ipcType">契约接口类型的语义符号。</param>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）的语义符号。（可能与 <paramref name="ipcType"/> 是相同类型。）</param>
    /// <param name="member"><paramref name="ipcType"/> 中成员的语义符号。</param>
    public IpcPublicMemberProxyJointGenerator(INamedTypeSymbol ipcType, ISymbol member)
    {
        ipcType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        member = member ?? throw new ArgumentNullException(nameof(member));

        _proxyMemberGenerator = member switch
        {
            IMethodSymbol methodSymbol => new IpcPublicMethodInfo(ipcType, ipcType, methodSymbol, methodSymbol),
            IPropertySymbol propertySymbol => new IpcPublicPropertyInfo(ipcType, ipcType, propertySymbol, propertySymbol),
            _ => throw new DiagnosticException(
                IPC200_IpcMembers_OnlyPropertiesMethodsAndEventsAreSupported,
                member.Locations.FirstOrDefault(),
                member.Name),
        };
        _shapeMemberGenerator = (IPublicIpcObjectShapeMemberGenerator) _proxyMemberGenerator;
        _jointMatchGenerator = (IPublicIpcObjectJointMatchGenerator) _proxyMemberGenerator;
    }

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="contractType">契约接口类型的语义符号。</param>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）的语义符号。（可能与 <paramref name="contractType"/> 是相同类型。）</param>
    /// <param name="contractMember">此成员原始定义（即在 <paramref name="contractType"/> 中所定义的方法）的语义符号。</param>
    /// <param name="ipcMember">标记了 <see cref="IpcMemberAttribute"/> 的此成员实现的语义符号。（可能与 <paramref name="contractMember"/> 是相同实例。）</param>
    public IpcPublicMemberProxyJointGenerator(INamedTypeSymbol contractType, INamedTypeSymbol ipcType, ISymbol contractMember, ISymbol ipcMember)
    {
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        contractMember = contractMember ?? throw new ArgumentNullException(nameof(contractMember));

        _proxyMemberGenerator = contractMember switch
        {
            IMethodSymbol methodSymbol => new IpcPublicMethodInfo(contractType, ipcType, methodSymbol, (IMethodSymbol) ipcMember),
            IPropertySymbol propertySymbol => new IpcPublicPropertyInfo(contractType, ipcType, propertySymbol, (IPropertySymbol) ipcMember),
            _ => throw new DiagnosticException(
                IPC200_IpcMembers_OnlyPropertiesMethodsAndEventsAreSupported,
                contractMember.Locations.FirstOrDefault(),
                contractMember.Name),
        };
        _shapeMemberGenerator = (IPublicIpcObjectShapeMemberGenerator) _proxyMemberGenerator;
        _jointMatchGenerator = (IPublicIpcObjectJointMatchGenerator) _proxyMemberGenerator;
    }

    /// <summary>
    /// 生成此成员在 IPC 代理中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>成员源代码。</returns>
    public MemberDeclarationSourceTextBuilder GenerateProxyMember(SourceTextBuilder builder) => _proxyMemberGenerator.GenerateProxyMember(builder);

    /// <summary>
    /// 生成此成员在 IPC 代理壳中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>成员源代码。</returns>
    internal MemberDeclarationSourceTextBuilder GenerateShapeMember(SourceTextBuilder builder) => _shapeMemberGenerator.GenerateShapeMember(builder);

    /// <summary>
    /// 生成此成员在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="realInstanceVariableName">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>成员源代码。</returns>
    public string GenerateJointMatch(SourceTextBuilder builder, string realInstanceVariableName) => _jointMatchGenerator.GenerateJointMatch(builder, realInstanceVariableName);
}
