using System.Text.Json;

namespace Juners.Json.Serialization.Tests;

[TestClass()]
public class YnBoolConverterTests
{
    static IEnumerable<object?[]> ReadTestData
    {
        get
        {
            yield return Read(@"""Y""", true);
            yield return Read(@"""N""", false);
            yield return Read(@"""y""", true);
            yield return Read(@"""n""", false);
            static object?[] Read(string json, bool expected)
                => new object?[] { json, expected };
        }
    }
    [TestMethod("json -> bool")]
    [DynamicData(nameof(ReadTestData))]
    public void ReadTest(string json, bool expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new YnBoolConverter(),
            }
        };
        var actual = JsonSerializer.Deserialize<bool>(json, options);
        Assert.AreEqual(expected, actual);
    }
    static IEnumerable<object?[]> WriteTestData
    {
        get
        {
            yield return Write(true, @"""Y""");
            yield return Write(false, @"""N""");
            static object?[] Write(bool value, string expected)
                => new object?[] { value, expected };
        }
    }
    [TestMethod("bool -> json")]
    [DynamicData(nameof(WriteTestData))]
    public void WriteTest(bool value, string expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new YnBoolConverter(),
            }
        };
        var actual = JsonSerializer.Serialize(value, options);
        Assert.AreEqual(expected, actual);
    }
}
