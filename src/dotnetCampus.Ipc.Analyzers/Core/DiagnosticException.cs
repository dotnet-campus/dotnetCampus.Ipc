using Microsoft.CodeAnalysis;

namespace dotnetCampus.Ipc.Analyzers.Core;

/// <summary>
/// 当出现错误时，通过抛出此异常来报告编译错误。
/// </summary>
internal class DiagnosticException : Exception
{
    private readonly object?[]? _messageArgs;

    public DiagnosticException(DiagnosticDescriptor diagnostic, Location? location, params object?[]? messageArgs)
    {
        Diagnostic = diagnostic;
        Location = location;
        _messageArgs = messageArgs;
    }

    public DiagnosticDescriptor Diagnostic { get; }

    public Location? Location { get; }

    public Diagnostic ToDiagnostic()
    {
        return Microsoft.CodeAnalysis.Diagnostic.Create(Diagnostic, Location, _messageArgs);
    }
}
