using dotnetCampus.Ipc.SourceGenerators.Models;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;

internal class IpcPublicMethodInfo : IPublicIpcObjectProxyMemberGenerator, IPublicIpcObjectShapeMemberGenerator, IPublicIpcObjectJointMatchGenerator
{
    /// <summary>
    /// 契约接口类型的语义符号。
    /// </summary>
    /// <remarks>
    /// 如果需要获取接口定义相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly INamedTypeSymbol _contractType;

    /// <summary>
    /// IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）的语义符号。（可能与 <see cref="_ipcType"/> 是相同类型。）
    /// </summary>
    /// <remarks>
    /// 如果需要获取接口特性（<see cref="Attribute"/>）相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly INamedTypeSymbol _ipcType;

    /// <summary>
    /// 此方法原始定义（即在 <see cref="_contractType"/> 中所定义的方法）的语义符号。
    /// </summary>
    /// <remarks>
    /// 如果需要获取方法签名相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly IMethodSymbol _contractMethod;

    /// <summary>
    /// 标记了 <see cref="IpcMemberAttribute"/> 的此方法实现的语义符号。（可能与 <see cref="_contractMethod"/> 是相同实例。）
    /// </summary>
    /// <remarks>
    /// 如果需要获取方法特性（<see cref="Attribute"/>）相关的信息，应从此属性中获取。
    /// </remarks>
    private readonly IMethodSymbol _ipcMethod;

    /// <summary>
    /// 如果此成员是一个异步方法，则此值为 true；否则为 false。
    /// </summary>
    private readonly bool _isAsyncMethod;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="contractType">契约接口类型的语义符号。</param>
    /// <param name="ipcType">IPC 类型（即标记了 <see cref="IpcPublicAttribute"/> 或 <see cref="IpcShapeAttribute"/> 的类型）的语义符号。（可能与 <paramref name="contractType"/> 是相同类型。）</param>
    /// <param name="contractMethod">此成员原始定义（即在 <paramref name="contractType"/> 中所定义的方法）的语义符号。</param>
    /// <param name="ipcMethod">标记了 <see cref="IpcMemberAttribute"/> 的此成员实现的语义符号。（可能与 <paramref name="contractMethod"/> 是相同实例。）</param>
    public IpcPublicMethodInfo(INamedTypeSymbol contractType, INamedTypeSymbol ipcType, IMethodSymbol contractMethod, IMethodSymbol ipcMethod)
    {
        _contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        _ipcType = ipcType ?? throw new ArgumentNullException(nameof(contractType));
        _contractMethod = contractMethod ?? throw new ArgumentNullException(nameof(contractMethod));
        _ipcMethod = ipcMethod ?? throw new ArgumentNullException(nameof(contractMethod));
        var returnType = contractMethod.ReturnType.OriginalDefinition.ToString();
        _isAsyncMethod = returnType is "System.Threading.Tasks.Task" or "System.Threading.Tasks.Task<TResult>";
    }

    /// <summary>
    /// 生成此方法在 IPC 代理中的源代码。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns>方法源代码。</returns>
    public MemberDeclarationSourceTextBuilder GenerateProxyMember(SourceTextBuilder builder)
    {
        var parameters = GenerateMethodParameters(builder, _contractMethod.Parameters);
        var arguments = GenerateGarmArguments(builder, _contractMethod.Parameters);
        var asyncReturnType = GetAsyncReturnType(_contractMethod.ReturnType);
        var returnTypeName = asyncReturnType is null ? "void" : builder.SimplifyNameByAddUsing(asyncReturnType);
        var methodContainingTypeName = builder.SimplifyNameByAddUsing(_contractMethod.ContainingType);
        var isAsync = _isAsyncMethod;
        var returnsVoid = _contractMethod.ReturnsVoid || asyncReturnType is null;
        var namedValues = _ipcMethod.GetIpcNamedValues(asyncReturnType, _ipcType);

        return new(
            builder.AddUsing("System.Threading.Tasks"),
            (isAsync, returnsVoid) switch
            {
                // 异步 Task 方法。
                (true, true) => $@"

Task {methodContainingTypeName}.{_contractMethod.Name}({parameters})
{{
    return CallMethodAsync(new Garm<object?>[] {{ {arguments} }}, {namedValues});
}}

                ",
                // 异步 Task<T> 方法。
                (true, _) => $@"

Task<{returnTypeName}> {methodContainingTypeName}.{_contractMethod.Name}({parameters})
{{
    return CallMethodAsync<{returnTypeName}>(new Garm<object?>[] {{ {arguments} }}, {namedValues});
}}

                ",
                // 同步 void 方法。
                (false, true) => namedValues.WaitsVoid
                    ? @$"
void {methodContainingTypeName}.{_contractMethod.Name}({parameters})
{{
    CallMethod(new Garm<object?>[] {{ {arguments} }}, {namedValues}).Wait();
}}"
                    : @$"
void {methodContainingTypeName}.{_contractMethod.Name}({parameters})
{{
    _ = CallMethod(new Garm<object?>[] {{ {arguments} }}, {namedValues});
}}",

                // 同步 T 方法。
                (false, _) => $@"
{returnTypeName} {methodContainingTypeName}.{_contractMethod.Name}({parameters})
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
        var parameters = GenerateMethodParameters(builder, _contractMethod.Parameters);
        var returnTypeName = builder.SimplifyNameByAddUsing(_contractMethod.ReturnType);
        var methodContainingTypeName = builder.SimplifyNameByAddUsing(_contractMethod.ContainingType);
        return new(
            builder,
            @$"
[IpcMethod]
{returnTypeName} {methodContainingTypeName}.{_contractMethod.Name}({parameters})
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
        var containingTypeName = builder.SimplifyNameByAddUsing(_contractMethod.ContainingType);
        var parameterTypes = GenerateMethodParameterTypes(builder, _contractMethod.Parameters);
        var arguments = GenerateMethodArguments(_contractMethod.Parameters);
        var asyncReturnType = GetAsyncReturnType(_contractMethod.ReturnType);
        var returnTypeName = asyncReturnType is null ? "void" : builder.SimplifyNameByAddUsing(asyncReturnType);
        var isAsync = _isAsyncMethod;
        var returnsVoid = _contractMethod.ReturnsVoid || asyncReturnType is null;

        if (isAsync && returnsVoid)
        {
            // 异步 Task 方法。
            var call = $"{real}.{_contractMethod.Name}({arguments})";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Func<Task>(() => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Func<{parameterTypes}, Task>(({arguments}) => {call}));";
            return sourceCode;
        }
        else if (isAsync && !returnsVoid)
        {
            // 异步 Task<T> 方法。
            var @return = $"Task<Garm<{returnTypeName}>>";
            var call = GenerateGarmReturn(builder, asyncReturnType!, $"await {real}.{_contractMethod.Name}({arguments}).ConfigureAwait(false)");
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Func<{@return}>(async () => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Func<{parameterTypes}, {@return}>(async ({arguments}) => {call}));";
            return sourceCode;
        }
        else if (!isAsync && returnsVoid)
        {
            // 同步 void 方法。
            var call = $"{real}.{_contractMethod.Name}({arguments})";
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Action(() => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Action<{parameterTypes}>(({arguments}) => {call}));";
            return sourceCode;
        }
        else
        {
            // 同步 T 方法。
            var @return = $"Garm<{returnTypeName}>";
            var call = GenerateGarmReturn(builder, _contractMethod.ReturnType, $"{real}.{_contractMethod.Name}({arguments})");
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Func<{@return}>(() => {call}));"
                : $"MatchMethod(nameof({containingTypeName}.{_contractMethod.Name}), new Func<{parameterTypes}, {@return}>(({arguments}) => {call}));";
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
    /// 根据参数列表生成方法实参列表字符串，同时将原参数改为 Garm 类型的参数，以支持 IPC 对象的跨进程传输。
    /// <para>这么做是因为以下语句在 instance 实例为接口时因无法隐式转换而编译不通过，在其他情况下却可以：</para>
    /// <c>new Garm&lt;object?&gt;[] { instance }</c>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="parameters">方法参数列表。</param>
    /// <returns>方法实参列表字符串。</returns>
    private string GenerateGarmArguments(SourceTextBuilder builder, ImmutableArray<IParameterSymbol> parameters)
    {
        return string.Join(
            ", ",
            parameters.Select(x => x.Type.GetIsIpcType()
                ? $"new Garm<{builder.SimplifyNameByAddUsing(x.Type)}>({x.Name}, typeof({x.Type.Name}))"
                : $"new Garm<{builder.SimplifyNameByAddUsing(x.Type)}>({x.Name})"));
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

    /// <summary>
    /// 根据方法返回值生成返回值字符串，同时将原参数改为 Garm 类型的参数，以支持 IPC 对象的跨进程传输。
    /// <para>这么做是因为以下语句在 instance 实例为接口时因无法隐式转换而编译不通过，在其他情况下却可以：</para>
    /// <c>new Garm&lt;object?&gt;[] { instance }</c>
    /// </summary>
    /// <param name="builder"></param>
    /// <param name="return">方法返回值类型。</param>
    /// <param name="value">方法返回值。</param>
    /// <returns>方法实参列表字符串。</returns>
    private string GenerateGarmReturn(SourceTextBuilder builder, ITypeSymbol @return, string value)
    {
        return @return.GetIsIpcType()
                ? $"new Garm<{builder.SimplifyNameByAddUsing(@return)}>({value}, typeof({@return.Name}))"
                : $"new Garm<{builder.SimplifyNameByAddUsing(@return)}>({value})";
    }
}
