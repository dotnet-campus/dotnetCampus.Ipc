using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace dotnetCampus.Ipc.Utils
{
    internal static class ExceptionHacker
    {
        internal static Exception ReplaceStackTrace(Exception target, string stack) => HackExceptionStackTrace(target, stack);

        private static readonly Func<Exception, string, Exception> HackExceptionStackTrace = new Func<Func<Exception, string, Exception>>(() =>
        {
            var target = Expression.Parameter(typeof(Exception));
            var stack = Expression.Parameter(typeof(string));
            var traceFormatType = typeof(StackTrace).GetNestedType("TraceFormat", BindingFlags.NonPublic)!;
            var toString = typeof(StackTrace).GetMethod("ToString", BindingFlags.NonPublic | BindingFlags.Instance, null, new[] { traceFormatType }, null)!;
            var normalTraceFormat = Enum.GetValues(traceFormatType).GetValue(0)!;
            var stackTraceStringField = typeof(Exception).GetField("_stackTraceString", BindingFlags.NonPublic | BindingFlags.Instance)!;
            var assign = Expression.Assign(Expression.Field(target, stackTraceStringField), stack);
            return Expression.Lambda<Func<Exception, string, Exception>>(Expression.Block(assign, target), target, stack).Compile();
        })();
    }
}
