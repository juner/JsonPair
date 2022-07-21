using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// 
/// </summary>
public class BitBoolConverter : JsonConverter<bool>
{
    /// <inheritdoc/>
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.ValueSpan.Length == 0)
            throw new FormatException();
        return (char)reader.ValueSpan[0] switch
        {
            '0' => false,
            '1' => true,
            _ => throw new FormatException(),
        };
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
        => writer.WriteNumberValue(value ? 1 : 0);
}
