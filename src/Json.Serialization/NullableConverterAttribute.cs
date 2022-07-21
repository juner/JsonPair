﻿using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

/// <summary>
/// use <see cref="NullableConverter"/> attribute.
/// </summary>
public class NullableConverterAttribute : JsonConverterAttribute
{
    readonly Type _converterType;
    public NullableType ReadNullable { get; set; }
    public NullableType WriteNullable { get; set; }
    /// <summary>
    /// use <see cref="NullableConverter"/> attribute.
    /// </summary>
    /// <param name="converterType"></param>
    /// <param name="nullable"></param>
    public NullableConverterAttribute(Type converterType, NullableType nullable = NullableType.Null) : this(converterType, nullable, nullable) { }
    /// <summary>
    /// use <see cref="NullableConverter"/> attribute.
    /// </summary>
    /// <param name="converterType"></param>
    /// <param name="readNullable"></param>
    /// <param name="writeNullable"></param>
    /// <exception cref="ArgumentException"></exception>
    public NullableConverterAttribute(Type converterType, NullableType readNullable, NullableType writeNullable) : base(default!)
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
        var factory = new NullableConverter(converter);
        JsonSerializerOptions options = new();
        return factory.CreateConverter(typeToConvert, options);
    }
}
