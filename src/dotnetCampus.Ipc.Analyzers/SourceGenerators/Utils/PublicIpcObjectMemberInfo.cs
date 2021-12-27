using System.Collections.Immutable;
using System.Linq;

using dotnetCampus.Ipc.Analyzers.Core;
using dotnetCampus.Ipc.CompilerServices.Attributes;

using Microsoft.CodeAnalysis;

using static dotnetCampus.Ipc.Analyzers.Core.Diagnostics;

namespace dotnetCampus.Ipc.Analyzers.SourceGenerators.Utils;

/// <summary>
/// 辅助生成契约接口中每一个成员对应的 IPC 代理和对接。
/// </summary>
internal class PublicIpcObjectMemberInfo
{
    /// <summary>
    /// 此成员在接口定义中的语义符号。
    /// </summary>
    private readonly ISymbol _interfaceMember;

    /// <summary>
    /// 此成员在类型实现中的语义符号。
    /// </summary>
    private readonly ISymbol _implementationMember;

    /// <summary>
    /// 契约接口的语义符号。
    /// </summary>
    private readonly INamedTypeSymbol _contractType;

    /// <summary>
    /// 如果此成员是一个异步方法，则此值为 true；否则为 false。
    /// </summary>
    private readonly bool _isAsyncMethod;

    /// <summary>
    /// 创建 IPC 对象的其中一个成员信息。
    /// </summary>
    /// <param name="contractType">契约接口的语义符号。</param>
    /// <param name="interfaceMember">此成员在接口定义中的语义符号。</param>
    /// <param name="implementationMember">此成员在类型实现中的语义符号。</param>
    public PublicIpcObjectMemberInfo(INamedTypeSymbol contractType, ISymbol interfaceMember, ISymbol implementationMember)
    {
        _contractType = contractType ?? throw new ArgumentNullException(nameof(contractType));
        _interfaceMember = interfaceMember ?? throw new ArgumentNullException(nameof(interfaceMember));
        _implementationMember = implementationMember ?? throw new ArgumentNullException(nameof(implementationMember));

        if (interfaceMember is IMethodSymbol method)
        {
            var returnType = method.ReturnType.OriginalDefinition.ToString();
            _isAsyncMethod = returnType is "System.Threading.Tasks.Task" or "System.Threading.Tasks.Task<TResult>";
        }
    }

    /// <summary>
    /// 生成此成员在 IPC 代理中的源代码。
    /// </summary>
    /// <returns>成员源代码。</returns>
    public string GenerateProxyMember()
    {
        return _interfaceMember switch
        {
            IMethodSymbol methodSymbol => GenerateProxyMethod(methodSymbol),
            IPropertySymbol propertySymbol => GenerateProxyProperty(propertySymbol),
            _ => throw new DiagnosticException(
                DIPC003_OnlyMethodOrPropertyIsSupported,
                _implementationMember.Locations.FirstOrDefault(),
                _interfaceMember.Name),
        };
    }

    /// <summary>
    /// 生成此方法在 IPC 代理中的源代码。
    /// </summary>
    /// <param name="method">此方法的语义符号。</param>
    /// <returns>方法源代码。</returns>
    private string GenerateProxyMethod(IMethodSymbol method)
    {
        if (_isAsyncMethod)
        {
            var parameters = GenerateMethodParameters(method.Parameters);
            var arguments = GenerateMethodArguments(method.Parameters);
            var asyncReturnType = GetAsyncReturnType(method.ReturnType);
            var sourceCode = asyncReturnType is null
                ? @$"        public System.Threading.Tasks.Task {method.Name}({parameters})
        {{
            return CallMethodAsync(new object[] {{ {arguments} }});
        }}"
                : @$"        public System.Threading.Tasks.Task<{asyncReturnType}> {method.Name}({parameters})
        {{
            return CallMethodAsync<{asyncReturnType}>(new object[] {{ {arguments} }});
        }}";
            return sourceCode;
        }
        else if (method.ReturnsVoid)
        {
            var waitVoid = CheckWaitingVoid();
            var parameters = GenerateMethodParameters(method.Parameters);
            var arguments = GenerateMethodArguments(method.Parameters);
            var sourceCode = waitVoid
                ? @$"        public void {method.Name}({parameters})
        {{
            CallMethod(new object[] {{ {arguments} }}).Wait();
        }}"
                : @$"        public void {method.Name}({parameters})
        {{
            _ = CallMethod(new object[] {{ {arguments} }});
        }}";
            return sourceCode;
        }
        else
        {
            var parameters = GenerateMethodParameters(method.Parameters);
            var arguments = GenerateMethodArguments(method.Parameters);
            var sourceCode = @$"        public {method.ReturnType} {method.Name}({parameters})
        {{
            return CallMethod<{method.ReturnType}>(new object[] {{ {arguments} }}).Result;
        }}";
            return sourceCode;
        }
    }

    /// <summary>
    /// 生成此属性在 IPC 代理中的源代码。
    /// </summary>
    /// <param name="property">此属性的语义符号。</param>
    /// <returns>属性源代码。</returns>
    private string GenerateProxyProperty(IPropertySymbol property)
    {
        var isReadonly = CheckIsReadonly();
        var getterName = isReadonly ? "GetReadonlyValueAsync" : "GetValueAsync";
        if (property.GetMethod is { } getMethod && property.SetMethod is { } setMethod)
        {
            var sourceCode = $@"        public {property.Type} {property.Name}
        {{
            get => {getterName}<{property.Type}>().Result;
            set => SetValueAsync(value).Wait();
        }}";
            return sourceCode;
        }
        else if (property.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"        public {property.Type} {property.Name} => {getterName}<{property.Type}>().Result;";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC004_OnlyGetOrGetSetPropertyIsSupported,
                _implementationMember.Locations.FirstOrDefault(),
                _interfaceMember.Name);
        }
    }

    /// <summary>
    /// 生成此成员在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="realInstanceVariableName">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>成员源代码。</returns>
    public string GenerateJointMatch(string realInstanceVariableName)
    {
        return _interfaceMember switch
        {
            IMethodSymbol methodSymbol => GenerateJointMethodMatch(methodSymbol, realInstanceVariableName),
            IPropertySymbol propertySymbol => GenerateJointPropertyMatch(propertySymbol, realInstanceVariableName),
            _ => throw new DiagnosticException(
                DIPC003_OnlyMethodOrPropertyIsSupported,
                _implementationMember.Locations.FirstOrDefault(),
                _interfaceMember.Name),
        };
    }

    /// <summary>
    /// 生成此方法在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="method">此方法的语义符号。</param>
    /// <param name="real">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>方法源代码。</returns>
    private string GenerateJointMethodMatch(IMethodSymbol method, string real)
    {
        if (method.ReturnsVoid)
        {
            var arguments = GenerateMethodArguments(method.Parameters);
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_contractType}.{method.Name}), new System.Action(() => {real}.{method.Name}()));"
                : $"MatchMethod(nameof({_contractType}.{method.Name}), new System.Action<{GenerateMethodParameterTypes(method.Parameters)}>(({arguments}) => {real}.{method.Name}({arguments})));";
            return sourceCode;
        }
        else
        {
            var arguments = GenerateMethodArguments(method.Parameters);
            var sourceCode = string.IsNullOrWhiteSpace(arguments)
                ? $"MatchMethod(nameof({_contractType}.{method.Name}), new System.Func<{method.ReturnType}>(() => {real}.{method.Name}()));"
                : $"MatchMethod(nameof({_contractType}.{method.Name}), new System.Func<{GenerateMethodParameterTypes(method.Parameters)}, {method.ReturnType}>(({arguments}) => {real}.{method.Name}({arguments})));";
            return sourceCode;
        }
    }

    /// <summary>
    /// 生成此属性在 IPC 对接中的源代码。
    /// </summary>
    /// <param name="property">此属性的语义符号。</param>
    /// <param name="real">IPC 对接方法中真实实例的实参名称。</param>
    /// <returns>属性源代码。</returns>
    private string GenerateJointPropertyMatch(IPropertySymbol property, string real)
    {
        if (property.GetMethod is { } getMethod && property.SetMethod is { } setMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_contractType}.{property.Name}), new System.Func<{property.Type}>(() => {real}.{property.Name}), new System.Action<{property.Type}>(value => {real}.{property.Name} = value));";
            return sourceCode;
        }
        else if (property.GetMethod is { } getOnlyMethod)
        {
            var sourceCode = $"MatchProperty(nameof({_contractType}.{property.Name}), new System.Func<{property.Type}>(() => {real}.{property.Name}));";
            return sourceCode;
        }
        else
        {
            throw new DiagnosticException(
                DIPC004_OnlyGetOrGetSetPropertyIsSupported,
                _implementationMember.Locations.FirstOrDefault(),
                _interfaceMember.Name);
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
    private string? GetAsyncReturnType(ITypeSymbol returnType)
    {
        if (returnType is INamedTypeSymbol namedReturnType)
        {
            if (_isAsyncMethod)
            {
                if (namedReturnType.TypeArguments.FirstOrDefault() is { } returnArgument)
                {
                    // Task<TResult>
                    return returnArgument.ToString();
                }
                else
                {
                    // Task
                    return null;
                }
            }
            else
            {
                return returnType.ToString();
            }
        }
        else
        {
            return returnType.ToString();
        }
    }

    /// <summary>
    /// 检查此成员是否标记了 <see cref="IpcMethodAttribute.WaitsVoid"/>。
    /// </summary>
    /// <returns></returns>
    private bool CheckWaitingVoid()
    {
        if (GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.WaitsVoid)) is { } value)
        {
            return string.Equals(value.ToString(), "true", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    /// <summary>
    /// 检查此成员是否标记了 <see cref="IpcPropertyAttribute.IsReadonly"/>。
    /// </summary>
    /// <returns></returns>
    private bool CheckIsReadonly()
    {
        if (GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IsReadonly)) is { } value)
        {
            return string.Equals(value.ToString(), "true", StringComparison.OrdinalIgnoreCase);
        }
        return false;
    }

    /// <summary>
    /// 检查此成员的 <typeparamref name="TAttribute"/> 类型特性的 <paramref name="namedArgumentName"/> 名字的参数的值。
    /// </summary>
    /// <typeparam name="TAttribute">特性类型。</typeparam>
    /// <typeparam name="T">参数类型。</typeparam>
    /// <param name="namedArgumentName">参数名称。</param>
    /// <returns>参数的值。</returns>
    private T GetAttributeValue<TAttribute, T>(string namedArgumentName)
    {
        var value = GetAttributeValue(typeof(TAttribute).FullName, namedArgumentName);
        if (typeof(T) == typeof(bool))
        {
            return (T) (object) Convert.ToBoolean(value);
        }
        throw new NotSupportedException("尚不支持读取其他类型的特性。");
    }

    /// <summary>
    /// 检查此成员的 <paramref name="attributeTypeName"/> 类型特性的 <paramref name="namedArgumentName"/> 名字的参数的值。
    /// </summary>
    /// <param name="attributeTypeName">特性类型的名称。</param>
    /// <param name="namedArgumentName">参数名称。</param>
    /// <returns>参数的值。</returns>
    private object? GetAttributeValue(string attributeTypeName, string namedArgumentName)
    {
        if (_implementationMember.GetAttributes().FirstOrDefault(x => string.Equals(
             x.AttributeClass?.ToString(),
             attributeTypeName,
             StringComparison.Ordinal)) is { } ipcMethodAttribute)
        {
            var value = ipcMethodAttribute.NamedArguments
                .FirstOrDefault(x => x.Key == namedArgumentName).Value;
            return value.Value;
        }
        return null;
    }
}
