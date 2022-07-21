using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// enum を pascalCase 文字列として表現する
/// </summary>
public class JsonStringPascalCaseEnumConverter : JsonStringEnumConverter
{
    /// <summary>
    /// 
    /// </summary>
    public JsonStringPascalCaseEnumConverter() : base(JsonNamingPolicy.CamelCase) { }
}
