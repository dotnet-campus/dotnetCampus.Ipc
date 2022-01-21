namespace dotnetCampus.Ipc.SourceGenerators.Models;

internal class MemberDeclarationSourceTextBuilder
{
    private readonly SourceTextBuilder _root;
    private readonly string _sourceCode;
    private readonly List<string> _expressions = new();

    public MemberDeclarationSourceTextBuilder(SourceTextBuilder builder, string sourceCode)
    {
        _root = builder ?? throw new ArgumentNullException(nameof(builder));
        _sourceCode = sourceCode ?? "";
    }

    public MemberDeclarationSourceTextBuilder AddExpressions(Func<SourceTextBuilder, IEnumerable<string>> expressions)
    {
        foreach (var expression in expressions(_root))
        {
            _expressions.Add(expression);
        }

        return this;
    }

    public override string ToString()
    {
        if (_sourceCode.Contains("{"))
        {
            return _sourceCode;
        }
        else
        {
            return $@"{_sourceCode}
{{
    {string.Join(Environment.NewLine, _expressions)}
}}";
        }
    }
}
