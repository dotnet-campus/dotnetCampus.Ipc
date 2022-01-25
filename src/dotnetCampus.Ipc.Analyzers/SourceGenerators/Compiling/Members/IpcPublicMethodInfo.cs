using dotnetCampus.Ipc.SourceGenerators.Models;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class IpcPublicMethodInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectShapeMemberGenerator, IPublicIpcObjectJointMatchGenerator
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
    /// <param name="builder"></param>
    /// <returns>方法源代码。</returns>
    public MemberDeclarationSourceTextBuilder GenerateProxyMember(SourceTextBuilder builder)
    {
        var parameters = GenerateMethodParameters(builder, _method.Parameters);
        var arguments = GenerateMethodArguments(_method.Parameters);
        var asyncReturnType = GetAsyncReturnType(_method.ReturnType);
        var returnTypeName = asyncReturnType is null ? "void" : builder.SimplifyNameByAddUsing(asyncReturnType);
        var methodContainingTypeName = builder.SimplifyNameByAddUsing(_method.ContainingType);
        var isAsync = _isAsyncMethod;
        var returnsVoid = _method.ReturnsVoid || asyncReturnType is null;
        var namedValues = _method.GetIpcNamedValues(asyncReturnType, _ipcType);

        return new(
            builder.AddUsing("System.Threading.Tasks"),
            (isAsync, returnsVoid) switch
            {
                // 异步 Task 方法。
                (true, true) => $@"

Task {methodContainingTypeName}.{_method.Name}({parameters})
{{
    return CallMethodAsync(new Garm<object?>[] {{ {arguments} }}, {namedValues});
}}

                ",
                // 异步 Task<T> 方法。
                (true, _) => $@"

Task<{returnTypeName}> {methodContainingTypeName}.{_method.Name}({parameters})
{{
    return CallMethodAsync<{returnTypeName}>(new Garm<object?>[] {{ {arguments} }}, {namedValues});
}}

                ",
                // 同步 void 方法。
                (false, true) => namedValues.WaitsVoid
                    ? @$"
void {methodContainingTypeName}.{_method.Name}({parameters})
{{
    CallMethod(new Garm<object?>[] {{ {arguments} }}, {namedValues}).Wait();
}}"
                    : @$"
void {methodContainingTypeName}.{_method.Name}({parameters})
{{
    _ = CallMethod(new Garm<object?>[] {{ {arguments} }}, {namedValues});
}}",

                // 同步 T 方法。
                (false, _) => $@"
{returnTypeName} {methodContainingTypeName}.{_method.Name}({parameters})
{{
    return CallMethod<{returnTypeName}>(new Garm<object?>[] {{ {arguments} }}, {namedValues}).Result;
}}
                ",
            }
        );
    }

    /// <summary>
    /// 生成此成员在 IPC 代理壳中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>成员源代码。</returns>
    public MemberDeclarationSourceTextBuilder GenerateShapeMember(SourceTextBuilder builder)
    {
        var parameters = GenerateMethodParameters(builder, _method.Parameters);
        var returnTypeName = builder.SimplifyNameByAddUsing(_method.ReturnType);
        var methodContainingTypeName = builder.SimplifyNameByAddUsing(_method.ContainingType);
        return new(
            builder,
            @$"
[IpcMethod]
{returnTypeName} {methodContainingTypeName}.{_method.Name}({parameters})
{{
    throw null;
}}
        ");
    }

    /// <summary>
    /// 生成此方法在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="real">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>方法源代码。</returns>
    public string GenerateJointMatch(SourceTextBuilder builder, string real)
    {
        var containingTypeName = builder.SimplifyNameByAddUsing(_method.ContainingType);
        var parameterTypes = GenerateMethodParameterTypes(builder, _method.Parameters);
        var arguments = GenerateMethodArguments(_method.Parameters);
        var asyncReturnType = GetAsyncReturnType(_method.ReturnType);
        var returnTypeName = asyncReturnType is null ? "void" : builder.SimplifyNameByAddUsing(asyncReturnType);
        var isAsync = _isAsyncMethod;
        var returnsVoid = _method.ReturnsVoid || asyncReturnType is null;

        if (isAsync && returnsVoid)
        {
            // 异步 Task 方法。
            var call = $"{real}.{_method.Name}({arguments})";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Func<Task>(() => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Func<{parameterTypes}, Task>(({arguments}) => {call}));";
            return sourceCode;
        }
        else if (isAsync && !returnsVoid)
        {
            // 异步 Task<T> 方法。
            var @return = $"Task<Garm<{returnTypeName}>>";
            var call = $"new Garm<{asyncReturnType}>(await {real}.{_method.Name}({arguments}).ConfigureAwait(false))";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Func<{@return}>(async () => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Func<{parameterTypes}, {@return}>(async ({arguments}) => {call}));";
            return sourceCode;
        }
        else if (!isAsync && returnsVoid)
        {
            // 同步 void 方法。
            var call = $"{real}.{_method.Name}({arguments})";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Action(() => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Action<{parameterTypes}>(({arguments}) => {call}));";
            return sourceCode;
        }
        else
        {
            // 同步 T 方法。
            var @return = $"Garm<{returnTypeName}>";
            var call = $"new Garm<{returnTypeName}>({real}.{_method.Name}({arguments}))";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Func<{@return}>(() => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_method.Name}), new Func<{parameterTypes}, {@return}>(({arguments}) => {call}));";
            return sourceCode;
        }
    }

    /// <summary>
    /// 根据参数列表生成方法形参列表字符串。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameters">方法参数列表。</param>
    /// <returns>方法形参列表字符串。</returns>
    private string GenerateMethodParameters(SourceTextBuilder builder, ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(
            ", ",
            parameters.Select(x => $"{builder.SimplifyNameByAddUsing(x.Type)} {x.Name}"));
    }

    /// <summary>
    /// 根据参数列表生成方法参数类型列表字符串。
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameters">方法参数列表。</param>
    /// <returns>方法参数类型列表字符串。</returns>
    private string GenerateMethodParameterTypes(SourceTextBuilder builder, ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(
            ", ",
            parameters.Select(x => builder.SimplifyNameByAddUsing(x.Type)));
    }

    /// <summary>
    /// 根据参数列表生成方法实参列表字符串。
    /// </summary>
    /// <param name="parameters">方法参数列表。</param>
    /// <returns>方法实参列表字符串。</returns>
    private string GenerateMethodArguments(ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(
            ", ",
            parameters.Select(x => $"{x.Name}"));
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
