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
public class PathThroughConverterTests
{
    [TestMethod("use PathThroughConverter")]
    public void PathThroughConverterTest()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        {
            Record1 record = new(new("test"), 1);
            var expected = "{\"child\":{\"childValue\":\"test\"},\"subValue\":1}";
            var actual = JsonSerializer.Serialize(record, options);
            Assert.AreEqual(expected, actual, "serialize string property");
        }
        {
            Record1 expected = new(new("test"), 1);
            var json = "{\"child\":{\"childValue\":\"test\"},\"subValue\":1}";
            var actual = JsonSerializer.Deserialize<Record1>(json, options);
            Assert.AreEqual(expected, actual, "deserialize string property");
        }
    }
    internal record Record1(
        [property: JsonConverter(typeof(PathThroughConverter))]
        RecordChild Child,
        [property: JsonConverter(typeof(PathThroughConverter))]
        int SubValue);
    internal record RecordChild(
        string ChildValue
    );
    [TestMethod("use PathThroughConverter<T>")]
    public void TypedPathThroughConverterTest()
    {
        JsonSerializerOptions options = new(JsonSerializerDefaults.Web);
        {
            Record2 record = new("test");
            var expected = "{\"value\":\"test\"}";
            var actual = JsonSerializer.Serialize(record, options);
            Assert.AreEqual(expected, actual, "serialize string property");
        }
        {
            Record2 expected = new("test");
            var json = "{\"value\":\"test\"}";
            var actual = JsonSerializer.Deserialize<Record2>(json, options);
            Assert.AreEqual(expected, actual, "deserialize string property");
        }
    }
    internal record Record2(
        [property: JsonConverter(typeof(PathThroughConverter<string>))]
        string Value);
}
