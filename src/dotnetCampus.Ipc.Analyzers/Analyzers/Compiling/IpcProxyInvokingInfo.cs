namespace dotnetCampus.Ipc.Analyzers.Compiling;

/// <summary>
/// 用语法节点和语义模型描述一次方法调用。
/// </summary>
[DebuggerDisplay("{MemberAccessNode.GetLocation()?.SourceSpan.Start,nq}: {MemberAccessNode.ToFullString(),nq}")]
internal struct IpcProxyInvokingInfo
{
    private IpcProxyInvokingInfo(SemanticModel semanticModel, MemberAccessExpressionSyntax memberAccessNode,
        IMethodSymbol methodSymbol, INamedTypeSymbol contractType)
    {
        SemanticModel = semanticModel;
        MemberAccessNode = memberAccessNode ?? throw new ArgumentNullException(nameof(memberAccessNode));
        MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        ShapeType = null;
    }

    private IpcProxyInvokingInfo(SemanticModel semanticModel, MemberAccessExpressionSyntax memberAccessNode,
        IMethodSymbol methodSymbol, INamedTypeSymbol contractType, INamedTypeSymbol shapeType)
    {
        SemanticModel = semanticModel;
        MemberAccessNode = memberAccessNode ?? throw new ArgumentNullException(nameof(memberAccessNode));
        MethodSymbol = methodSymbol ?? throw new ArgumentNullException(nameof(methodSymbol));
        ContractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        ShapeType = shapeType ?? throw new ArgumentNullException(nameof(shapeType));
    }

    /// <summary>
    /// 能用来解读本语法节点的语义模型。
    /// </summary>
    public SemanticModel SemanticModel { get; }

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

    /// <summary>
    /// 如果本次调用使用到了 IPC 形状代理，那么此属性是 IPC 形状代理的类型。
    /// </summary>
    public INamedTypeSymbol? ShapeType { get; }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hashCode = -25060616;
        hashCode = hashCode * -1521134295 + EqualityComparer<SemanticModel>.Default.GetHashCode(SemanticModel);
        hashCode = hashCode * -1521134295 + EqualityComparer<MemberAccessExpressionSyntax>.Default.GetHashCode(MemberAccessNode);
        hashCode = hashCode * -1521134295 + EqualityComparer<IMethodSymbol>.Default.GetHashCode(MethodSymbol);
        return hashCode;
    }

    public static IpcProxyInvokingInfo? TryCreateIpcProxyInvokingInfo(SemanticModel semanticModel, MemberAccessExpressionSyntax memberAccessNode, CancellationToken cancellationToken)
    {
        var methodName = memberAccessNode.Name.Identifier.ValueText;
        if (methodName != "CreateIpcProxy")
        {
            // 初筛。
            return null;
        }

        if (semanticModel.GetSymbolInfo(memberAccessNode, cancellationToken).Symbol is IMethodSymbol methodSymbol)
        {
            // 当前正在调用的方法是一个正常的方法。
            if (memberAccessNode.Name is GenericNameSyntax genericNameNode)
            {
                if (genericNameNode.TypeArgumentList.Arguments.Count == 1)
                {
                    if (genericNameNode.TypeArgumentList.Arguments[0] is TypeSyntax typeArgumentNode
                        && semanticModel.GetSymbolInfo(typeArgumentNode, cancellationToken).Symbol is INamedTypeSymbol typeArgumentSymbol)
                    {
                        return new(semanticModel, memberAccessNode, methodSymbol, typeArgumentSymbol);
                    }
                }
                else if (genericNameNode.TypeArgumentList.Arguments.Count == 2)
                {
                    if (genericNameNode.TypeArgumentList.Arguments[0] is TypeSyntax contractTypeArgumentNode
                        && semanticModel.GetSymbolInfo(contractTypeArgumentNode, cancellationToken).Symbol is INamedTypeSymbol contractTypeArgumentSymbol
                        && genericNameNode.TypeArgumentList.Arguments[1] is TypeSyntax shapeTypeArgumentNode
                        && semanticModel.GetSymbolInfo(shapeTypeArgumentNode, cancellationToken).Symbol is INamedTypeSymbol shapeTypeArgumentSymbol)
                    {
                        return new(semanticModel, memberAccessNode, methodSymbol, contractTypeArgumentSymbol, shapeTypeArgumentSymbol);
                    }
                }
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
