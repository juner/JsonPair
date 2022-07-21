using System.Text.Json;

namespace Juners.Json.Serialization.Tests;

[TestClass()]
public class YyyyMmDdHhMmSsDateConverterTests
{
    static IEnumerable<object?[]> ReadTestData
    {
        get
        {
            yield return Read(@"""20220704160449""", new DateTime(2022, 7, 4, 16, 4, 49, DateTimeKind.Local));
            static object?[] Read(string json, DateTime expected)
                => new object?[] { json, expected };
        }
    }
    [TestMethod("json -> yyyymmddhhmmss datetime")]
    [DynamicData(nameof(ReadTestData))]
    public void ReadTest(string json, DateTime expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new YyyyMmDdHhMmSsDateConverter(),
            }
        };
        var actual = JsonSerializer.Deserialize<DateTime>(json, options);
        Assert.AreEqual(expected, actual);
    }
    static IEnumerable<object?[]> WriteTestData
    {
        get
        {
            yield return Write(new DateTime(2022, 7, 4, 16, 4, 49, DateTimeKind.Local), @"""20220704160449""");
            static object?[] Write(DateTime value, string expected)
                => new object?[] { value, expected };
        }
    }
    [TestMethod("yyyymmddhhmmss datetime -> json")]
    [DynamicData(nameof(WriteTestData))]
    public void WriteTest(DateTime value, string expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new YyyyMmDdHhMmSsDateConverter(),
            }
        };
        var actual = JsonSerializer.Serialize(value, options);
        Assert.AreEqual(expected, actual);
    }
}
