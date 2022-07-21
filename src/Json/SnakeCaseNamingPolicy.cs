using Juners.Json.Extensions;
using System.Text.Json;

namespace Juners.Json;

/// <summary>
/// snake_case 形式に名前を変換するポリシー
/// </summary>
public class SnakeCaseNamingPolicy : JsonNamingPolicy
{
    /// <inheritdoc/>
    public override string ConvertName(string name) => name.CamelCaseToSnakeCase();
}
