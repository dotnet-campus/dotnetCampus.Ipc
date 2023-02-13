namespace dotnetCampus.Ipc.SourceGenerators.Models;

internal class ClassDeclarationSourceTextBuilder
{
    private readonly SourceTextBuilder _root;
    private readonly string _typeName;
    private readonly List<string> _attributeList = new();
    private readonly List<string> _baseTypeNames = new();
    private readonly List<MemberDeclarationSourceTextBuilder> _memberBuilders = new();

    public ClassDeclarationSourceTextBuilder(SourceTextBuilder root, string className, params string[] baseTypeList)
    {
        if (string.IsNullOrWhiteSpace(className))
        {
            throw new ArgumentException($"“{nameof(className)}”不能为 null 或空白。", nameof(className));
        }
        _root = root ?? throw new ArgumentNullException(nameof(root));
        _typeName = className;
        _baseTypeNames = baseTypeList.ToList();
    }

    internal ClassDeclarationSourceTextBuilder WithAttribute(string attribute)
    {
        if (string.IsNullOrWhiteSpace(attribute))
        {
            throw new ArgumentException($"“{nameof(attribute)}”不能为 null 或空白。", nameof(attribute));
        }

        _attributeList.Add(attribute);
        return this;
    }

    internal ClassDeclarationSourceTextBuilder AddMemberDeclaration(Func<SourceTextBuilder, MemberDeclarationSourceTextBuilder> memberBuilder)
    {
        if (memberBuilder is null)
        {
            throw new ArgumentNullException(nameof(memberBuilder));
        }

        _memberBuilders.Add(memberBuilder(_root));
        return this;
    }

    internal ClassDeclarationSourceTextBuilder AddMemberDeclarations(Func<SourceTextBuilder, IEnumerable<MemberDeclarationSourceTextBuilder>> memberBuilders)
    {
        if (memberBuilders is null)
        {
            throw new ArgumentNullException(nameof(memberBuilders));
        }

        _memberBuilders.AddRange(memberBuilders(_root));
        return this;
    }

    public override string ToString()
    {
        return $@"{string.Join("\r\n", _attributeList)}
internal class {_typeName} {(_baseTypeNames.Count > 0 ? ":" : "")} {string.Join(", ", _baseTypeNames)}
{{
    {string.Join(
        "\r\n",
        _memberBuilders.Select(x => x.ToString()))}
}}
        ";
    }
}
