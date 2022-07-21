using System.Text.Json;

namespace Juners.Json.Serialization.Internals;

internal static class NullableTypeExtensions
{
    public static bool IsNullable(in this Utf8JsonReader self, NullableType nullable)
    {
        if ((nullable & NullableType.Null) == NullableType.Null)
            return true;
        if ((nullable & NullableType.False) == NullableType.False)
            return true;
        if ((nullable & NullableType.EmptyString) == NullableType.EmptyString)
            return self.ValueSpan.Length == 0;
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
        return false;
    }
}
