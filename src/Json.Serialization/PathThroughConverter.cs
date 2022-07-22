using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// path through converter (default object serialize / deserialize)
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
///   [property: JsonConverter(typeof(PathThroughConverter))]
///   string Value
/// );
/// </code>
/// Wrong usage
/// <code>
/// [JsonConverter(typeof(PathThroughConverter)]
/// record NgRecord(
///   string Value
/// );
/// </code>
/// </example>
public class PathThroughConverter : JsonConverterFactory
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => true;
    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        => (JsonConverter?)Activator.CreateInstance(typeof(PathThroughConverter<>).MakeGenericType(typeToConvert));
}

/// <summary>
/// path through converter (default object serialize / deserialize)
/// </summary>
/// <typeparam name="T">Force type</typeparam>
/// <remarks>
/// Do not register and use it in <see cref="JsonSerializerOptions"/>.<br/>
/// Also, do not register it in the <see cref="JsonConverterAttribute"/> of the <c>class</c> or <c>struct</c>.<br/>
/// Set <c>property</c> or <c>field</c> using <see cref="JsonConverterAttribute"/>.
/// </remarks>
/// <example>
/// Correct usage
/// <code>
/// record OkRecord(
///   [property: JsonConverter(typeof(PathThroughConverter&lt;string&gt;))]
///   string Value
/// );
/// </code>
/// Wrong usage
/// <code>
/// [JsonConverter(typeof(PathThroughConverter&lt;string&gt;)]
/// record NgRecord(
///   string Value
/// );
/// </code>
/// </example>
public class PathThroughConverter<T> : JsonConverter<T>
{
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
        => typeof(T).IsAssignableFrom(typeToConvert);
    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<T>(ref reader, options);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, options);
}
