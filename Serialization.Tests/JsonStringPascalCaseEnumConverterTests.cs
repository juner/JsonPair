using System.Text.Json;

namespace JsonPair.Serialization.Tests;

[TestClass()]
public class JsonStringPascalCaseEnumConverterTests
{
    static IEnumerable<object?[]> ReadTestData
    {
        get
        {
            yield return Read(@"""friendlyType""", EnumType.FriendlyType);
            yield return Read(@"""process""", EnumType.Process);
            static object?[] Read(string json, EnumType expected)
                => new object?[] { json, expected };
        }
    }
    [TestMethod("json -> enum")]
    [DynamicData(nameof(ReadTestData))]
    public void ReadTest(string json, EnumType expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new JsonStringPascalCaseEnumConverter(),
            }
        };
        var actual = JsonSerializer.Deserialize<EnumType>(json, options);
        Assert.AreEqual(expected, actual);
    }
    static IEnumerable<object?[]> WriteTestData
    {
        get
        {
            yield return Write(EnumType.FriendlyType, @"""friendlyType""");
            yield return Write(EnumType.Process, @"""process""");
            static object?[] Write(EnumType value, string expected)
                => new object?[] { value, expected };
        }
    }
    [TestMethod("enum -> json")]
    [DynamicData(nameof(WriteTestData))]
    public void WriteTest(EnumType value, string expected)
    {
        JsonSerializerOptions options = new()
        {
            Converters =
            {
                new JsonStringPascalCaseEnumConverter(),
            }
        };
        var actual = JsonSerializer.Serialize(value, options);
        Assert.AreEqual(expected, actual);
    }
    public enum EnumType
    {
        FriendlyType,
        Process,
    }
}

