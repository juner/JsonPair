namespace JsonPair.Extensions.Tests;

[TestClass()]
public class StringCaseExtensionsTests
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expected"></param>
    [TestMethod("スネークケース変換")]
    [DataRow("CamelCase", "camel_case")]
    [DataRow("pascalCase", "pascal_case")]
    [DataRow("ACamelCase", "a_camel_case")]
    public void ToSnakeCaseTest(string value, string expected)
        => Assert.AreEqual(expected, value.ToSnakeCase());
}
