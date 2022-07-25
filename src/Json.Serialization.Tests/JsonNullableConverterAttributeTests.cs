using Juners.Json.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Juners.Json.Serialization.Tests;

[TestClass()]
public class JsonNullableConverterAttributeTests
{
    [TestMethod("NullableType")]
    public void NullableTypeSetTest()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        {
            var json = "{\"value1\":false,\"value2\":\"\",\"value3\":null,\"value4\":[]}";
            Record1 expectedResult = new(null, null, null, null);
            var actualResult = JsonSerializer.Deserialize<Record1>(json, options)!;
            Assert.AreEqual(expectedResult, actualResult, "deserialize nullable");
        }
        {
            var json = "{\"value1\":null}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record1>(json, options));
        }
        {
            var json = "{\"value1\":\"\"}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record1>(json, options));
        }
        {
            var json = "{\"value2\":null}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record1>(json, options));
        }
        {
            var json = "{\"value2\":false}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record1>(json, options));
        }
        {
            var json = "{\"value3\":\"\"}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record1>(json, options));
        }
        {
            var json = "{\"value3\":false}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record1>(json, options));
        }
        {
            var json = "{\"value1\":\"Type1\",\"value2\":\"Type2\",\"value3\":\"Type3\",\"value4\":{\"value\":\"test\"}}";
            Record1 expectedResult = new(Enum1.Type1, Enum1.Type2, Enum1.Type3, new("test"));
            var actualResult = JsonSerializer.Deserialize<Record1>(json, options)!;
            Assert.AreEqual(expectedResult, actualResult, "deserialize");
        }
        {
            var expectedJson = "{\"value1\":false,\"value2\":\"\",\"value3\":null,\"value4\":[]}";
            Record1 record = new(null, null, null, null);
            var actualJson = JsonSerializer.Serialize(record, options)!;
            Assert.AreEqual(expectedJson, actualJson, "serialize nullable");
        }
        {
            var expectedJson = "{\"value1\":\"Type1\",\"value2\":\"Type2\",\"value3\":\"Type3\",\"value4\":{\"value\":\"test\"}}";
            Record1 record = new(Enum1.Type1, Enum1.Type2, Enum1.Type3, new("test"));
            var actualJson = JsonSerializer.Serialize(record, options)!;
            Assert.AreEqual(expectedJson, actualJson, "serialize");
        }
    }
    internal record Record1(
        [property: JsonNullableConverter(typeof(JsonStringEnumConverter), JsonNullableType.False)]
        Enum1? Value1 = null,
        [property: JsonNullableConverter(typeof(JsonStringEnumConverter), JsonNullableType.EmptyString)]
        Enum1? Value2 = null,
        [property: JsonNullableConverter(typeof(JsonStringEnumConverter), JsonNullableType.Null)]
        Enum1? Value3 = null,
        [property: JsonNullableConverter(typeof(PathThroughConverter), JsonNullableType.EmptyArray)]
        RecordChild1? Value4 = null);
    internal enum Enum1
    {
        Type1,
        Type2,
        Type3,
        Type4,
    }
    internal record RecordChild1(string Value);
    [TestMethod("NullableType.Any")]
    public void AnyNullableSetTest()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        {
            var json = "{\"value\":false}";
            Record2 expectedResult = new(null);
            var actualResult = JsonSerializer.Deserialize<Record2>(json, options)!;
            Assert.AreEqual(expectedResult, actualResult, "deserialize any nullable `false`");
        }
        {
            var json = "{\"value\":\"\"}";
            Record2 expectedResult = new(null);
            var actualResult = JsonSerializer.Deserialize<Record2>(json, options)!;
            Assert.AreEqual(expectedResult, actualResult, "deserialize any nullable `\"\"`");
        }
        {
            var json = "{\"value\":null}";
            Record2 expectedResult = new(null);
            var actualResult = JsonSerializer.Deserialize<Record2>(json, options)!;
            Assert.AreEqual(expectedResult, actualResult, "deserialize any nullable ` null`");
        }
        {
            var expectedJson = "{\"value\":null}";
            Record2 record = new(null);
            var actualJson = JsonSerializer.Serialize(record, options)!;
            Assert.AreEqual(expectedJson, actualJson, "serialize any nullable");
        }

    }
    internal record Record2(
        [property: JsonNullableConverter(typeof(JsonStringEnumConverter), ReadNullable = JsonNullableType.Any)]
        Enum1? Value
        );
    [TestMethod("NullableType.None")]
    public void NoSetNullableSetTest()
    {

        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        {
            var json = "{\"value\":null}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record3>(json, options));
        }
        {
            var json = "{\"value\":false}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record3>(json, options));
        }
        {
            var json = "{\"value\":\"\"}";
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Deserialize<Record3>(json, options));
        }
        {
            var json = "{\"value\":\"Type2\"}";
            Record3 expectedResult = new(Enum1.Type2);
            var actualResult = JsonSerializer.Deserialize<Record3>(json, options);
            Assert.AreEqual(expectedResult, actualResult);
        }
        {
            Record3 record = new(null);
            Assert.ThrowsException<JsonException>(() => JsonSerializer.Serialize(record, options));
        }
        {
            var expectedJson = "{\"value\":\"Type2\"}";
            Record3 record = new(Enum1.Type2);
            var actualJson = JsonSerializer.Serialize(record, options);
            Assert.AreEqual(expectedJson, actualJson);
        }
    }

    internal record Record3(
        [property: JsonNullableConverter(typeof(JsonStringEnumConverter), JsonNullableType.None)]
        Enum1? Value
        );
}
