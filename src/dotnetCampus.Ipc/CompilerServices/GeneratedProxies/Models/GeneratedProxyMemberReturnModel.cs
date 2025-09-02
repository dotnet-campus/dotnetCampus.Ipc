using System.Runtime.Serialization;

#if UseNewtonsoftJson
using Newtonsoft.Json;
#endif
#if NET6_0_OR_GREATER
using System.Text.Json.Serialization;
#endif

namespace dotnetCampus.Ipc.CompilerServices.GeneratedProxies.Models;

[DataContract]
internal class GeneratedProxyMemberReturnModel
{
    public GeneratedProxyMemberReturnModel()
    {
    }

    public GeneratedProxyMemberReturnModel(Exception exception)
    {
        if (exception is null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        Exception = new GeneratedProxyExceptionModel(exception);
    }

    public GeneratedProxyMemberReturnModel(IpcJsonElement @return)
    {
        // 当返回对象为其他类型时，将尝试进行序列化。
        Return = new GeneratedProxyObjectModel { Value = @return };
    }

#if DEBUG
#if UseNewtonsoftJson
    [JsonProperty("i")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("i")]
#endif
    public GeneratedProxyMemberInvokeModel? Invoking { get; set; }
#endif

#if UseNewtonsoftJson
    [JsonProperty("r")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("r")]
#endif
    public GeneratedProxyObjectModel? Return { get; set; }

#if UseNewtonsoftJson
    [JsonProperty("e")]
#endif
#if NET6_0_OR_GREATER
    [JsonPropertyName("e")]
#endif
    public GeneratedProxyExceptionModel? Exception { get; set; }
}
