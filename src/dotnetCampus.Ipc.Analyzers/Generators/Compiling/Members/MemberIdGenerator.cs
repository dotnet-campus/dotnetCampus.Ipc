using System.Security.Cryptography;

namespace dotnetCampus.Ipc.Generators.Compiling.Members;

/// <summary>
/// 为成员签名生成唯一 ID，用于 IPC 对象的跨进程通信和跨版本兼容。
/// </summary>
public static class MemberIdGenerator
{
    /// <summary>
    /// 生成属性的 get 或 set 方法的唯一 ID。
    /// </summary>
    /// <param name="getSet">只能传 get 或 set 这两种字符串。</param>
    /// <param name="propertyName">属性名称。</param>
    /// <returns>属性的唯一 ID。</returns>
    public static string GeneratePropertyId(string getSet, string propertyName)
        => CalculateHash($"{getSet}_{propertyName}()");

    /// <summary>
    /// 生成方法的唯一 ID。
    /// </summary>
    /// <param name="method">方法。</param>
    /// <returns>方法的唯一 ID。</returns>
    public static string GenerateMethodId(IMethodSymbol method)
        => GenerateMethodId(method.Name, method.Parameters.Select(x => x.Type.ToString()));

    /// <summary>
    /// 生成方法的唯一 ID。
    /// </summary>
    /// <param name="methodName">方法名称。</param>
    /// <param name="parameterTypeNames">参数类型名称集合。</param>
    /// <returns>方法的唯一 ID。</returns>
    public static string GenerateMethodId(string methodName, IEnumerable<string> parameterTypeNames)
        => CalculateHash($"{methodName}({string.Join(",", parameterTypeNames)})");

    /// <summary>
    /// 根据方法签名计算哈希值，作为唯一 ID。
    /// </summary>
    /// <param name="text">方法签名文本。</param>
    /// <returns>唯一 ID。</returns>
    private static string CalculateHash(string text)
    {
        // **请勿修改此方法的实现，否则会导致跨版本兼容性问题。**
        using var sha256 = SHA256.Create();

        // **请勿修改此方法的实现，否则会导致跨版本兼容性问题。**
        var inputBytes = Encoding.UTF8.GetBytes(text);
        var hashBytes = sha256.ComputeHash(inputBytes);

        // **请勿修改此方法的实现，否则会导致跨版本兼容性问题。**
        return $"0x{BitConverter.ToInt64(hashBytes, 0):X16}";
    }
}
