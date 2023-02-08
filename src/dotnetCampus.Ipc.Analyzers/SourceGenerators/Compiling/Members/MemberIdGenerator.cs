using System.Security.Cryptography;

namespace dotnetCampus.Ipc.SourceGenerators.Compiling.Members;
public static class MemberIdGenerator
{
    public static string GeneratePropertyId(string getSet, string propertyName)
        => CalculateHash($"{getSet}_{propertyName}()");

    public static string GenerateMethodId(IMethodSymbol method)
        => GenerateMethodId(method.Name, method.Parameters.Select(x => x.Type.ToString()));

    public static string GenerateMethodId(string methodName, IEnumerable<string> parameterTypeNames)
        => CalculateHash($"{methodName}({string.Join(",", parameterTypeNames)})");

    private static string CalculateHash(string text)
    {
        using var sha256 = SHA256.Create();

        var inputBytes = Encoding.UTF8.GetBytes(text);
        var hashBytes = sha256.ComputeHash(inputBytes);

        return BitConverter.ToString(hashBytes)
            .Replace("-", "")
            .Substring(0, 16)
            .ToLower(CultureInfo.InvariantCulture);
    }
}
