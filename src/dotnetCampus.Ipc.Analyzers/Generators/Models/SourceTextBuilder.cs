using dotnetCampus.Ipc.Generators.Utils;

namespace dotnetCampus.Ipc.Generators.Models;

internal class SourceTextBuilder
{
    private readonly SortedList<string, string> _usingNamespaces = new();
    private string? _namespaceName;
    private readonly List<ClassDeclarationSourceTextBuilder> _typeDeclarationBuilders = new();

    public SourceTextBuilder AddUsing(string usingNamespace)
    {
        if (string.IsNullOrWhiteSpace(usingNamespace))
        {
            throw new ArgumentException($"“{nameof(usingNamespace)}”不能为 null 或空白。", nameof(usingNamespace));
        }

        if (!_usingNamespaces.ContainsKey(usingNamespace))
        {
            _usingNamespaces.Add(usingNamespace, usingNamespace);
        }
        return this;
    }

    public SourceTextBuilder AddUsingForTypes(params ITypeSymbol[] typeSymbols)
    {
        foreach (var typeSymbol in typeSymbols)
        {
            AddUsing(typeSymbol.ContainingNamespace.ToString());
        }
        return this;
    }

    public string SimplifyNameByAddUsing(ITypeSymbol typeSymbol)
    {
        var originalName = typeSymbol.ToString();
        var @namespace = typeSymbol.ContainingNamespace?.ToString();
        if (@namespace == null)
        {
            return originalName;
        }
        if (originalName.StartsWith(@namespace, StringComparison.Ordinal))
        {
            // 常规类型，“命名空间.类名”型。
            var recursiveTypeName = GetNestedTypeNameRecursively(typeSymbol);
            var nullablePostfix = typeSymbol.NullableAnnotation is NullableAnnotation.Annotated
                ? "?"
                : "";
            AddUsing(@namespace);
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol
                && namedTypeSymbol.TypeArguments.Length > 0)
            {
                if (namedTypeSymbol.Name == "Nullable"
                    && string.Equals(typeSymbol.ContainingNamespace.ToString(), "System", StringComparison.Ordinal))
                {
                    // Nullable<T> 类型。
                    return $"{SimplifyNameByAddUsing(namedTypeSymbol.TypeArguments[0])}?";
                }
                else
                {
                    // Class<T> 类型。
                    var typeArgumentNames = string.Join(
                        ", ",
                        namedTypeSymbol.TypeArguments.Select(x => SimplifyNameByAddUsing(x)));
                    return $"{recursiveTypeName}<{typeArgumentNames}>{nullablePostfix}";
                }
            }
            else
            {
                // T 类型。
                return recursiveTypeName + nullablePostfix;
            }
        }
        else
        {
            if (typeSymbol is INamedTypeSymbol namedTypeSymbol
                && namedTypeSymbol.TypeArguments.Length > 0
                && string.Equals(namedTypeSymbol.Name, "ValueTuple")
                && string.Equals(namedTypeSymbol.ContainingNamespace.ToString(), "System"))
            {
                // ValueTuple 类型（以后再说）。
                var tupleMembers = string.Join(", ", namedTypeSymbol.TupleElements
                    .Select(x => $"{SimplifyNameByAddUsing(x.Type)} {x.Name}"));
                return $"({tupleMembers})";
            }
            else
            {
                // 特殊类型（如 Boolean/Void）。
                return originalName;
            }
        }
    }

    /// <summary>
    /// 返回一个类型的嵌套内部类名称。
    /// 无视特殊类型（如 Nullable、ValueTuple 等），因此请勿对特殊类型调用此方法。
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <returns></returns>
    private string GetNestedTypeNameRecursively(ITypeSymbol typeSymbol)
    {
        if (typeSymbol.ContainingType is { } containingType)
        {
            return $"{GetNestedTypeNameRecursively(containingType)}.{typeSymbol.Name}";
        }
        return typeSymbol.Name;
    }

    public SourceTextBuilder DeclareNamespace(string namespaceName)
    {
        if (string.IsNullOrWhiteSpace(namespaceName))
        {
            throw new ArgumentException($"“{nameof(namespaceName)}”不能为 null 或空白。", nameof(namespaceName));
        }

        if (_namespaceName is not null)
        {
            throw new NotSupportedException($"只支持声明一次命名空间，而之前已经声明过 {_namespaceName}。");
        }

        _namespaceName = namespaceName;
        return this;
    }

    public SourceTextBuilder AddClassDeclaration(Func<SourceTextBuilder, ClassDeclarationSourceTextBuilder> classDeclarationBuilder)
    {
        if (classDeclarationBuilder is null)
        {
            throw new ArgumentNullException(nameof(classDeclarationBuilder));
        }

        _typeDeclarationBuilders.Add(classDeclarationBuilder(this));
        return this;
    }

    public override string ToString() => ToString(false);

    public string ToString(bool useFileScopedNamespace)
    {
        var builder = new StringBuilder();
        foreach (var usingNamespace in _usingNamespaces)
        {
            builder.Append($"using {usingNamespace.Value};");
        }
        var classes = string.Join(
            "\r\n",
            _typeDeclarationBuilders.Select(x => x.ToString()));
        if (useFileScopedNamespace)
        {
            builder.Append($@"
namespace {_namespaceName};
{classes}
");
        }
        else
        {
            builder.Append($@"
namespace {_namespaceName}
{{
    {classes}
}}
");
        }
        return GeneratorHelper.FormatCode(builder.ToString());
    }
}
