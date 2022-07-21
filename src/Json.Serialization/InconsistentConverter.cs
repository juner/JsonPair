using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// inconsistent converter (default object serialize / deserialize)
/// </summary>
public class InconsistentConverter : JsonConverter<object>
{
    /// <summary>
    /// inconsistent converter (default object serialize / deserialize)
    /// </summary>
    public InconsistentConverter() { }
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => true;
    /// <inheritdoc/>
    public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize(ref reader, typeToConvert, options);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, options);
}

/// <summary>
/// inconsistent converter (default object serialize / deserialize)
/// </summary>
public class InconsistentConverter<T> : JsonConverter<T>
    where T : class
{
    /// <summary>
    /// inconsistent converter (default object serialize / deserialize)
    /// </summary>
    public InconsistentConverter() { }
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert) => typeof(T).IsAssignableFrom(typeToConvert);
    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => JsonSerializer.Deserialize<T>(ref reader, options);

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T? value, JsonSerializerOptions options)
        => JsonSerializer.Serialize(writer, value, options);
}
