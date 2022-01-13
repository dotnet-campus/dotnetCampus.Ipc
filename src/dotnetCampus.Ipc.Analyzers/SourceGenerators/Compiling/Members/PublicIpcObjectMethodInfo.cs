using System.Collections.Immutable;

using dotnetCampus.Ipc.SourceGenerators.Utils;

using Microsoft.CodeAnalysis;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class PublicIpcObjectMethodInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectJointMatchGenerator
{
    /// <summary>
    /// 契约接口的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _contractType;

    /// <summary>
    /// 真实类型的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _realType;

    /// <summary>
    /// 此成员在类型实现中的语义符号。
    /// </summary>
    private readonly IMethodSymbol _method;

    /// <summary>
    /// 如果此成员是一个异步方法，则此值为 true；否则为 false。
    /// </summary>
    private readonly bool _isAsyncMethod;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="contractType">契约接口的语义符号。</param>
    /// <param name="realType">真实类型的语义符号。</param>
    /// <param name="interfaceMember">此成员在接口定义中的语义符号。</param>
    /// <param name="implementationMember">此成员在类型实现中的语义符号。</param>
    public PublicIpcObjectMethodInfo(INamedTypeSymbol contractType, INamedTypeSymbol realType, IMethodSymbol interfaceMember, IMethodSymbol implementationMember)
    {
        _contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        _realType = realType ?? throw new ArgumentNullException(nameof(realType));
        _method = implementationMember ?? throw new ArgumentNullException(nameof(implementationMember));
        var returnType = interfaceMember.ReturnType.OriginalDefinition.ToString();
        _isAsyncMethod = returnType is "System.Threading.Tasks.Task" or "System.Threading.Tasks.Task<TResult>";
    }

    /// <summary>
    /// 生成此方法在 IPC 代理中的源代码。
    /// </summary>
    /// <returns>方法源代码。</returns>
    public string GenerateProxyMember()
    {
        if (_isAsyncMethod)
        {
            // 异步方法。
            var parameters = GenerateMethodParameters(_method.Parameters);
            var arguments = GenerateMethodArguments(_method.Parameters);
            var asyncReturnType = GetAsyncReturnType(_method.ReturnType);
            var attributes = _method.GetIpcAttributesAsAnInvokingArg(asyncReturnType, _realType);
            var sourceCode = asyncReturnType is null
                ? @$"        public System.Threading.Tasks.Task {_method.Name}({parameters})
        {{
            return CallMethodAsync(new object[] {{ {arguments} }}, {attributes});
        }}"
                : @$"        public System.Threading.Tasks.Task<{asyncReturnType}> {_method.Name}({parameters})
        {{
            return CallMethodAsync<{asyncReturnType}>(new object[] {{ {arguments} }}, {attributes});
        }}";
            return sourceCode;
        }
        else if (_method.ReturnsVoid)
        {
            // 同步 void 方法。
            var waitVoid = _method.CheckIpcWaitingVoid();
            var parameters = GenerateMethodParameters(_method.Parameters);
            var arguments = GenerateMethodArguments(_method.Parameters);
            var attributes = _method.GetIpcAttributesAsAnInvokingArg(null, _realType);
            var sourceCode = waitVoid
                ? @$"        public void {_method.Name}({parameters})
        {{
            CallMethod(new object[] {{ {arguments} }}, {attributes}).Wait();
        }}"
                : @$"        public void {_method.Name}({parameters})
        {{
            _ = CallMethod(new object[] {{ {arguments} }}, {attributes});
        }}";
            return sourceCode;
        }
        else
        {
            // 同步带返回值方法。
            var parameters = GenerateMethodParameters(_method.Parameters);
            var arguments = GenerateMethodArguments(_method.Parameters);
            var @return = _method.ReturnType;
            var attributes = _method.GetIpcAttributesAsAnInvokingArg(@return, _realType);
            var sourceCode = @$"        public {_method.ReturnType} {_method.Name}({parameters})
        {{
            return CallMethod<{@return}>(new object[] {{ {arguments} }}, {attributes}).Result;
        }}";
            return sourceCode;
        }
    }

    /// <summary>
    /// 生成此方法在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="real">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>方法源代码。</returns>
    public string GenerateJointMatch(string real)
    {
        if (_method.ReturnsVoid)
        {
            var arguments = GenerateMethodArguments(_method.Parameters);
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_contractType}.{_method.Name}), new System.Action(() => {real}.{_method.Name}()));"
                : $"MatchMethod(nameof({_contractType}.{_method.Name}), new System.Action<{GenerateMethodParameterTypes(_method.Parameters)}>(({arguments}) => {real}.{_method.Name}({arguments})));";
            return sourceCode;
        }
        else
        {
            var arguments = GenerateMethodArguments(_method.Parameters);
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_contractType}.{_method.Name}), new System.Func<{_method.ReturnType}>(() => {real}.{_method.Name}()));"
                : $"MatchMethod(nameof({_contractType}.{_method.Name}), new System.Func<{GenerateMethodParameterTypes(_method.Parameters)}, {_method.ReturnType}>(({arguments}) => {real}.{_method.Name}({arguments})));";
            return sourceCode;
        }
    }

    /// <summary>
    /// 根据参数列表生成方法形参列表字符串。
    /// </summary>
    /// <param name="parameters">方法参数列表。</param>
    /// <returns>方法形参列表字符串。</returns>
    private string GenerateMethodParameters(ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(", ", parameters.Select(x => $"{x.Type} {x.Name}"));
    }

    /// <summary>
    /// 根据参数列表生成方法参数类型列表字符串。
    /// </summary>
    /// <param name="parameters">方法参数列表。</param>
    /// <returns>方法参数类型列表字符串。</returns>
    private string GenerateMethodParameterTypes(ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(", ", parameters.Select(x => $"{x.Type}"));
    }

    /// <summary>
    /// 根据参数列表生成方法实参列表字符串。
    /// </summary>
    /// <param name="parameters">方法参数列表。</param>
    /// <returns>方法实参列表字符串。</returns>
    private string GenerateMethodArguments(ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(", ", parameters.Select(x => $"{x.Name}"));
    }

    /// <summary>
    /// 如果方法是异步方法，则返回泛型 Task 的内部类型或非泛型 Task 的 null；如果是同步方法，则返回原类型。
    /// </summary>
    /// <param name="returnType">此方法返回类型的语义符号。</param>
    /// <returns>返回类型的源代码。</returns>
    private ITypeSymbol? GetAsyncReturnType(ITypeSymbol returnType)
    {
        if (returnType is INamedTypeSymbol namedReturnType)
        {
            if (_isAsyncMethod)
            {
                if (namedReturnType.TypeArguments.FirstOrDefault() is { } returnArgument)
                {
                    // Task<TResult>
                    return returnArgument;
                }
                else
                {
                    // Task
                    return null;
                }
            }
            else
            {
                return returnType;
            }
        }
        else
        {
            return returnType;
        }
    }
}
