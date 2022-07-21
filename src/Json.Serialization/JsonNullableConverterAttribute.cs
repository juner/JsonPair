using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// use <see cref="JsonNullableConverter"/> attribute.
/// </summary>
public class JsonNullableConverterAttribute : JsonConverterAttribute
{
    readonly Type _converterType;
    /// <summary>
    /// read nullable
    /// </summary>
    public JsonNullableType ReadNullable { get; set; }
    /// <summary>
    /// write nullable
    /// </summary>
    public JsonNullableType WriteNullable { get; set; }
    /// <summary>
    /// use <see cref="JsonNullableConverter"/> attribute.
    /// </summary>
    /// <param name="converterType"></param>
    /// <param name="nullable"></param>
    public JsonNullableConverterAttribute(Type converterType, JsonNullableType nullable = JsonNullableType.Null) : this(converterType, nullable, nullable) { }
    /// <summary>
    /// use <see cref="JsonNullableConverter"/> attribute.
    /// </summary>
    /// <param name="converterType"></param>
    /// <param name="readNullable"></param>
    /// <param name="writeNullable"></param>
    /// <exception cref="ArgumentException"></exception>
    public JsonNullableConverterAttribute(Type converterType, JsonNullableType readNullable, JsonNullableType writeNullable) : base(default!)
    {
        if (!typeof(JsonConverter).IsAssignableFrom(converterType))
            throw new ArgumentException($"required type is {typeof(JsonConverter)}. converterType:{converterType}", nameof(converterType));
        _converterType = converterType;
        ReadNullable = readNullable;
        WriteNullable = writeNullable;
    }
    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert)
    {
        var converter = (JsonConverter)Activator.CreateInstance(_converterType)!;
        return new JsonNullableConverter(converter, ReadNullable, WriteNullable);
    }
}
