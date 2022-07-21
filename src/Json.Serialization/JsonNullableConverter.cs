using Juners.Json.Serialization.Internals;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;
/// <summary>
/// any nullable type converter
/// </summary>
public sealed class JsonNullableConverter : JsonConverterFactory
{
    readonly JsonConverter _converter;
    readonly JsonNullableType _readNullable;
    readonly JsonNullableType _writeNullable;
    /// <summary>
    /// any nullable type converter
    /// </summary>
    /// <param name="converter"></param>
    public JsonNullableConverter(JsonConverter converter) : this(converter, JsonNullableType.Null) { }
    /// <summary>
    /// any nullable type converter
    /// </summary>
    /// <param name="converter"></param>
    /// <param name="nullable"></param>
    public JsonNullableConverter(JsonConverter converter, JsonNullableType nullable = JsonNullableType.Null) : this(converter, nullable, nullable) { }
    /// <summary>
    /// any nullable type converter
    /// </summary>
    /// <param name="converter"></param>
    /// <param name="readNullable"></param>
    /// <param name="writeNullable"></param>
    public JsonNullableConverter(JsonConverter converter, JsonNullableType readNullable, JsonNullableType writeNullable)
    {
        Validate(converter);
        _converter = converter;
        _readNullable = readNullable;
        _writeNullable = writeNullable;
    }
    static void Validate(JsonConverter converter)
    {
        if (converter is null)
            throw new ArgumentNullException(nameof(converter));
        if (converter is JsonNullableConverter)
            throw new ArgumentException($"not support converter type {converter.GetType()}", nameof(converter));
        if (converter.GetType() is { IsGenericType: true } and Type type
            && type.GetGenericTypeDefinition() is Type definitionType && (
                definitionType == typeof(JsonNullableConverter<>)
                || definitionType == typeof(JsonNullableConverter<,>)
            ))
            throw new ArgumentException($"not support converter type {converter.GetType()}", nameof(converter));
    }
    /// <inheritdoc/>
    public override bool CanConvert(Type typeToConvert)
    {
        if (_converter.TryGetFactory(out var factory))
        {
            return factory.CanConvert(typeToConvert)
                || (
                    Nullable.GetUnderlyingType(typeToConvert) is { } notNullableTypeToConvert
                    && factory.CanConvert(notNullableTypeToConvert)
                );
        }
        if (_converter.TryGetTypedConverter(out _, out var typed))
        {
            return typed == typeof(object) || typeToConvert.IsAssignableFrom(typed);
        }
        return false;
    }
    /// <inheritdoc/>
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (_converter.TryGetTypedConverter(typeToConvert, options, out var converter, out var outerType, out var innerType))
        {
            var type = outerType != innerType
                ? typeof(JsonNullableConverter<,>).MakeGenericType(outerType, innerType)
                : typeof(JsonNullableConverter<>).MakeGenericType(outerType);
            return Activator.CreateInstance(type, new object?[] { converter, _readNullable, _writeNullable }) as JsonConverter;
        }
        return null;
    }
}
/// <summary>
/// any nullable type converter
/// </summary>
/// <typeparam name="TOuter">外見型</typeparam>
/// <typeparam name="TInner">内部型</typeparam>
public sealed class JsonNullableConverter<TOuter, TInner> : JsonConverter<TOuter>
{
    /// <inheritdoc/>
    public override bool HandleNull => true;
    readonly JsonConverter<TInner> _converter;
    readonly JsonNullableType _readNullable;
    readonly JsonNullableType _writeNullable;
    /// <summary>
    /// any nullable type converter
    /// </summary>
    /// <param name="converter"></param>
    /// <param name="nullable"></param>
    public JsonNullableConverter(JsonConverter<TInner> converter, JsonNullableType nullable) : this(converter, nullable, nullable) { }
    /// <summary>
    /// any nullable type converter
    /// </summary>
    /// <param name="converter"></param>
    /// <param name="readNullable"></param>
    /// <param name="writeNullable"></param>
    public JsonNullableConverter(JsonConverter<TInner> converter, JsonNullableType readNullable, JsonNullableType writeNullable)
    {
        _converter = converter;
        _readNullable = readNullable;
        _writeNullable = writeNullable;
    }
    /// <inheritdoc/>
    public override TOuter? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.IsNullable(_readNullable))
            return default;
        typeToConvert = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        if (_converter.Read(ref reader, typeToConvert, options) is not TOuter result)
            return default;
        return result;
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TOuter value, JsonSerializerOptions options)
    {
        if (value is not TInner innerValue)
        {
            if (!writer.TryWriteNull(_writeNullable))
                throw new JsonException();
            return;
        }
        _converter.Write(writer, innerValue, options);
    }
}
/// <summary>
/// any nullable type converter
/// </summary>
/// <typeparam name="T">外見型</typeparam>
public sealed class JsonNullableConverter<T> : JsonConverter<T>
{
    /// <inheritdoc/>
    public override bool HandleNull => true;
    readonly JsonConverter<T> _converter;
    readonly JsonNullableType _readNullable;
    readonly JsonNullableType _writeNullable;
    /// <summary>
    /// any nullable type converter
    /// </summary>
    /// <param name="converter"></param>
    /// <param name="readNullable"></param>
    /// <param name="writeNullable"></param>
    public JsonNullableConverter(JsonConverter<T> converter, JsonNullableType readNullable, JsonNullableType writeNullable)
    {
        _converter = converter;
        _readNullable = readNullable;
        _writeNullable = writeNullable;
    }
    /// <inheritdoc/>
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.IsNullable(_readNullable))
            return default;
        typeToConvert = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        return _converter.Read(ref reader, typeToConvert, options)!;
    }
    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, T value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            if (!writer.TryWriteNull(_writeNullable))
                throw new JsonException();
            return;
        }
        _converter.Write(writer, value, options);
    }
}
