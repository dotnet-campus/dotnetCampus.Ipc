namespace dotnetCampus.Ipc.CodeAnalysis.Models;

/// <summary>
/// 仅供 <see cref="GeneratedIpcProxy"/> 的自动生成的派生类与基类传递参数使用，包含参数传递所需的各种个性化需求。
/// </summary>
internal class IpcAttributeNamedValues
{
    public IpcAttributeNamedValues(INamedTypeSymbol? contractType, INamedTypeSymbol? realType)
    {
        ContractType = contractType;
        RealType = realType;
    }

    public IpcAttributeNamedValues(INamedTypeSymbol? contractType, INamedTypeSymbol? realType, ISymbol? member, ITypeSymbol? memberReturnType)
    {
        ContractType = contractType;
        RealType = realType;
        Member = member;
        MemberReturnType = memberReturnType;
    }

    public INamedTypeSymbol? ContractType { get; }

    public INamedTypeSymbol? RealType { get; }

    public ISymbol? Member { get; }

    public ITypeSymbol? MemberReturnType { get; }

    public Assignable<object?>? DefaultReturn { get; set; }

    public Assignable<bool>? IgnoresIpcException { get; set; }

    public Assignable<bool>? IsReadonly { get; set; }

    public Assignable<int>? Timeout { get; set; }

    public Assignable<bool>? WaitsVoid { get; set; }

    public override string ToString()
    {
        var quoteObject = MemberReturnType?.ToString() == "string";
        return $@"new()
{{
    {Format(nameof(DefaultReturn), DefaultReturn, x => Format(x, quoteObject))}
    {Format(nameof(Timeout), Timeout)}
    {Format(nameof(IgnoresIpcException), IgnoresIpcException)}
    {Format(nameof(IsReadonly), IsReadonly)}
    {Format(nameof(WaitsVoid), WaitsVoid)}
}}";
    }

    private string Format<T>(string name, Assignable<T>? assignable, Func<T?, string>? customFormatter = null)
    {
        if (assignable is null)
        {
            // 未赋值。
            return "";
        }

        var value = assignable.Value.Value;
        if (customFormatter is not null)
        {
            return $"{name} = {customFormatter(value)},";
        }

        if (typeof(T) == typeof(bool) && value is bool booleanValue)
        {
            return $"{name} = {(booleanValue ? "true" : "false")},";
        }
        if (value is int int32Value)
        {
            return $"{name} = {int32Value.ToString(CultureInfo.InvariantCulture)},";
        }

        throw new NotSupportedException("暂不支持其他类型的字符串输出。");
    }

    /// <summary>
    /// 将 Attribute 里的对象转为字符串。
    /// </summary>
    /// <param name="value"></param>
    /// <param name="quoteObject">在格式化参数时，如果非 null，是否应该用引号将其包围。</param>
    /// <returns></returns>
    private static string Format(object? value, bool quoteObject)
    {
        if (value is null)
        {
            return "null";
        }
        else if (quoteObject)
        {
            return $@"""{value}""";
        }
        else
        {
            return value.ToString();
        }
    }
}
