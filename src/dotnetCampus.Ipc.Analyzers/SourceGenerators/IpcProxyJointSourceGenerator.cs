using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using dotnetCampus.Ipc.Analyzers.SourceGenerators.Utils;
using dotnetCampus.Ipc.CompilerServices.Attributes;

namespace dotnetCampus.Ipc.Analyzers.SourceGenerators;

/// <summary>
/// 为 IPC 对象生成对应的代理（Proxy）和对接（Joint）。
/// </summary>
[Generator]
public class IpcProxyJointSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        foreach (var ipcObjectType in FindIpcPublicObjects(context.Compilation))
        {
            var contractType = ipcObjectType.ContractType;
            var proxySource = GenerateProxySource(ipcObjectType);
            var jointSource = GenerateJointSource(ipcObjectType);
            var assemblySource = GenerateAssemblySource(ipcObjectType);
            context.AddSource($"{contractType}.proxy", SourceText.From(proxySource, Encoding.UTF8));
            context.AddSource($"{contractType}.joint", SourceText.From(jointSource, Encoding.UTF8));
            context.AddSource($"{contractType}.assembly", SourceText.From(assemblySource, Encoding.UTF8));
        }
    }

    /// <summary>
    /// 生成代理类。
    /// </summary>
    /// <param name="realTypeCompilation">真实对象的编译信息。</param>
    /// <returns>代理类的源代码。</returns>
    private string GenerateProxySource(PublicIpcObjectCompilation realTypeCompilation)
    {
        var (contractTypeName, ipcTypeName) = GetIpcContractTypeNames(realTypeCompilation.ContractType);
        var members = string.Join(
            Environment.NewLine + Environment.NewLine,
            realTypeCompilation.EnumerateMembersByContractType()
            .Select(x => x.GenerateProxyMember()));
        var sourceCode = FormatCode(@$"{realTypeCompilation.GetUsing()}
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

namespace {realTypeCompilation.GetNamespace()}
{{
    internal class {ipcTypeName}IpcProxy : GeneratedIpcProxy<{contractTypeName}>, {contractTypeName}
    {{
{members}
    }}
}}
");
        return sourceCode;
    }

    /// <summary>
    /// 生成对接类。
    /// </summary>
    /// <param name="realTypeCompilation">真实对象的编译信息。</param>
    /// <returns>对接类的源代码。</returns>
    private string GenerateJointSource(PublicIpcObjectCompilation realTypeCompilation)
    {
        const string realInstanceName = "real";
        var (contractTypeName, ipcTypeName) = GetIpcContractTypeNames(realTypeCompilation.ContractType);
        var matches = string.Join(
            Environment.NewLine,
            realTypeCompilation.EnumerateMembersByContractType()
            .Select(x => x.GenerateJointMatch(realInstanceName)));
        var sourceCode = FormatCode(@$"{realTypeCompilation.GetUsing()}
using dotnetCampus.Ipc.CompilerServices.GeneratedProxies;

namespace {realTypeCompilation.GetNamespace()}
{{
    internal class {ipcTypeName}IpcJoint : GeneratedIpcJoint<{contractTypeName}>
    {{
        protected override void MatchMembers({contractTypeName} {realInstanceName})
        {{
{matches}
        }}
    }}
}}
");
        return sourceCode;
    }

    /// <summary>
    /// 生成代理对接关系信息。
    /// </summary>
    /// <param name="realTypeCompilation">真实对象的编译信息。</param>
    /// <returns>程序集特性的源代码。</returns>
    private string GenerateAssemblySource(PublicIpcObjectCompilation realTypeCompilation)
    {
        var (_, ipcTypeName) = GetIpcContractTypeNames(realTypeCompilation.ContractType);
        var sourceCode = @$"using dotnetCampus.Ipc.CompilerServices.Attributes;
using {realTypeCompilation.GetNamespace()};

[assembly: {GetAttributeName(typeof(AssemblyIpcProxyJointAttribute))}(typeof({realTypeCompilation.ContractType}), typeof({ipcTypeName}IpcProxy), typeof({ipcTypeName}IpcJoint))]";
        return sourceCode;
    }

    /// <summary>
    /// 在整个项目的编译信息中寻找 IPC 真实对象的编译信息。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <returns>所有 IPC 真实对象的编译信息</returns>
    private IEnumerable<PublicIpcObjectCompilation> FindIpcPublicObjects(Compilation compilation)
    {
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            if (PublicIpcObjectCompilation.TryFind(compilation, syntaxTree, out var publicIpcObjectCompilations))
            {
                foreach (var publicIpcObject in publicIpcObjectCompilations)
                {
                    yield return publicIpcObject;
                }
            }
        }
    }

    /// <summary>
    /// 根据契约类型生成文件名的一部分。
    /// </summary>
    /// <param name="contractType">契约类型。</param>
    /// <returns>（随便取的）契约类型名和 IPC 类型名。</returns>
    private (string contractTypeName, string ipcTypeName) GetIpcContractTypeNames(ITypeSymbol contractType)
    {
        var contractTypeName = contractType.Name;
        var typeName = contractTypeName.StartsWith("I")
            ? contractTypeName.Substring(1)
            : contractTypeName;
        return (contractTypeName, typeName);
    }

    /// <summary>
    /// 格式化代码。
    /// </summary>
    /// <param name="sourceCode">未格式化的源代码。</param>
    /// <returns>格式化的源代码。</returns>
    private string FormatCode(string sourceCode)
    {
        return CSharpSyntaxTree.ParseText(sourceCode).GetRoot().NormalizeWhitespace().SyntaxTree.GetText().ToString();
    }

    /// <summary>
    /// 获取一个类型作为 Attribute 编写时的编写名称，即去掉末尾的 Attribute 字符串。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <returns>去掉末尾 Attribute 后的字符串。</returns>
    private string GetAttributeName(Type type)
    {
        const string attributePostfix = "Attribute";
        var typeName = type.FullName;
        if (typeName.EndsWith(attributePostfix))
        {
            return typeName.Substring(0, typeName.Length - attributePostfix.Length);
        }
        return typeName;
    }
}
