using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics;
using System.Threading;
using dotnetCampus.Ipc.SourceGenerators.Compiling;

namespace dotnetCampus.Ipc;

/// <summary>
/// 为没有真实实现类型的 IPC 调用生成傀儡代理。
/// </summary>
[Generator]
internal class IpcShapeGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        //System.Diagnostics.Debugger.Launch();
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var ipcPublicObjectsInCurrentAssembly = FindIpcPublicObjects(context.Compilation).ToList();
        var ipcProxyUsages = FindIpcProxyUsages(context.Compilation, "CreateIpcProxy", context.CancellationToken).ToList();

        //ipcProxyUsages.Where(x=>x.)

    }

    /// <summary>
    /// 查找当前程序集里所有调用了名为 <paramref name="methodName"/> 方法的代码，并返回这个调用语法以及被调用的方法。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <param name="methodName">方法名。</param>
    /// <param name="cancellationToken">取消。</param>
    /// <returns>
    /// <list type="bullet">
    /// <item>accessSyntax: 调用此方法的语法节点。</item>
    /// <item>methodSymbol: 所调用的方法。</item>
    /// </list>
    /// </returns>
    public IEnumerable<IpcProxyInvokingInfo> FindIpcProxyUsages(
        Compilation compilation, string methodName, CancellationToken cancellationToken = default)
    {
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            var semanticModel = compilation.GetSemanticModel(syntaxTree);
            var infos = from node in syntaxTree.GetRoot().DescendantNodes()
                        where node.IsKind(SyntaxKind.InvocationExpression)
                        let invocation = (InvocationExpressionSyntax) node
                        where invocation.Expression is MemberAccessExpressionSyntax
                        let memberAccessNode = (MemberAccessExpressionSyntax) invocation.Expression
                        where memberAccessNode.Name.Identifier.ValueText.Contains(methodName)
                        let info = IpcProxyInvokingInfo.TryCreateIpcProxyInvokingInfo(semanticModel, memberAccessNode, cancellationToken)
                        where info is not null
                        select (IpcProxyInvokingInfo) info;
            foreach (var info in infos)
            {
                yield return info;
            }
        }
    }

    /// <summary>
    /// 在整个项目的编译信息中寻找 IPC 真实对象的编译信息。
    /// </summary>
    /// <param name="compilation">整个项目的编译信息。</param>
    /// <returns>所有 IPC 真实对象的编译信息</returns>
    private IEnumerable<IpcPublicCompilation> FindIpcPublicObjects(Compilation compilation)
    {
        foreach (var syntaxTree in compilation.SyntaxTrees)
        {
            if (IpcPublicCompilation.TryFind(compilation, syntaxTree, out var publicIpcObjectCompilations))
            {
                foreach (var publicIpcObject in publicIpcObjectCompilations)
                {
                    yield return publicIpcObject;
                }
            }
        }
    }
}
