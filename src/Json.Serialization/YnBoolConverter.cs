using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// Y / N を真偽値に変換する コンバーター
/// </summary>
public class YnBoolConverter : JsonConverter<bool>
{
    /// <inheritdoc/>
    public override bool Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.ValueSpan.Length == 0)
            throw new FormatException();
        return char.ToUpperInvariant((char)reader.ValueSpan[0]) switch
        {
            'Y' => true,
            'N' => false,
            _ => throw new FormatException(),
        };
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options)
    {
        Span<byte> span = new byte[1];
        span[0] = (byte)(value ? 'Y' : 'N');
        writer.WriteStringValue(span);
    }
}
