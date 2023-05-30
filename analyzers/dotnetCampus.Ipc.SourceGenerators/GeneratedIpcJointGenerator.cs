using System.Text.RegularExpressions;

using Microsoft.CodeAnalysis.Text;

namespace dotnetCampus.Ipc;

/// <summary>
/// 为 GeneratedIpcJoint 类生成更多的泛型参数。
/// </summary>
[Generator(LanguageNames.CSharp)]
public class GeneratedIpcJointGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var compilations = context.SyntaxProvider.CreateSyntaxProvider(
            // 找到 GeneratedIpcJoint 类型。
            (syntaxNode, ct) => syntaxNode is ClassDeclarationSyntax cds && cds.Identifier.ToString() == "GeneratedIpcJoint",
            // 语义解析：确定是否真的是感兴趣的 IPC 接口。
            (generatorSyntaxContext, ct) => generatorSyntaxContext)
            .Where(x =>
            {
                var node = x.Node as ClassDeclarationSyntax;
                var symbol = x.SemanticModel.GetDeclaredSymbol(x.Node) as INamedTypeSymbol;
                return node is not null && symbol is not null && symbol.IsGenericType;
            });

        context.RegisterSourceOutput(compilations, Execute);
    }

    private void Execute(SourceProductionContext context, GeneratorSyntaxContext generatorContext)
    {
        var source = GenerateGeneratedIpcJoint(generatorContext);
        context.AddSource($"GeneratedIpcJoint.generic", SourceText.From(source, Encoding.UTF8));
    }

    private string GenerateGeneratedIpcJoint(GeneratorSyntaxContext context) => $$"""
        using System;
        using System.Collections.Generic;
        using System.Linq;
        using System.Threading.Tasks;

        using dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

        namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies;
        partial class GeneratedIpcJoint<TContract>
        {
        {{string.Join("\r\n\r\n",
            Enumerable.Range(2, 15)
                .Select(x => GenerateMethodGroup(context, x))
                .SelectMany(x => x))}}
        }
        """;

    private IEnumerable<string> GenerateMethodGroup(GeneratorSyntaxContext context, int genericCount)
    {
        yield return GenerateVoidMethod(context, genericCount);
        yield return GenerateReturnMethod(context, genericCount);
        yield return GenerateAsyncVoidMethod(context, genericCount);
        yield return GenerateAsyncReturnMethod(context, genericCount);
    }

    private string GenerateVoidMethod(GeneratorSyntaxContext context, int genericCount) => $$"""
        /// <summary>
        /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
        /// </summary>
        /// <param name="memberId">方法签名的 Id。</param>
        /// <param name="methodInvoker">对接实现。</param>
        protected void MatchMethod<{{GenerateTs(genericCount)}}>(ulong memberId, Action<{{GenerateTs(genericCount)}}> methodInvoker)
        {
            _methods.Add(memberId, (new[] { {{GenerateTs(genericCount, "typeof(T)")}} }, args =>
            {
                methodInvoker({{GenerateTs(genericCount, i => $"CastArg<T>(args![{i-1}])!")}});
                return DefaultGarm;
            }
            ));
        }
    """;

    private string GenerateReturnMethod(GeneratorSyntaxContext context, int genericCount) => $$"""
        /// <summary>
        /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
        /// </summary>
        /// <param name="memberId">方法签名的 Id。</param>
        /// <param name="methodInvoker">对接实现。</param>
        protected void MatchMethod<{{GenerateTs(genericCount)}}>(ulong memberId, Func<{{GenerateTs(genericCount)}}, Task> methodInvoker)
        {
            _asyncMethods.Add(memberId, (new[] { {{GenerateTs(genericCount, "typeof(T)")}} }, async args =>
            {
                await methodInvoker({{GenerateTs(genericCount, i => $"CastArg<T>(args![{i - 1}])!")}}).ConfigureAwait(false);
                return DefaultGarm;
            }
            ));
        }
    """;

    private string GenerateAsyncVoidMethod(GeneratorSyntaxContext context, int genericCount) => $$"""
        /// <summary>
        /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
        /// </summary>
        /// <param name="memberId">方法签名的 Id。</param>
        /// <param name="methodInvoker">对接实现。</param>
        protected void MatchMethod<{{GenerateTs(genericCount)}}, TReturn>(ulong memberId, Func<{{GenerateTs(genericCount)}}, Garm<TReturn>> methodInvoker)
        {
            _methods.Add(memberId, (new[] { {{GenerateTs(genericCount, "typeof(T)")}} }, args => methodInvoker({{GenerateTs(genericCount, i => $"CastArg<T>(args![{i - 1}])!")}})));
        }
    """;

    private string GenerateAsyncReturnMethod(GeneratorSyntaxContext context, int genericCount) => $$"""
        /// <summary>
        /// 匹配一个 IPC 目标对象上的某个方法，使其他 IPC 节点访问此 IPC 对象时能执行 <paramref name="methodInvoker"/> 所指向的具体实现。
        /// </summary>
        /// <param name="memberId">方法签名的 Id。</param>
        /// <param name="methodInvoker">对接实现。</param>
        protected void MatchMethod<{{GenerateTs(genericCount)}}, TReturn>(ulong memberId, Func<{{GenerateTs(genericCount)}}, Task<Garm<TReturn>>> methodInvoker)
        {
            _asyncMethods.Add(memberId, (new[] { {{GenerateTs(genericCount, "typeof(T)")}} }, async args => await methodInvoker({{GenerateTs(genericCount, i => $"CastArg<T>(args![{i - 1}])!")}}).ConfigureAwait(false)));
        }
    """;

    private string GenerateTs(int genericCount, string? template = null)
    {
        var templateWithDefault = template ?? "T";
        return string.Join(", ",
            Enumerable.Range(1, genericCount)
                .Select(x => GenrateT(x, templateWithDefault)));
    }

    private string GenerateTs(int genericCount, Func<int, string> templateGetter)
    {
        return string.Join(", ",
            Enumerable.Range(1, genericCount)
                .Select(x => GenrateT(x, templateGetter(x))));
    }

    /// <summary>
    /// 使用正则表达式找到 template 中独立的 T，然后替换为 T1、T2、T3……
    /// </summary>
    /// <param name="genericIndex">泛型序号，从 1 开始。</param>
    /// <param name="template">泛型格式化模板，默认为 T；也可以用别的，如 typeof(T)。</param>
    private string GenrateT(int genericIndex, string template)
    {
        var regex = new Regex(@"\bT\b");
        var match = regex.Match(template);
        if (match.Success)
        {
            var index = match.Index;
            var length = match.Length;
            var prefix = template.Substring(0, index);
            var suffix = template.Substring(index + length);
            return prefix + $"T{genericIndex}" + suffix;
        }
        else
        {
            return template;
        }
    }
}
