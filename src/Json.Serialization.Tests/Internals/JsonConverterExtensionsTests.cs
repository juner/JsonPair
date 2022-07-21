using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization.Internals.Tests;
[TestClass]
public class JsonConverterExtensionsTests
{
    public static IEnumerable<object?[]> TryGetFactoryTestData
    {
        get
        {
            {
                var converter = new JsonStringEnumConverter();
                yield return TryGetFactoryTest(converter, true, converter);
            }
            {
                JsonSerializerOptions options = new();
                var converter = options.GetConverter(typeof(int));
                yield return TryGetFactoryTest(converter, false, null);
            }

            static object?[] TryGetFactoryTest(JsonConverter self, bool expectedResult, JsonConverterFactory? expectedFactory)
                => new object?[] { self, expectedResult, expectedFactory };
        }
    }
    [TestMethod, DynamicData(nameof(TryGetFactoryTestData))]
    public void TryGetFactoryTest(JsonConverter self, bool expectedResult, JsonConverterFactory? expectedFactory)
    {
        var actualResult = self.TryGetFactory(out var actualFactory);
        Assert.AreEqual(expectedResult, actualResult, "result");
        Assert.AreEqual(expectedFactory, actualFactory, "factory");
    }
    public static IEnumerable<object?[]> TryGetTypedConverterTest1Data
    {
        get
        {
            {
                var converter = new JsonStringEnumConverter();
                yield return TryGetTypedConverterTest(converter, false, null, null);
            }
            {
                JsonSerializerOptions options = new();
                var typeToConvert = typeof(int);
                var converter = options.GetConverter(typeToConvert);
                yield return TryGetTypedConverterTest(converter, true, converter, typeToConvert);
            }
            static object?[] TryGetTypedConverterTest(JsonConverter self, bool expectedResult, JsonConverter? expectedConverter, Type? expectedTyped)
                => new object?[] { self, expectedResult, expectedConverter, expectedTyped };
        }
    }
    [TestMethod, DynamicData(nameof(TryGetTypedConverterTest1Data))]
    public void TryGetTypedConverterTest1(JsonConverter self, bool expectedResult, JsonConverter? expectedConverter, Type? expectedTyped)
    {
        var actualResult = self.TryGetTypedConverter(out var actualConverter, out var actualTyped);
        Assert.AreEqual(expectedResult, actualResult, "result");
        Assert.AreEqual(expectedConverter, actualConverter, "converter");
        Assert.AreEqual(expectedTyped, actualTyped, "typed");
    }
    public static IEnumerable<object?[]> TryCreateTypedConverterTestData
    {
        get
        {
            {
                var factory = new JsonStringEnumConverter();
                var typeToConvert = typeof(Enum1);
                JsonSerializerOptions options = new();
                var converter = factory.CreateConverter(typeToConvert, options);
                yield return TryCreateTypedConverterTest(factory, typeToConvert, options, true, converter, typeToConvert);
            }
            {
                JsonSerializerOptions options = new();
                var typeToConvert = typeof(int);
                var converter = options.GetConverter(typeToConvert);
                yield return TryCreateTypedConverterTest(converter, typeToConvert, options, true, converter, typeToConvert);
            }
            {
                JsonConverter? converter = null;
                var typeToConvert = typeof(int);
                JsonSerializerOptions options = new();
                yield return TryCreateTypedConverterTest(converter, typeToConvert, options, false, null, null);
            }
            static object?[] TryCreateTypedConverterTest(JsonConverter? con, Type typeToConvert, JsonSerializerOptions options, bool expectedResult, JsonConverter? expectedConverter, Type? expectedWithTyped)
                => new object?[] { con, typeToConvert, options, expectedResult, expectedConverter, expectedWithTyped };
        }
    }
    [TestMethod, DynamicData(nameof(TryCreateTypedConverterTestData))]
    public void TryCreateTypedConverterTest(JsonConverter? con, Type typeToConvert, JsonSerializerOptions options, bool expectedResult, JsonConverter? expectedConverter, Type? expectedWithTyped)
    {
        var actualResult = con.TryCreateTypedConverter(typeToConvert, options, out var actualConverter, out var actualWithTyped);
        Assert.AreEqual(expectedResult, actualResult, "result");
        if (expectedConverter is not null)
            Assert.IsInstanceOfType(actualConverter, expectedConverter.GetType(), "converter");
        else
            Assert.IsNull(actualConverter, "converter");
        Assert.AreEqual(expectedWithTyped, actualWithTyped, "withTyped");
    }
    internal enum Enum1
    {
        Value1,
        Value2,
    }
    public static IEnumerable<object?[]> TryGetTypedConverterTest2Data
    {
        get
        {
            {
                var factory = new JsonStringEnumConverter();
                var innerType = typeof(Enum2);
                var outerType = typeof(Enum2?);
                JsonSerializerOptions options = new();
                var converter = factory.CreateConverter(innerType, options);
                yield return TryGetTypedConverterTest(factory, outerType, options, true, converter, outerType, innerType);
            }
            {
                var factory = new JsonStringEnumConverter();
                var typeToConvert = typeof(Enum2);
                JsonSerializerOptions options = new();
                var converter = factory.CreateConverter(typeToConvert, options);
                yield return TryGetTypedConverterTest(factory, typeToConvert, options, true, converter, typeToConvert, typeToConvert);
            }
            {
                JsonSerializerOptions options = new();
                var typeToConvert = typeof(int?);
                var converter = options.GetConverter(typeToConvert);
                yield return TryGetTypedConverterTest(converter, typeToConvert, options, true, converter, typeToConvert, typeToConvert);
            }
            static object?[] TryGetTypedConverterTest(JsonConverter con, Type typeToConvert, JsonSerializerOptions options, bool expectedResult, JsonConverter expectedConverter, Type expectedOuterType, Type expectedInnerType)
                => new object?[] { con, typeToConvert, options, expectedResult, expectedConverter, expectedOuterType, expectedInnerType };
        }
    }
    [TestMethod, DynamicData(nameof(TryGetTypedConverterTest2Data))]
    public void TryGetTypedConverterTest2(JsonConverter con, Type typeToConvert, JsonSerializerOptions options, bool expectedResult, JsonConverter expectedConverter, Type expectedOuterType, Type expectedInnerType)
    {
        var actualResult = con.TryGetTypedConverter(typeToConvert, options, out var actualConverter, out var actualOuterType, out var actualInnerType);
        Assert.AreEqual(expectedResult, actualResult, "result");
        if (expectedConverter is not null)
            Assert.IsInstanceOfType(actualConverter, expectedConverter.GetType(), "converter");
        else
            Assert.IsNull(actualConverter, "converter");
        Assert.AreEqual(expectedOuterType, actualOuterType, "outerType");
        Assert.AreEqual(expectedInnerType, actualInnerType, "innerType");
    }
    internal enum Enum2
    {
        Value3,
        Value4,
    }
}
