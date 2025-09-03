using System.Reflection;
using System.Text.Json.Serialization;
using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Tests.CompilerServices;
using dotnetCampus.Ipc.Tests.IpcRouteds.DirectRouteds;

namespace dotnetCampus.Ipc.Tests;

#if NET8_0_OR_GREATER

[JsonSerializable(typeof(BindingFlags))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(IList<string>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(FakeIpcObjectSubModelA))]
[JsonSerializable(typeof(IFakeIpcObject.NestedEnum))]
[JsonSerializable(typeof((double a, uint b, int? c, byte d)))]
[JsonSerializable(typeof(JsonIpcDirectRoutedProviderTest.FakeArgument))]
[JsonSerializable(typeof(JsonIpcDirectRoutedProviderTest.FakeResult))]
[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = false,
    IncludeFields = true,
    UseStringEnumConverter = true)]
internal partial class TestJsonContext : JsonSerializerContext
{
    public static IpcConfiguration CreateIpcConfiguration() => new IpcConfiguration()
        .UseSystemTextJsonIpcObjectSerializer(Default);
}

#else

internal static class TestJsonContext
{
    public static IpcConfiguration CreateIpcConfiguration() => new IpcConfiguration()
        .UseNewtonsoftJsonIpcObjectSerializer(null);
}

#endif

internal static class IpcConfigurationExtensions
{
    public static IpcConfiguration UseTestFrameworkJsonSerializer(this IpcConfiguration ipcConfiguration)
    {
#if NET8_0_OR_GREATER
        // 在 .NET 8.0 及更高版本中，测试 System.Text.Json 作为底层 IPC 传输机制。
        return ipcConfiguration.UseSystemTextJsonIpcObjectSerializer(TestJsonContext.Default);
#else
        // 在旧版本的 .NET 中，测试 Newtonsoft.Json 作为底层 IPC 传输机制。
        return ipcConfiguration.UseNewtonsoftJsonIpcObjectSerializer(null);
#endif
    }
}
