using System.Text.Json.Serialization;

namespace dotnetCampus.Ipc.Tests;

#if NET6_0_OR_GREATER

[JsonSerializable(typeof(bool))]
internal partial class TestJsonContext : JsonSerializerContext;
#endif
