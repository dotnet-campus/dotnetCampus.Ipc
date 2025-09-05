using System.Reflection;
using dotnetCampus.Ipc.Generators.Builders;

namespace dotnetCampus.Ipc;

internal static class GeneratorInfo
{
    public static string RootNamespace => typeof(GeneratorInfo).Namespace!;

    public static string ToolName { get; } = typeof(GeneratorInfo).Assembly
        .GetCustomAttribute<AssemblyTitleAttribute>()?.Title ?? typeof(GeneratorInfo).Namespace!;

    public static string ToolVersion { get; } = typeof(GeneratorInfo).Assembly
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "0.0.0";

    private static readonly SymbolDisplayFormat GlobalDisplayFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    private static readonly SymbolDisplayFormat NotNullGlobalDisplayFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.IncludeTypeParameters,
        miscellaneousOptions:
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    private static readonly SymbolDisplayFormat GlobalTypeOfDisplayFormat = new SymbolDisplayFormat(
        globalNamespaceStyle: SymbolDisplayGlobalNamespaceStyle.Included,
        typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces,
        genericsOptions: SymbolDisplayGenericsOptions.None,
        miscellaneousOptions:
        SymbolDisplayMiscellaneousOptions.EscapeKeywordIdentifiers |
        SymbolDisplayMiscellaneousOptions.IncludeNullableReferenceTypeModifier |
        SymbolDisplayMiscellaneousOptions.UseSpecialTypes);

    public static string ToGlobalDisplayString(this ISymbol symbol)
    {
        return symbol.ToDisplayString(GlobalDisplayFormat);
    }

    public static string ToNotNullGlobalDisplayString(this ISymbol symbol)
    {
        // 对于 Nullable<T>（例如 Nullable<int>、int?）等，是类型而不是可空标记，所以需要特别取出里面的类型 T。
        if (symbol is ITypeSymbol { IsValueType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T } typeSymbol)
        {
            return typeSymbol is INamedTypeSymbol { IsGenericType: true, ConstructedFrom.SpecialType: SpecialType.System_Nullable_T } namedType
                // 获取 Nullable<T> 中的 T。
                ? namedType.TypeArguments[0].ToDisplayString(GlobalDisplayFormat)
                // 处理直接带有可空标记的类型 (int? 这种形式)。
                : typeSymbol.WithNullableAnnotation(NullableAnnotation.None).ToDisplayString(GlobalDisplayFormat);
        }

        // 对于其他符号或非可空类型，使用不包含可空引用类型修饰符的格式
        return symbol.ToDisplayString(NotNullGlobalDisplayFormat);
    }

    public static string ToGlobalTypeOfDisplayString(this INamedTypeSymbol symbol)
    {
        var name = symbol.ToDisplayString(GlobalTypeOfDisplayFormat);
        return symbol.IsGenericType ? $"{name}<{new string(',', symbol.TypeArguments.Length - 1)}>" : name;
    }

    /// <summary>
    /// 为生成的代码添加生成工具信息。
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static TypeDeclarationSourceTextBuilder AddGeneratedToolAndEditorBrowsingAttributes(this TypeDeclarationSourceTextBuilder builder)
    {
        return builder
            .AddAttribute("[global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]")
            .AddAttribute($"""[global::System.CodeDom.Compiler.GeneratedCode("{ToolName}", "{ToolVersion}")]""");
    }
}
