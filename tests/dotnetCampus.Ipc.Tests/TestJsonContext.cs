using System.Reflection;
using System.Text.Json.Serialization;
using dotnetCampus.Ipc.Tests.CompilerServices;

namespace dotnetCampus.Ipc.Tests;

#if NET6_0_OR_GREATER

[JsonSerializable(typeof(BindingFlags))]
[JsonSerializable(typeof(List<string>))]
[JsonSerializable(typeof(string[]))]
[JsonSerializable(typeof(FakeIpcObjectSubModelA))]
[JsonSerializable(typeof(IFakeIpcObject.NestedEnum))]
[JsonSerializable(typeof((double a, uint b, int? c, byte d)))]
internal partial class TestJsonContext : JsonSerializerContext;
#endif
