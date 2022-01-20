using System.Collections.Immutable;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class IpcPublicMethodInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectJointMatchGenerator
{
    /// <summary>
    /// IPC 类型的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _ipcType;

    /// <summary>
    /// 此方法的语义符号。
    /// </summary>
    private readonly IMethodSymbol _method;

    /// <summary>
    /// 如果此成员是一个异步方法，则此值为 true；否则为 false。
    /// </summary>
    private readonly bool _isAsyncMethod;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 的接口类型）的语义符号。</param>
    /// <param name="method">此成员的语义符号。</param>
    public IpcPublicMethodInfo(INamedTypeSymbol ipcType, IMethodSymbol method)
    {
        _ipcType = ipcType ?? throw new ArgumentNullException(nameof(ipcType));
        _method = method ?? throw new ArgumentNullException(nameof(method));
        var returnType = method.ReturnType.OriginalDefinition.ToString();
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
            var namedValues = _method.GetIpcNamedValues(asyncReturnType, _ipcType);
            var sourceCode = asyncReturnType is null
                ? @$"System.Threading.Tasks.Task {_method.ContainingType.Name}.{_method.Name}({parameters})
        {{
            return CallMethodAsync(new Garm<object?>[] {{ {arguments} }}, {namedValues});
        }}"
                : @$"System.Threading.Tasks.Task<{asyncReturnType}> {_method.ContainingType.Name}.{_method.Name}({parameters})
        {{
            return CallMethodAsync<{asyncReturnType}>(new Garm<object?>[] {{ {arguments} }}, {namedValues});
        }}";
            return sourceCode;
        }
        else if (_method.ReturnsVoid)
        {
            // 同步 void 方法。
            var parameters = GenerateMethodParameters(_method.Parameters);
            var arguments = GenerateMethodArguments(_method.Parameters);
            var namedValues = _method.GetIpcNamedValues(null, _ipcType);
            var sourceCode = namedValues.WaitsVoid
                ? @$"void {_method.ContainingType.Name}.{_method.Name}({parameters})
        {{
            CallMethod(new Garm<object?>[] {{ {arguments} }}, {namedValues}).Wait();
        }}"
                : @$"void {_method.ContainingType.Name}.{_method.Name}({parameters})
        {{
            _ = CallMethod(new Garm<object?>[] {{ {arguments} }}, {namedValues});
        }}";
            return sourceCode;
        }
        else
        {
            // 同步带返回值方法。
            var parameters = GenerateMethodParameters(_method.Parameters);
            var arguments = GenerateMethodArguments(_method.Parameters);
            var @return = _method.ReturnType;
            var namedValues = _method.GetIpcNamedValues(@return, _ipcType);
            var sourceCode = @$"{_method.ReturnType} {_method.ContainingType.Name}.{_method.Name}({parameters})
        {{
            return CallMethod<{@return}>(new Garm<object?>[] {{ {arguments} }}, {namedValues}).Result;
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
        var arguments = GenerateMethodArguments(_method.Parameters);
        var asyncReturn = GetAsyncReturnType(_method.ReturnType);
        if (_method.ReturnsVoid)
        {
            // void 同步方法。
            var call = $"{real}.{_method.Name}({arguments})";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Action(() => {call}));"
                : $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Action<{GenerateMethodParameterTypes(_method.Parameters)}>(({arguments}) => {call}));";
            return sourceCode;
        }
        else if (!_isAsyncMethod)
        {
            // T 同步方法。
            var @return = $"Garm<{_method.ReturnType}>";
            var call = $"new Garm<{_method.ReturnType}>({real}.{_method.Name}({arguments}))";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Func<{@return}>(() => {call}));"
                : $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Func<{GenerateMethodParameterTypes(_method.Parameters)}, {@return}>(({arguments}) => {call}));";
            return sourceCode;
        }
        else if (asyncReturn is null)
        {
            // Task 异步方法。
            var call = $"{real}.{_method.Name}({arguments})";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Func<Task>(() => {call}));"
                : $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Func<{GenerateMethodParameterTypes(_method.Parameters)}, Task>(({arguments}) => {call}));";
            return sourceCode;
        }
        else
        {
            // Task<T> 异步方法。
            var @return = $"Task<Garm<{asyncReturn}>>";
            var call = $"new Garm<{asyncReturn}>(await {real}.{_method.Name}({arguments}).ConfigureAwait(false))";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Func<{@return}>(async () => {call}));"
                : $"MatchMethod(nameof({_ipcType}.{_method.Name}), new System.Func<{GenerateMethodParameterTypes(_method.Parameters)}, {@return}>(async ({arguments}) => {call}));";
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
