using Juners.Json.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Juners.Json.Extensions.Tests;

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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <param name="expected"></param>
    [TestMethod("CamelCase -> kebab-case")]
    [DataRow("CamelCase", "camel-case")]
    [DataRow("pascalCase", "pascal-case")]
    [DataRow("ACamelCase", "a-camel-case")]
    public void CamelCaseToKebabCaseTest(string value, string expected)
        => Assert.AreEqual(expected, value.CamelCaseToKebabCase());

    [TestMethod("kebab-case -> CamelCase ")]
    [DataRow("kebab-case", false, "kebabCase")]
    [DataRow("kebab-case", true, "KebabCase")]
    [DataRow("-kebab-case", false, "KebabCase")]
    [DataRow("KEBAB-CASE", false, "kebabCase")]
    public void KebabCaseToCamelCaseTest(string value, bool usePascal, string expected)
        => Assert.AreEqual(expected, value.KebabCaseToCamelCase(usePascal));
}
