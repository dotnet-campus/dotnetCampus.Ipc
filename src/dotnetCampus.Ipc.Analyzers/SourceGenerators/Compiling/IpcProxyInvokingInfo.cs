using System.Diagnostics;
using System.Threading;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling;

/// <summary>
/// 用语法节点和语义模型描述一次方法调用。
/// </summary>
[DebuggerDisplay("{MemberAccessNode.GetLocation()?.SourceSpan.Start,nq}: {MemberAccessNode.ToFullString(),nq}")]
internal struct IpcProxyInvokingInfo
{
    /// <summary>
    /// 能用来解读本语法节点的语义模型。
    /// </summary>
    private readonly SemanticModel _semanticModel;

    private IpcProxyInvokingInfo(SemanticModel semanticModel, MemberAccessExpressionSyntax memberAccessNode,
        IMethodSymbol methodSymbol, INamedTypeSymbol contractType)
    {
        _semanticModel = semanticModel;
        MemberAccessNode = memberAccessNode ?? throw new ArgumentNullException(nameof(memberAccessNode));
        MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
    }

    /// <summary>
    /// 本次调用这个方法时，调用的这一句语法节点。
    /// </summary>
    public MemberAccessExpressionSyntax MemberAccessNode { get; }

    /// <summary>
    /// 本次所调用的方法的语义符号。
    /// </summary>
    public IMethodSymbol MethodSymbol { get; }

    /// <summary>
    /// 本次调用的契约类型。
    /// </summary>
    public INamedTypeSymbol ContractType { get; }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = -25060616;
        hashCode = hashCode * -1521134295 + EqualityComparer<SemanticModel>.Default.GetHashCode(_semanticModel);
        hashCode = hashCode * -1521134295 + EqualityComparer<MemberAccessExpressionSyntax>.Default.GetHashCode(MemberAccessNode);
        hashCode = hashCode * -1521134295 + EqualityComparer<IMethodSymbol>.Default.GetHashCode(MethodSymbol);
        return hashCode;
    }

    public static IpcProxyInvokingInfo? TryCreateIpcProxyInvokingInfo(SemanticModel semanticModel, MemberAccessExpressionSyntax memberAccessNode, CancellationToken cancellationToken)
    {
        if (semanticModel.GetSymbolInfo(memberAccessNode, cancellationToken).Symbol is IMethodSymbol methodSymbol)
        {
            // 当前正在调用的方法是一个正常的方法。
            if (memberAccessNode.Name is GenericNameSyntax genericNameNode
                && genericNameNode.TypeArgumentList.Arguments.Count == 1
                && genericNameNode.TypeArgumentList.Arguments[0] is TypeSyntax typeArgumentNode
                && semanticModel.GetSymbolInfo(typeArgumentNode, cancellationToken).Symbol is INamedTypeSymbol typeArgumentSymbol)
            {
                return new(semanticModel, memberAccessNode, methodSymbol, typeArgumentSymbol);
            }
            else
            {
                // 无法解析。
            }
        }
        else
        {
            // 当前正在调用的方法无法找到对应的符号（编译不通过）。
        }
        return null;
    }

    public static IEqualityComparer<IpcProxyInvokingInfo> ContractTypeEqualityComparer { get; } = new EqualityComparerByContractType();

    private sealed class EqualityComparerByContractType : IEqualityComparer<IpcProxyInvokingInfo>
    {
        public bool Equals(IpcProxyInvokingInfo x, IpcProxyInvokingInfo y)
        {
            return false;
        }

        public int GetHashCode(IpcProxyInvokingInfo info)
        {
            return info.GetHashCode();
        }
    }
}
