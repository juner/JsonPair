using System.Text.Json;

namespace Juners.Json.Serialization.Internals;

internal static class NullableTypeExtensions
{
    public static bool IsNullable(ref this Utf8JsonReader self, NullableType nullable)
    {
        if ((nullable & NullableType.Null) == NullableType.Null && self.TokenType is JsonTokenType.Null)
            return true;
        if ((nullable & NullableType.False) == NullableType.False && self.TokenType is JsonTokenType.False)
            return true;
        if ((nullable & NullableType.EmptyString) == NullableType.EmptyString && self.TokenType is JsonTokenType.String)
            return self.ValueSpan.Length == 0;
        if ((nullable & NullableType.EmptyArray) == NullableType.EmptyArray && self.TokenType is JsonTokenType.StartArray)
        {
            self.Read();
            return true;
        }
        return false;
    }
    public static bool TryWriteNull(this Utf8JsonWriter self, NullableType nullable)
    {
        if ((nullable & NullableType.Null) == NullableType.Null)
        {
            self.WriteNullValue();
            return true;
        }
        if ((nullable & NullableType.False) == NullableType.False)
        {
            self.WriteBooleanValue(false);
            return true;
        }
        if ((nullable & NullableType.EmptyString) == NullableType.EmptyString)
        {
            self.WriteStringValue(string.Empty);
            return true;
        }
        if ((nullable & NullableType.EmptyArray) == NullableType.EmptyArray)
        {
            self.WriteStartArray();
            self.WriteEndArray();
            return true;
        }
        return false;
    }
}
