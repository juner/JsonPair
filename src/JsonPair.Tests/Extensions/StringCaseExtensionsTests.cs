using JsonPair.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace JsonPair.Extensions.Tests;

[TestClass()]
public class StringCaseExtensionsTests
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expected"></param>
    [TestMethod("CamelCase -> snake_case")]
    [DataRow("CamelCase", "camel_case")]
    [DataRow("pascalCase", "pascal_case")]
    [DataRow("ACamelCase", "a_camel_case")]
    public void CamelCaseToSnakeCaseTest(string value, string expected)
        => Assert.AreEqual(expected, value.CamelCaseToSnakeCase());

    [TestMethod("snake_case -> CamelCase ")]
    [DataRow("snake_case", false, "snakeCase")]
    [DataRow("snake_case", true, "SnakeCase")]
    [DataRow("_snake_case", false, "SnakeCase")]
    [DataRow("SNAKE_CASE", false, "snakeCase")]
    public void SnakeCaseToCamelCaseTest(string value, bool usePascal, string expected)
        => Assert.AreEqual(expected, value.SnakeCaseToCamelCase(usePascal));
}
