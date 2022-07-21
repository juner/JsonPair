using System.Buffers.Text;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// YYYYMMDD 形式 と DateTime を相互変換するコンバーター
/// </summary>
public class YyyyMmDdDateConverter : JsonConverter<DateTime>
{
    /// <inheritdoc/>
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        Debug.Assert(typeToConvert == typeof(DateTime));
        var span = reader.ValueSpan;
        if (!Utf8Parser.TryParse(span[..4], out int year, out _))
            throw new FormatException();
        if (!Utf8Parser.TryParse(span[4..6], out byte month, out _))
            throw new FormatException();
        if (!Utf8Parser.TryParse(span[6..8], out byte day, out _))
            throw new FormatException();
        return new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Local);
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        var utf8Date_ = new byte[8];
        Array.Fill(utf8Date_, (byte)'0');
        Span<byte> utf8Date = utf8Date_;
        Span<byte> tmpSpan = new byte[4];
        var tmp = tmpSpan[..4];
        if (Utf8Formatter.TryFormat(value.Year, tmp, out var written))
            tmp[..written].CopyTo(utf8Date[(4 - written)..4]);
        else throw new FormatException();
        tmp = tmpSpan[..2];
        if (Utf8Formatter.TryFormat((byte)value.Month, tmp, out written))
            tmp[..written].CopyTo(utf8Date[(4 + 2 - written)..(4 + 2)]);
        else throw new FormatException();
        tmp = tmpSpan[..2];
        if (Utf8Formatter.TryFormat((byte)value.Day, tmp, out written))
            tmp[..written].CopyTo(utf8Date[(6 + 2 - written)..(6 + 2)]);
        else throw new FormatException();
        writer.WriteStringValue(utf8Date);
    }
}
