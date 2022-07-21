using System.Text.Json;

namespace Juners.Json.Serialization.Internals;

internal static class NullableTypeExtensions
{
    public static bool IsNullable(ref this Utf8JsonReader self, JsonNullableType nullable)
    {
        if ((nullable & JsonNullableType.Null) == JsonNullableType.Null && self.TokenType is JsonTokenType.Null)
            return true;
        if ((nullable & JsonNullableType.False) == JsonNullableType.False && self.TokenType is JsonTokenType.False)
            return true;
        if ((nullable & JsonNullableType.EmptyString) == JsonNullableType.EmptyString && self.TokenType is JsonTokenType.String)
            return self.ValueSpan.Length == 0;
        if ((nullable & JsonNullableType.EmptyArray) == JsonNullableType.EmptyArray && self.TokenType is JsonTokenType.StartArray)
        {
            self.Read();
            return true;
        }
        return false;
    }
    public static bool TryWriteNull(this Utf8JsonWriter self, JsonNullableType nullable)
    {
        if ((nullable & JsonNullableType.Null) == JsonNullableType.Null)
        {
            self.WriteNullValue();
            return true;
        }
        if ((nullable & JsonNullableType.False) == JsonNullableType.False)
        {
            self.WriteBooleanValue(false);
            return true;
        }
        if ((nullable & JsonNullableType.EmptyString) == JsonNullableType.EmptyString)
        {
            self.WriteStringValue(string.Empty);
            return true;
        }
        if ((nullable & JsonNullableType.EmptyArray) == JsonNullableType.EmptyArray)
        {
            self.WriteStartArray();
            self.WriteEndArray();
            return true;
        }
        return false;
    }
}
