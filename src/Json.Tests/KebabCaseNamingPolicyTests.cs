using System.Text.Json;

namespace Juners.Json.Tests;

[TestClass]
public class KebabCaseNamingPolicyTests
{
    [TestMethod("snake_case json -> model")]
    public void DeserializeTest()
    {
        var content = "{\"result-data\":{\"status-code\":\"200\"}}";
        JsonSerializerOptions option = new()
        {
            PropertyNamingPolicy = new KebabCaseNamingPolicy(),
        };
        var result = JsonSerializer.Deserialize<ResultClass>(content, option);
        Assert.IsNotNull(result);
        var data = result.ResultData;
        Assert.AreEqual("200", data.StatusCode);
    }
    [TestMethod("model -> snake_case json")]
    public void SerializeTest()
    {
        var data = new ResultClass(new ResultDataClass("200"));
        JsonSerializerOptions option = new()
        {
            PropertyNamingPolicy = new KebabCaseNamingPolicy(),
        };
        var json = JsonSerializer.Serialize(data, option);
        Assert.AreEqual("{\"result-data\":{\"status-code\":\"200\"}}", json);
    }
    internal record ResultClass(ResultDataClass ResultData);
    internal record ResultDataClass(string StatusCode);
}
