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
public class NullableConverterTests
{
    [TestMethod()]
    public void CanConvertTest1()
    {
        var innerConverter = new JsonStringEnumConverter();
        var outerConverter = new NullableConverter(innerConverter);

        // 前提条件 inner の挙動
        Assert.AreEqual(true, innerConverter.CanConvert(typeof(Enum1)), "inner enum");
        Assert.AreEqual(false, innerConverter.CanConvert(typeof(Enum1?)), "inner enum nullable");

        // outer の挙動 ↑が↓になること
        Assert.AreEqual(true, outerConverter.CanConvert(typeof(Enum1)), "outer enum");
        Assert.AreEqual(true, outerConverter.CanConvert(typeof(Enum1?)), "outer enum nullable");

        JsonSerializerOptions options = new();
        {
            var actualConverter = outerConverter.CreateConverter(typeof(Enum1), options);
            var actualConverterDefinitionType = actualConverter?.GetType().GetGenericTypeDefinition();
            Assert.AreEqual(typeof(NullableConverter<>), actualConverterDefinitionType, "simple");
        }
        {
            var actualConverter = outerConverter.CreateConverter(typeof(Enum1?), options);
            var actualConverterDefinitionType = actualConverter?.GetType().GetGenericTypeDefinition();
            Assert.AreEqual(typeof(NullableConverter<,>), actualConverterDefinitionType, "nullable");
        }

    }
    internal enum Enum1
    {
        Value1,
        Value2,
    }
}
