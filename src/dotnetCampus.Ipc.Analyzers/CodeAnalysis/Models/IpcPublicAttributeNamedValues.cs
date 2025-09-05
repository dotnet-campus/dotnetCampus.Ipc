namespace dotnetCampus.Ipc.CodeAnalysis.Models;

/// <summary>
/// 仅供 GeneratedIpcProxy 的自动生成的派生类与基类传递参数使用，包含参数传递所需的各种个性化需求。
/// </summary>
internal class IpcPublicAttributeNamedValues
{
    public IpcPublicAttributeNamedValues(INamedTypeSymbol? ipcType)
    {
        IpcType = ipcType;
    }

    public IpcPublicAttributeNamedValues(INamedTypeSymbol? ipcType, ISymbol? member, ITypeSymbol? memberReturnType)
    {
        IpcType = ipcType;
        Member = member;
        MemberReturnType = memberReturnType;
    }

    public INamedTypeSymbol? IpcType { get; }

    public ISymbol? Member { get; }

    public ITypeSymbol? MemberReturnType { get; }

    public Assignable<object?>? DefaultReturn { get; init; }

    public Assignable<bool>? IgnoresIpcException { get; init; }

    public Assignable<bool>? IsReadonly { get; init; }

    public Assignable<int>? Timeout { get; init; }

    public Assignable<bool>? WaitsVoid { get; init; }

    public override string ToString() => ToIndentString("    ", 0);

    public string ToIndentString(string indent, int baseIndentLevel = 1)
    {
        var baseIndent = string.Concat(Enumerable.Repeat(indent, baseIndentLevel));
        List<string> assignments =
        [
            Format(nameof(DefaultReturn), DefaultReturn, x => Format(x, MemberReturnType)),
            Format(nameof(Timeout), Timeout),
            Format(nameof(IgnoresIpcException), IgnoresIpcException),
            Format(nameof(IsReadonly), IsReadonly),
            Format(nameof(WaitsVoid), WaitsVoid),
        ];
        assignments.RemoveAll(string.IsNullOrEmpty);
        if (assignments.Count is 0)
        {
            return "default";
        }

        var builder = new StringBuilder();
        builder.AppendLine("new()");
        builder.Append(baseIndent).AppendLine("{");
        foreach (var assignment in assignments)
        {
            builder.Append(baseIndent).Append(indent).AppendLine(assignment);
        }
        builder.Append(baseIndent).Append("}");
        return builder.ToString();
    }

    protected string Format<T>(string name, Assignable<T>? assignable, Func<T?, string>? customFormatter = null)
    {
        if (assignable is null)
        {
            // 未赋值。
            return "";
        }

        var value = assignable.Value.Value;
        return customFormatter is null
            ? $"{name} = {Format(value, typeof(T))},"
            : $"{name} = {customFormatter(value)},";
    }

    /// <summary>
    /// 将 Attribute 里设置的值转为字符串。
    /// </summary>
    /// <param name="value">值。</param>
    /// <param name="valueType">值的类型。</param>
    /// <returns></returns>
    protected static string Format(object? value, Type valueType)
    {
        if (value is null)
        {
            return "null";
        }
        else if (valueType == typeof(string))
        {
            return $@"""{value}""";
        }
        if (valueType == typeof(bool) && value is bool booleanValue)
        {
            return booleanValue ? "true" : "false";
        }
        if (value is int int32Value)
        {
            // 可能会出现隐式转换，所以难以判断目标类型。
            return int32Value.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            return value.ToString();
        }
    }

    /// <summary>
    /// 将 Attribute 里设置的值转为字符串。
    /// </summary>
    /// <param name="value">值。</param>
    /// <param name="valueType">值的类型。</param>
    /// <returns></returns>
    protected static string Format(object? value, ITypeSymbol? valueType)
    {
        if (value is null)
        {
            return "null";
        }
        else if (valueType?.ToString() == "string")
        {
            return $@"""{value}""";
        }
        if (valueType?.ToString() == "bool" && value is bool booleanValue)
        {
            return booleanValue ? "true" : "false";
        }
        if (value is int int32Value)
        {
            return int32Value.ToString(CultureInfo.InvariantCulture);
        }
        else
        {
            return value.ToString();
        }
    }
}
