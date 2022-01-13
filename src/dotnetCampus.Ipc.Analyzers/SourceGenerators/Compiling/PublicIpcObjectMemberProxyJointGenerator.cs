using dotnetCampus.Ipc.Core;
using dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

using Microsoft.CodeAnalysis;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling;

/// <summary>
/// 辅助生成契约接口中每一个成员对应的 IPC 代理和对接。
/// </summary>
internal class PublicIpcObjectMemberProxyJointGenerator
{
    private readonly IPublicIpcObjectProxyMemberGenerator _proxyMemberGenerator;
    private readonly IPublicIpcObjectJointMatchGenerator _jointMatchGenerator;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="contractType">契约接口的语义符号。</param>
    /// <param name="realType">真实类型的语义符号。</param>
    /// <param name="interfaceMember">此成员在接口定义中的语义符号。</param>
    /// <param name="implementationMember">此成员在类型实现中的语义符号。</param>
    public PublicIpcObjectMemberProxyJointGenerator(INamedTypeSymbol contractType, INamedTypeSymbol realType, ISymbol interfaceMember, ISymbol implementationMember)
    {
        contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        realType = realType ?? throw new ArgumentNullException(nameof(realType));
        interfaceMember = interfaceMember ?? throw new ArgumentNullException(nameof(interfaceMember));
        implementationMember = implementationMember ?? throw new ArgumentNullException(nameof(implementationMember));

        _proxyMemberGenerator = interfaceMember switch
        {
            IMethodSymbol methodSymbol => new PublicIpcObjectMethodInfo(contractType, realType, methodSymbol, (IMethodSymbol) implementationMember),
            IPropertySymbol propertySymbol => new PublicIpcObjectPropertyInfo(contractType, realType, propertySymbol, (IPropertySymbol) implementationMember),
            _ => throw new DiagnosticException(
                DIPC003_OnlyMethodOrPropertyIsSupported,
                implementationMember.Locations.FirstOrDefault(),
                interfaceMember.Name),
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
