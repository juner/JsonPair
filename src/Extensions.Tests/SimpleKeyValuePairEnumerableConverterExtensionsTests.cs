using JsonPair.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonPair.Extensions.Tests;

[TestClass()]
public class SimpleKeyValuePairEnumerableConverterExtensionsTests
{
    [TestMethod("snake_case による 辞書の作成")]
    public void SnakeCaseTest()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var actual = new Record1("test", 40).ToKeyValuePairEnumerable(options).ToList();
        var expected = new List<KeyValuePair<string, string>>
        {
            new("openValue", "test" ),
            new("starNumber", "40" ),
        };
        Assert.AreEqual(expected.Count, actual.Count, nameof(expected.Count));
        CollectionAssert.AreEquivalent(expected, actual);

    }
    internal record Record1(string OpenValue, int StarNumber);
    [TestMethod("コンバータを使用した文字列化")]
    public void ConverterTest()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var actual = new Record2(
            new DateTime(2022, 7, 4, 11, 22, 33),
            null,
            false,
            null).ToKeyValuePairEnumerable(options).ToList();
        var expected = new List<KeyValuePair<string, string>>
        {
            new("startDate", "20220704" ),
            new("endDate", "null" ),
            new("yesAndNo", "N" ),
            new("onOrOff", "null" ),
        };
        Assert.AreEqual(expected.Count, actual.Count, nameof(expected.Count));
        CollectionAssert.AreEquivalent(expected, actual);

    }
    internal record Record2(
        [property: JsonConverter(typeof(YyyyMmDdDateConverter))] DateTime StartDate,
        [property: JsonConverter(typeof(YyyyMmDdHhMmSsDateConverter))] DateTime? EndDate,
        [property: JsonConverter(typeof(YnBoolConverter))] bool? YesAndNo,
        [property: JsonConverter(typeof(BitBoolConverter))] bool? OnOrOff);
    [TestMethod("除外パターン")]
    public void IgnoreTest()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var actual = new Record3(null, default, "test1", "test2")
            .ToKeyValuePairEnumerable(options).ToList();
        var expected = new List<KeyValuePair<string, string>>
        {
            // jsonIgnore Never は 出力される
            new("valueData1","test1" ),
            // get only property は出力される
            new("getOnlyValue", "sugoi!"),
            // set only property は出力されない
            // フィールドはオプションで指定されない限り出力されない
        };
        Assert.AreEqual(expected.Count, actual.Count, nameof(expected.Count));
        CollectionAssert.AreEquivalent(expected, actual);
    }
    internal record Record3(
        [property: JsonConverter(typeof(YnBoolConverter))]
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]bool? YesAndNo,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)] int StateNum,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Never)] string ValueData1,
        [property: JsonIgnore(Condition = JsonIgnoreCondition.Always)] string ValueData2)
    {
        public string GetOnlyValue { get; } = "sugoi!";
        public string SetOnlyValue { set { } }
        public string FieldValue = "test";
    }
    static IEnumerable<object?[]> TextEncodingPatternTestData
    {
        get
        {
            yield return TextEncodingPatternTest("\"");
            yield return TextEncodingPatternTest("\\");
            yield return TextEncodingPatternTest("/");
            yield return TextEncodingPatternTest("&");
            yield return TextEncodingPatternTest("'");

            static object?[] TextEncodingPatternTest(string expected)
                => new[] { expected };
        }
    }
    [TestMethod("文字列エンコードパターン")]
    [DynamicData(nameof(TextEncodingPatternTestData))]
    public void TextEncodingPatternTest(string expected)
    {
        var actual = new Record4(expected).ToKeyValuePairEnumerable().First().Value;
        Assert.AreEqual(expected, actual);
    }
    internal record Record4(string Value);
    [TestMethod("クラスへの JsonConverter の設定")]
    public void ClassConvertAnnotationTest()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var actual = new Record5("test1", Sugoi.VeryVeryTsuyoi, Myou.Myau)
            .ToKeyValuePairEnumerable(options).ToList();
        var expected = new List<KeyValuePair<string, string>>
        {
            new("value", "test1"),
            new("sugoi", "veryVeryTsuyoi"),
            new("myou", "1"),
        };
        Assert.AreEqual(expected.Count, actual.Count, nameof(expected.Count));
        CollectionAssert.AreEquivalent(expected, actual);
    }
    internal record Record5(string Value, Sugoi Sugoi, Myou Myou);
    [JsonConverter(typeof(JsonStringPascalCaseEnumConverter))]
    internal enum Sugoi
    {
        Default = 0,
        Super,
        VeryVeryTsuyoi,
    };
    internal enum Myou
    {
        Default = 0,
        Myau = 1,
        Myuu = 2,
    }
    [TestMethod("フィールド対応")]
    public void FieldSupportTest()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
        {
            IncludeFields = true,
        };
        var actual = new Record6("test1", 6)
            .ToKeyValuePairEnumerable(options).ToList();
        var expected = new List<KeyValuePair<string, string>>
        {
            new("test", "test1"),
            new("value", "6"),
        };
        Assert.AreEqual(expected.Count, actual.Count, nameof(expected.Count));
        CollectionAssert.AreEquivalent(expected, actual);
    }
    internal class Record6
    {
        public string Test;
        public int Value;
        public Record6(string test, int value) => (Test, Value) = (test, value);
    }
    [TestMethod("JsonPropertyName によるプロパティ名の指定")]
    public void JsonPropertyNameTest()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        var actual = new Record7("test-value")
            .ToKeyValuePairEnumerable(options).ToList();
        var expected = new List<KeyValuePair<string, string>>
        {
            new("data-value", "test-value"),
        };
        Assert.AreEqual(expected.Count, actual.Count, nameof(expected.Count));
        CollectionAssert.AreEquivalent(expected, actual);
    }
    internal record Record7([property: JsonPropertyName("data-value")] string ValueData);
}
