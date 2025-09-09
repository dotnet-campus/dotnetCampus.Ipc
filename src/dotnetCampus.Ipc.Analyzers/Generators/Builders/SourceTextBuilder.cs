namespace dotnetCampus.Ipc.Generators.Builders;

public class SourceTextBuilder : IDisposable
{
    private readonly HashSet<string> _systemUsings = [];
    private readonly HashSet<string> _otherUsings = [];
    private readonly HashSet<string> _staticUsings = [];
    private readonly HashSet<string> _aliasUsings = [];
    private readonly List<BracketSourceTextBuilder> _typeDeclarations = [];
    private readonly IDisposable _scope;

    public SourceTextBuilder(string @namespace)
    {
        Namespace = @namespace;
        _scope = SourceTextBuilderExtensions.BeginBuild(this);
    }

    public bool UseFileScopedNamespace { get; init; } = true;

    public string Namespace { get; }

    public string Indent { get; init; } = "    ";

    public bool AppendNewLineAtEnd { get; init; } = true;

    /// <summary>
    /// 是否允许通过 using 引用命名空间，从而简化类型名称。<br/>
    /// 如果允许，则在生成类型名称时，会尝试将类型的命名空间添加到 using 列表中，并返回简化后的类型名称字符串。<br/>
    /// 如果不允许，则生成类型名称时，直接返回完整类型名称字符串。
    /// </summary>
    public bool SimplifyTypeNamesByUsingNamespace { get; init; }

    /// <summary>
    /// 是否给所有涉及到命名空间的代码添加 global:: 前缀。<br/>
    /// 在 <see cref="SimplifyTypeNamesByUsingNamespace"/> 为 <see langword="true"/> 时，此选项无效。
    /// </summary>
    public bool ShouldPrependGlobal { get; init; } = true;

    public SourceTextBuilder Using(string usingNamespace)
    {
        var ns = usingNamespace.PrependGlobal(ShouldPrependGlobal);
        var systemNamespacePrefix = ShouldPrependGlobal ? "global::System" : "System";
        var isSystemNamespace = ns.Equals(systemNamespacePrefix, StringComparison.Ordinal)
                                || ns.StartsWith($"{systemNamespacePrefix}.", StringComparison.Ordinal);
        if (isSystemNamespace)
        {
            _systemUsings.Add(ns);
        }
        else
        {
            _otherUsings.Add(ns);
        }
        return this;
    }

    public SourceTextBuilder UsingStatic(string usingNamespace)
    {
        _staticUsings.Add(usingNamespace.PrependGlobal(ShouldPrependGlobal));
        return this;
    }

    public SourceTextBuilder UsingTypeAlias(string alias, string fullTypeName)
    {
        _aliasUsings.Add($"{alias} = {fullTypeName.PrependGlobal(ShouldPrependGlobal)}");
        return this;
    }

    public SourceTextBuilder AddRawText(string rawText)
    {
        var rawDeclaration = new RawSourceTextBuilder(this)
        {
            RawText = rawText,
        };
        _typeDeclarations.Add(rawDeclaration);
        return this;
    }

    public SourceTextBuilder AddTypeDeclaration(string declarationLine,
        Action<TypeDeclarationSourceTextBuilder> typeDeclarationBuilder)
    {
        var typeDeclaration = new TypeDeclarationSourceTextBuilder(this, declarationLine);
        typeDeclarationBuilder(typeDeclaration);
        _typeDeclarations.Add(typeDeclaration);
        return this;
    }

    public void Dispose()
    {
        _scope.Dispose();
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        BuildInto(builder, 0);
        return builder.ToString();
    }

    public void BuildInto(StringBuilder builder, int indentLevel)
    {
        builder.AppendLine("#nullable enable");

        // usings
        foreach (var line in _systemUsings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"using {line};");
        }
        foreach (var line in _otherUsings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"using {line};");
        }
        foreach (var line in _staticUsings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"using static {line};");
        }
        foreach (var line in _aliasUsings.OrderBy(x => x, StringComparer.OrdinalIgnoreCase))
        {
            builder.AppendLine($"using {line};");
        }
        if (_systemUsings.Count > 0 || _otherUsings.Count > 0 || _staticUsings.Count > 0 || _aliasUsings.Count > 0)
        {
            builder.AppendLine();
        }

        // namespace
        if (UseFileScopedNamespace)
        {
            builder.AppendLine($"namespace {Namespace};").AppendLine();
        }
        else
        {
            builder.AppendLine($"namespace {Namespace}");
        }
        using var _ = UseFileScopedNamespace
            ? EmptyScope.Begin()
            : BracketScope.Begin(builder, Indent, indentLevel);
        var typeIndentLevel = UseFileScopedNamespace ? 0 : indentLevel + 1;

        // types
        for (var i = 0; i < _typeDeclarations.Count; i++)
        {
            if (i > 0)
            {
                builder.AppendLine();
            }

            _typeDeclarations[i].BuildInto(builder, typeIndentLevel);
        }

        // 统一化换行符
        builder.Replace("\r", "");

        // 确保最后有且仅有一个换行符
        while (builder.Length > 0 && char.IsWhiteSpace(builder[^1]))
        {
            builder.Length--;
        }
        if (AppendNewLineAtEnd)
        {
            builder.AppendLine();
        }
    }
}

public abstract class BracketSourceTextBuilder(SourceTextBuilder root)
{
    public SourceTextBuilder Root => root;

    public string Indent => root.Indent;

    public abstract void BuildInto(StringBuilder builder, int indentLevel);

    protected void BuildMembersInto(StringBuilder builder, int indentLevel, List<BracketSourceTextBuilder> members)
    {
        for (var i = 0; i < members.Count; i++)
        {
            if (i > 0)
            {
                builder.AppendLine();
            }

            members[i].BuildInto(builder, indentLevel);
        }
    }
}

public class TypeDeclarationSourceTextBuilder(SourceTextBuilder root, string declarationLine) : BracketSourceTextBuilder(root)
{
    private readonly List<string> _attributes = [];
    private readonly List<string> _baseTypes = [];
    private readonly List<BracketSourceTextBuilder> _members = [];

    public string DeclarationLine { get; } = declarationLine;

    public TypeDeclarationSourceTextBuilder AddAttribute(string attribute)
    {
        _attributes.Add(attribute);
        return this;
    }

    public TypeDeclarationSourceTextBuilder AddBaseTypes(params ReadOnlySpan<string> baseTypes)
    {
        foreach (var baseType in baseTypes)
        {
            _baseTypes.Add(baseType);
        }
        return this;
    }

    public TypeDeclarationSourceTextBuilder AddMethodDeclaration(string signature,
        Action<MethodDeclarationSourceTextBuilder> methodDeclarationBuilder)
    {
        var methodDeclaration = new MethodDeclarationSourceTextBuilder(Root, signature);
        methodDeclarationBuilder(methodDeclaration);
        _members.Add(methodDeclaration);
        return this;
    }

    public TypeDeclarationSourceTextBuilder AddRawMembers(params ReadOnlySpan<string> rawTexts)
    {
        foreach (var rawText in rawTexts)
        {
            var memberDeclaration = new RawSourceTextBuilder(Root)
            {
                RawText = rawText,
            };
            _members.Add(memberDeclaration);
        }
        return this;
    }

    public TypeDeclarationSourceTextBuilder AddRawMembers(IEnumerable<string> rawTexts)
    {
        foreach (var rawText in rawTexts)
        {
            var memberDeclaration = new RawSourceTextBuilder(Root)
            {
                RawText = rawText,
            };
            _members.Add(memberDeclaration);
        }
        return this;
    }

    public override void BuildInto(StringBuilder builder, int indentLevel)
    {
        foreach (var attribute in _attributes)
        {
            builder.AppendLineWithIndent(attribute, Indent, indentLevel);
        }
        builder.AppendWithIndent(DeclarationLine, Indent, indentLevel);
        if (_baseTypes.Count > 0)
        {
            for (var i = 0; i < _baseTypes.Count; i++)
            {
                builder.Append(i is 0 ? " : " : ", ");
                builder.Append(_baseTypes[i]);
            }
        }
        builder.AppendLine();

        using var _ = BracketScope.Begin(builder, Indent, indentLevel);
        BuildMembersInto(builder, indentLevel + 1, _members);
    }
}

public class MethodDeclarationSourceTextBuilder(SourceTextBuilder root, string signature) : BracketSourceTextBuilder(root)
{
    private readonly List<string> _attributes = [];
    private readonly List<List<RawSourceTextBuilder>> _statementGroups = [];

    public string Signature { get; } = signature;

    public MethodDeclarationSourceTextBuilder AddAttribute(string attribute)
    {
        _attributes.Add(attribute);
        return this;
    }

    public MethodDeclarationSourceTextBuilder AddRawStatements(params ReadOnlySpan<string> rawTexts)
    {
        var list = new List<RawSourceTextBuilder>(rawTexts.Length);
        for (var i = 0; i < rawTexts.Length; i++)
        {
            list.Add(new RawSourceTextBuilder(Root)
            {
                RawText = rawTexts[i],
            });
        }
        _statementGroups.Add(list);
        return this;
    }

    public MethodDeclarationSourceTextBuilder AddRawStatements(IEnumerable<string> rawTexts)
    {
        _statementGroups.Add(
        [
            ..rawTexts.Select(x => new RawSourceTextBuilder(Root)
            {
                RawText = x,
            }),
        ]);
        return this;
    }

    public override void BuildInto(StringBuilder builder, int indentLevel)
    {
        foreach (var attribute in _attributes)
        {
            builder.AppendLineWithIndent(attribute, Indent, indentLevel);
        }
        builder.AppendLineWithIndent(Signature, Indent, indentLevel);

        using var _ = BracketScope.Begin(builder, Indent, indentLevel);

        for (var groupIndex = 0; groupIndex < _statementGroups.Count; groupIndex++)
        {
            if (groupIndex > 0)
            {
                builder.AppendLine();
            }

            for (var index = 0; index < _statementGroups[groupIndex].Count; index++)
            {
                _statementGroups[groupIndex][index].BuildInto(builder, indentLevel + 1);
            }
        }
    }
}

public class RawSourceTextBuilder(SourceTextBuilder root) : BracketSourceTextBuilder(root)
{
    public required string RawText { get; init; }

    public override void BuildInto(StringBuilder builder, int indentLevel)
    {
        builder.AppendLineWithIndent(RawText, Indent, indentLevel);
    }
}

file class BracketScope : IDisposable
{
    private readonly StringBuilder _builder;
    private readonly string _indent;
    private readonly int _indentLevel;

    public BracketScope(StringBuilder builder, string indent, int indentLevel)
    {
        _builder = builder;
        _indent = indent;
        _indentLevel = indentLevel;
        for (var i = 0; i < _indentLevel; i++)
        {
            _builder.Append(_indent);
        }
        _builder.AppendLine("{");
    }

    public void Dispose()
    {
        for (var i = 0; i < _indentLevel; i++)
        {
            _builder.Append(_indent);
        }
        _builder.AppendLine("}");
    }

    public static IDisposable Begin(StringBuilder builder, string indent, int indentLevel)
    {
        return new BracketScope(builder, indent, indentLevel);
    }
}

file class EmptyScope : IDisposable
{
    public void Dispose()
    {
    }

    public static IDisposable Begin()
    {
        return new EmptyScope();
    }
}

file sealed class ActionDisposable(object holdInstance, Action disposeAction) : IDisposable
{
    /// <summary>
    /// 我们需要保留此字段以便在本实例不被 GC 时，holdInstance 实例一定不会被 GC。
    /// </summary>
    private readonly object _holdInstance = holdInstance;

    ~ActionDisposable()
    {
        Dispose();
    }

    public void Dispose()
    {
        // 如果出现了并发多次执行，那就多次执行吧，不影响的。
        _ = _holdInstance;
        disposeAction();
        GC.SuppressFinalize(this);
    }
}

public static class SourceTextBuilderExtensions
{
    private static readonly WeakAsyncLocalAccessor<SourceTextBuilder> SourceTextBuilderLocal = new();

    private static readonly HashSet<string> KeywordTypeNames =
    [
        "bool", "byte", "sbyte", "char", "decimal", "double", "float", "int", "uint", "long", "ulong", "nint", "nuint", "short", "ushort",
        "object", "string", "void",
    ];

    internal static IDisposable BeginBuild(SourceTextBuilder sourceTextBuilder)
    {
        SourceTextBuilderLocal.Value = sourceTextBuilder;
        return new ActionDisposable(sourceTextBuilder, () => SourceTextBuilderLocal.Value = null);
    }

    /// <summary>
    /// 如果当前正在生成的源代码允许通过 using 引用命名空间，则：<br/>
    /// 提取 <paramref name="typeSymbol"/> 的命名空间，将其引用添加到当前正在生成的源代码的 using 列表中，
    /// 并返回简化后的类型名称字符串。<br/>
    /// 如果当前正在生成的源代码不允许通过 using 引用命名空间，则直接返回 <paramref name="typeSymbol"/> 的完整类型名称字符串。
    /// </summary>
    /// <param name="typeSymbol">要简化名称的类型符号。</param>
    /// <returns>简化后的类型名称字符串，或完整类型名称字符串。</returns>
    public static string ToUsingString(this ITypeSymbol typeSymbol)
    {
        var root = SourceTextBuilderLocal.Value;
        if (root is null)
        {
            // 当前没有正在生成的源代码，无法通过 using 引用命名空间。
            return typeSymbol.ToGlobalDisplayString();
        }
        if (!root.SimplifyTypeNamesByUsingNamespace)
        {
            // 当前正在生成的源代码不允许通过 using 引用命名空间。
            return root.ShouldPrependGlobal
                ? typeSymbol.ToGlobalDisplayString()
                : typeSymbol.ToDisplayString();
        }

        var namespaces = new List<string>();
        var simplifiedName = SimplifyNameByAddUsing(typeSymbol, namespaces);
        foreach (var @namespace in namespaces)
        {
            root.Using(@namespace);
        }
        return simplifiedName;
    }

    private static string SimplifyNameByAddUsing(ITypeSymbol typeSymbol, List<string> namespaces)
    {
        if (typeSymbol.Kind is SymbolKind.ArrayType)
        {
            // 数组类型（如 int[]、string[,] 等）
            var arrayType = (IArrayTypeSymbol)typeSymbol;
            return $"{SimplifyNameByAddUsing(arrayType.ElementType, namespaces)}[{new string(',', arrayType.Rank - 1)}]";
        }

        var originalDefinitionString = typeSymbol.OriginalDefinition.ToString();
        if (KeywordTypeNames.Contains(originalDefinitionString))
        {
            // 关键字类型（如 int、string 等）
            return originalDefinitionString;
        }
        if (originalDefinitionString.Equals("System.Nullable<T>", StringComparison.Ordinal))
        {
            // Nullable<T> 类型
            var genericType = ((INamedTypeSymbol)typeSymbol).TypeArguments[0];
            return $"{SimplifyNameByAddUsing(genericType, namespaces)}?";
        }
        if (originalDefinitionString.Equals("System.IntPtr", StringComparison.Ordinal))
        {
            // nint 类型
            return "nint";
        }
        if (originalDefinitionString.Equals("System.UIntPtr", StringComparison.Ordinal))
        {
            // nuint 类型
            return "nuint";
        }
        if (typeSymbol is INamedTypeSymbol { IsTupleType: true } valueTupleTypeSymbol)
        {
            // ValueTuple<T1, T2, ...> 类型
            var tupleMembers = string.Join(", ", valueTupleTypeSymbol.TupleElements
                .Select(x => $"{SimplifyNameByAddUsing(x.Type, namespaces)} {x.Name}"));
            return $"({tupleMembers})";
        }

        // 常规类型（或常规泛型类型）
        if (typeSymbol.ContainingNamespace is { IsGlobalNamespace: false } @namespace)
        {
            namespaces.Add(@namespace.ToDisplayString());
        }
        var recursiveTypeName = GetNestedTypeNameRecursively(typeSymbol);
        var nullablePostfix = typeSymbol.NullableAnnotation is NullableAnnotation.Annotated ? "?" : "";
        if (typeSymbol is INamedTypeSymbol { TypeArguments.Length: > 0 } namedTypeSymbol)
        {
            // Class<T> 类型
            var genericTypes = string.Join(", ", namedTypeSymbol.TypeArguments.Select(x => SimplifyNameByAddUsing(x, namespaces)));
            return $"{recursiveTypeName}<{genericTypes}>{nullablePostfix}";
        }
        else
        {
            if (typeSymbol is not INamedTypeSymbol)
            {
                throw new NotSupportedException($"目前尚未支持 {typeSymbol.GetType().FullName} 类型的名称简化。");
            }

            // T 类型
            return $"{recursiveTypeName}{nullablePostfix}";
        }

        // 返回一个类型的嵌套内部类名称。
        // 无视特殊类型（如 Nullable、ValueTuple 等），因此请勿对特殊类型调用此方法。
        static string GetNestedTypeNameRecursively(ITypeSymbol typeSymbol)
        {
            if (typeSymbol.ContainingType is { } containingType)
            {
                return $"{GetNestedTypeNameRecursively(containingType)}.{typeSymbol.Name}";
            }
            return typeSymbol.Name;
        }
    }
}

file static class Extensions
{
    internal static string PrependGlobal(this string name, bool? shouldPrependGlobal = true) => shouldPrependGlobal switch
    {
        // 一定加上 global:: 前缀
        true => name.StartsWith("global::", StringComparison.Ordinal) ? name : $"global::{name}",
        // 一定去掉 global:: 前缀
        false => name.StartsWith("global::", StringComparison.Ordinal) ? name[8..] : name,
        // 保持不变
        null => name,
    };

    internal static StringBuilder AppendIndent(this StringBuilder builder, string indent, int indentLevel)
    {
        for (var indentIndex = 0; indentIndex < indentLevel; indentIndex++)
        {
            builder.Append(indent);
        }
        return builder;
    }

    internal static StringBuilder AppendWithIndent(this StringBuilder builder, string text, string indent, int indentLevel)
    {
        var currentLineStart = 0;
        for (var index = 0; index < text.Length; index++)
        {
            if (text[index] != '\n' && index != text.Length - 1)
            {
                continue;
            }

            builder
                .AppendIndent(indent, indentLevel)
                .Append(text, currentLineStart, index - currentLineStart + 1);
            currentLineStart = index + 1;
        }
        return builder;
    }

    internal static StringBuilder AppendLineWithIndent(this StringBuilder builder, string text, string indent, int indentLevel)
    {
        return AppendWithIndent(builder, text, indent, indentLevel).AppendLine();
    }
}
