using dotnetCampus.Ipc.DiagnosticAnalyzers.Compiling;

using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

internal static class IpcSemanticSyntaxHelper
{
    /// <summary>
    /// 判断一个语义类型是否是 IPC 公开类型，如果是则返回其 IPC 特性语法。
    /// </summary>
    /// <param name="typeSymbol"></param>
    /// <param name="semanticModel"></param>
    /// <returns></returns>
    public static AttributeSyntax? TryGetClassDeclarationWithIpcAttribute(this INamedTypeSymbol typeSymbol, SemanticModel semanticModel)
    {
        if (typeSymbol.TryGetClassDeclaration() is { } classDeclarationSyntax)
        {
            var (attribute, _) = IpcAttributeHelper.TryFindClassAttributes(semanticModel, classDeclarationSyntax).FirstOrDefault();
            if (attribute is not null)
            {
                return attribute;
            }
        }
        return null;
    }
}
