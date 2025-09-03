using System.Text.Json.Serialization;

using dotnetCampus.Ipc.Context;
using dotnetCampus.Ipc.Exceptions;
using dotnetCampus.Ipc.IpcRouteds.DirectRouteds;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace dotnetCampus.Ipc.Tests.IpcRouteds.DirectRouteds;

[TestClass]
public class JsonIpcDirectRoutedProviderSystemJsonSerializerTest
{
    [TestMethod("测试直接路由匹配到不符合预期的响应时的异常")]
    public async Task TestRequestNotMatchResponse()
    {
        var name = "JsonIpcDirectRoutedProviderSystemJsonSerializerTest_1";
        var serverProvider = new JsonIpcDirectRoutedProvider(name, new IpcConfiguration().UseSystemTextJsonIpcObjectSerializer(JsonIpcDirectRoutedSystemJsonSerializerTestGenerationContext.Default));
        var requestPath = "RequestPath1";
        var requestValue = "请求的内容";
        serverProvider.AddRequestHandler(requestPath, (JsonIpcDirectRoutedSystemJsonSerializerTestRequest1 request) =>
        {
            Assert.AreEqual(requestValue, request.Value);
            // 特意返回不符合预期的内容，预期是 JsonIpcDirectRoutedSystemJsonSerializerTestResponse1 类型，实际返回是字符串
            return "错误的返回内容";
        });
        serverProvider.StartServer();

        var clientProvider = new JsonIpcDirectRoutedProvider(ipcConfiguration: new IpcConfiguration().UseSystemTextJsonIpcObjectSerializer(JsonIpcDirectRoutedSystemJsonSerializerTestGenerationContext.Default));

        JsonIpcDirectRoutedClientProxy jsonIpcDirectRoutedClientProxy = await clientProvider.GetAndConnectClientAsync(name);

        try
        {
            var request = new JsonIpcDirectRoutedSystemJsonSerializerTestRequest1() { Value = requestValue };
            var response =
                await jsonIpcDirectRoutedClientProxy
                    .GetResponseAsync<JsonIpcDirectRoutedSystemJsonSerializerTestResponse1>(requestPath,
                        request);
            _ = response; // 只是让分析器开森而已
        }
        catch (Exception e)
        {
            var jsonIpcDirectRouteSerializeLocalException = e as JsonIpcDirectRouteSerializeLocalException;
            Assert.IsNotNull(jsonIpcDirectRouteSerializeLocalException);
            // Json Ipc DirectRoute Serialize Exception. ResponseType=dotnetCampus.Ipc.Tests.IpcRouteds.DirectRouteds.JsonIpcDirectRoutedSystemJsonSerializerTestResponse1 ResponseMessage=[IpcMessage] Header=0;Tag=[JsonIpcDirectRoutedProviderSystemJsonSerializerTest_1];Body=[22 5C 75 39 35 31 39 5C 75 38 42 45 46 5C 75 37 36 38 34 5C 75 38 46 44 34 5C 75 35 36 44 45 5C 75 35 31 38 35 5C 75 35 42 42 39 22](GuessText="\u9519\u8BEF\u7684\u8FD4\u56DE\u5185\u5BB9") The JSON value could not be converted to dotnetCampus.Ipc.Tests.IpcRouteds.DirectRouteds.JsonIpcDirectRoutedSystemJsonSerializerTestResponse1. Path: $ | LineNumber: 0 | BytePositionInLine: 44.
            Assert.AreEqual(typeof(JsonIpcDirectRoutedSystemJsonSerializerTestResponse1),
                jsonIpcDirectRouteSerializeLocalException.ResponseType);
            // 在此返回，证明进入了异常。不想用 Assert.ThrowsException 方法，因为这个方法不能进一步调试异常里面的信息
            return;
        }

        Assert.Fail("预期一定会抛出异常，在 catch 分支返回");
    }
}

[JsonSerializable(typeof(JsonIpcDirectRoutedSystemJsonSerializerTestRequest1))]
[JsonSerializable(typeof(JsonIpcDirectRoutedSystemJsonSerializerTestResponse1))]
internal partial class JsonIpcDirectRoutedSystemJsonSerializerTestGenerationContext : JsonSerializerContext
{
}

public record JsonIpcDirectRoutedSystemJsonSerializerTestRequest1
{
    public string? Value { get; set; }
}

public record JsonIpcDirectRoutedSystemJsonSerializerTestResponse1
{
    public string? Result { get; set; }
}
