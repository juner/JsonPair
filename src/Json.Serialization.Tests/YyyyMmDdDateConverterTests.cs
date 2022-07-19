using System.Text.Json;

namespace Juners.Json.Serialization.Tests;

[TestClass()]
public class YyyyMmDdDateConverterTests
{
    static IEnumerable<object?[]> ReadTestData
    {
        get
        {
            yield return Read(@"""20220704""", new DateTime(2022, 7, 4, 0, 0, 0, DateTimeKind.Local));
            static object?[] Read(string json, DateTime expected)
                => new object?[] { json, expected };
        }
    }
    [TestMethod("json -> yyyymmdd datetime")]
    [DynamicData(nameof(ReadTestData))]
    public void ReadTest(string json, DateTime expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new YyyyMmDdDateConverter(),
            }
        };
        var actual = JsonSerializer.Deserialize<DateTime>(json, options);
        Assert.AreEqual(expected, actual);
    }
    static IEnumerable<object?[]> WriteTestData
    {
        get
        {
            yield return Write(new DateTime(2022, 7, 4, 0, 0, 0, DateTimeKind.Local), @"""20220704""");
            static object?[] Write(DateTime value, string expected)
                => new object?[] { value, expected };
        }
    }
    [TestMethod("yyyymmdd datetime -> json")]
    [DynamicData(nameof(WriteTestData))]
    public void WriteTest(DateTime value, string expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new YyyyMmDdDateConverter(),
            }
        };
        var actual = JsonSerializer.Serialize(value, options);
        Assert.AreEqual(expected, actual);
    }
}
