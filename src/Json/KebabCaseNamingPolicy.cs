using Juners.Json.Extensions;
using System.Text.Json;

namespace Juners.Json;

/// <summary>
/// kebab-case Naming policy
/// </summary>
public class KebabCaseNamingPolicy : JsonNamingPolicy
{
    /// <inheritdoc/>
    public override string ConvertName(string name) => name.CamelCaseToKebabCase();
}
