namespace dotnetCampus.Ipc.CodeAnalysis.Utils;

internal static class AttributeSyntaxHelper
{
    /// <summary>
    /// 获取一个类型作为 Attribute 编写时的编写名称，即去掉末尾的 Attribute 字符串。
    /// </summary>
    /// <param name="typeName">类型名称。</param>
    /// <returns>去掉末尾 Attribute 后的字符串。</returns>
    public static string GetAttributeName(string typeName)
    {
        const string attributePostfix = "Attribute";
        if (typeName.EndsWith(attributePostfix))
        {
            return typeName.Substring(0, typeName.Length - attributePostfix.Length);
        }
        return typeName;
    }
}
