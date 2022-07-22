using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// default converter
/// </summary>
/// <remarks>
/// Do not register and use it in <see cref="JsonSerializerOptions"/>.<br/>
/// Also, do not register it in the <see cref="JsonConverterAttribute"/> of the <c>class</c> or <c>struct</c>.<br/>
/// Set <c>property</c> or <c>field</c> using <see cref="JsonConverterAttribute"/>.
/// </remarks>
/// <example>
/// Correct usage
/// <code>
/// record OkRecord(
///   [property: JsonConverter(typeof(JsonDefaultConverter))]
///   string Value
/// );
/// </code>
/// Wrong usage
/// <code>
/// [JsonConverter(typeof(JsonDefaultConverter)]
/// record NgRecord(
///   string Value
/// );
/// </code>
/// </example>
public class JsonDefaultConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => true;
    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(typeof(JsonDefaultConverter<>).MakeGenericType(typeToConvert));
}
/// <summary>
/// default converter
/// </summary>
/// <remarks>
/// Do not register and use it in <see cref="JsonSerializerOptions"/>.<br/>
/// Also, do not register it in the <see cref="JsonConverterAttribute"/> of the <c>class</c> or <c>struct</c>.<br/>
/// Set <c>property</c> or <c>field</c> using <see cref="JsonConverterAttribute"/>.
/// </remarks>
/// <example>
/// Correct usage
/// <code>
/// record OkRecord(
///   [property: JsonConverter(typeof(JsonDefaultConverter&lt;string&gt;))]
///   string Value
/// );
/// </code>
/// Wrong usage
/// <code>
/// [JsonConverter(typeof(JsonDefaultConverter&lt;string&gt;)]
/// record NgRecord(
///   string Value
/// );
/// </code>
/// </example>
public class JsonDefaultConverter<T> : JsonConverter<T>
{
    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => (T?)JsonSerializer.Deserialize(ref reader, typeToConvert);
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value);
}
