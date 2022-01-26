using System.Threading.Tasks;

using dotnetCampus.Ipc.CodeAnalysis.Core;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using VerifyCS = dotnetCampus.Ipc.Analyzers.Tests.CSharpCodeFixVerifier<
    dotnetCampus.Ipc.DiagnosticAnalyzers.IgnoresIpcExceptionIsRecommendedAnalyzer,
    dotnetCampus.Ipc.CodeFixeProviders.IgnoresIpcExceptionIsRecommendedCodeFixProvider>;

namespace dotnetCampus.Ipc.Analyzers.Tests;

[TestClass]
public class IgnoresIpcExceptionAnalyzerTests
{
    //    [TestMethod]
    //    public async Task TestMethod1()
    //    {
    //        var test = @"";

    //        await VerifyCS.VerifyAnalyzerAsync(test);
    //    }

    //    [TestMethod]
    //    public async Task TestMethod2()
    //    {
    //        var test = @"
    //using System;
    //using System.Reflection;
    //using System.Threading;
    //using System.Threading.Tasks;

    //using dotnetCampus.Ipc.CompilerServices.Attributes;

    //namespace ConsoleApplication1
    //{
    //    [IpcPublic(typeof(IFakeIpcObject))]
    //    internal class FakeIpcObject : IFakeIpcObject
    //    {
    //    }
    //}";

    //        var fixtest = @"
    //using System;
    //using System.Reflection;
    //using System.Threading;
    //using System.Threading.Tasks;

    //using dotnetCampus.Ipc.CompilerServices.Attributes;

    //namespace ConsoleApplication1
    //{
    //    [IpcPublic(typeof(IFakeIpcObject), IgnoresIpcException = true)]
    //    internal class FakeIpcObject : IFakeIpcObject
    //    {
    //    }
    //}";

    //        var expected = VerifyCS.Diagnostic(Diagnostics.DIPC101_IgnoresIpcExceptionIsRecommended);
    //        await VerifyCS.VerifyCodeFixAsync(test, expected, fixtest);
    //    }
}
