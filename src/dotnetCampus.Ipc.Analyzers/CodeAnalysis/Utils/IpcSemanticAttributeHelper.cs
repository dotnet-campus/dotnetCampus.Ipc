using dotnetCampus.Ipc.CodeAnalysis.Models;

namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

/// <summary>
/// 包含 <see cref="IpcMemberAttribute"/> 相关语义分析的辅助扩展方法。
/// </summary>
internal static class IpcSemanticAttributeHelper
{
    /// <summary>
    /// 检查此类型上标记的 <see cref="IpcPublicAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    internal static IpcAttributeNamedValues GetIpcNamedValues(this INamedTypeSymbol type)
    {
        var contractType = type.GetAttributedContractType<IpcPublicAttribute>();
        var ignoresIpcException = type.GetAttributeValue<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException));
        var timeout = type.GetAttributeValue<IpcPublicAttribute, int>(nameof(IpcPublicAttribute.Timeout));
        return new IpcAttributeNamedValues(contractType, type)
        {
            Timeout = timeout,
            IgnoresIpcException = ignoresIpcException,
        };
    }

    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="method"></param>
    /// <param name="returnTypeSymbol">返回值类型，依此来决定如何将 Attribute 里的对象转为字符串。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    internal static IpcAttributeNamedValues GetIpcNamedValues(this IMethodSymbol method, ITypeSymbol? returnTypeSymbol, INamedTypeSymbol containingType)
    {
        var type = containingType;
        var contractType = method.GetAttributedContractType<IpcMemberAttribute>()
            ?? type.GetAttributedContractType<IpcPublicAttribute>();
        var waitsVoid = method.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.WaitsVoid));
        var ignoresIpcException = method.GetAttributeValue<IpcMethodAttribute, bool>(nameof(IpcMethodAttribute.IgnoresIpcException))
            ?? type.GetAttributeValue<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException));
        var defaultReturn = method.GetAttributeValue<IpcMethodAttribute, object?>(nameof(IpcMethodAttribute.DefaultReturn));
        var timeout = method.GetAttributeValue<IpcMethodAttribute, int>(nameof(IpcMethodAttribute.Timeout))
            ?? type.GetAttributeValue<IpcPublicAttribute, int>(nameof(IpcPublicAttribute.Timeout));
        return new IpcAttributeNamedValues(contractType, containingType, method, returnTypeSymbol)
        {
            DefaultReturn = defaultReturn,
            IgnoresIpcException = ignoresIpcException,
            Timeout = timeout,
            WaitsVoid = waitsVoid
        };
    }

    /// <summary>
    /// 检查此方法上标记的 <see cref="IpcMethodAttribute"/> 并将其转换为传入 IPC 代理的类型。
    /// </summary>
    /// <param name="property">要查找标记的属性。</param>
    /// <param name="containingType">要查找标记的类型。注意，当此属性来自于继承类时，属性所在的类型和真实要分析的类型不相同。</param>
    /// <returns></returns>
    public static IpcAttributeNamedValues GetIpcNamedValues(this IPropertySymbol property, INamedTypeSymbol containingType)
    {
        var type = containingType;
        var contractType = property.GetAttributedContractType<IpcMemberAttribute>()
            ?? type.GetAttributedContractType<IpcPublicAttribute>();
        var isReadonly = property.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IsReadonly));
        var ignoresIpcException = property.GetAttributeValue<IpcPropertyAttribute, bool>(nameof(IpcPropertyAttribute.IgnoresIpcException))
            ?? type.GetAttributeValue<IpcPublicAttribute, bool>(nameof(IpcPublicAttribute.IgnoresIpcException));
        var defaultReturn = property.GetAttributeValue<IpcPropertyAttribute, object?>(nameof(IpcPropertyAttribute.DefaultReturn));
        var timeout = property.GetAttributeValue<IpcPropertyAttribute, int>(nameof(IpcPropertyAttribute.Timeout))
            ?? type.GetAttributeValue<IpcPublicAttribute, int>(nameof(IpcPublicAttribute.Timeout));
        return new IpcAttributeNamedValues(contractType, containingType, property, property.Type)
        {
            DefaultReturn = defaultReturn,
            IgnoresIpcException = ignoresIpcException,
            IsReadonly = isReadonly,
            Timeout = timeout,
        };
    }

    private static INamedTypeSymbol? GetAttributedContractType<TAttribute>(this ISymbol symbol)
    {
        var attributeTypeName = typeof(TAttribute).FullName;
        if (symbol.GetAttributes().FirstOrDefault(x => string.Equals(
             x.AttributeClass?.ToString(),
             attributeTypeName,
             StringComparison.Ordinal)) is { } ipcMethodAttribute)
        {
            var typedConstant = ipcMethodAttribute.ConstructorArguments.FirstOrDefault();
            if (typedConstant.Kind is TypedConstantKind.Type)
            {
                return typedConstant.Value as INamedTypeSymbol;
            }
        }
        return null;
    }
}
