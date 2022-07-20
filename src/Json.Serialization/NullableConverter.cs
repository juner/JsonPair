using Juners.Json.Serialization.Internals;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization;

public class NullableConverter : JsonConverterFactory
{
    readonly JsonConverter _converter;
    readonly NullableType _readNullable;
    readonly NullableType _writeNullable;
    public NullableConverter(JsonConverter converter, NullableType readNullable, NullableType writeNullable)
    {
        _converter = converter;
        _readNullable = readNullable;
        _writeNullable = writeNullable;
    }

    public override bool CanConvert(Type typeToConvert)
    {
        if(_converter.TryGetFactory(out var factory))
        {
            return factory.CanConvert(typeToConvert)
                || (
                    Nullable.GetUnderlyingType(typeToConvert) is { } notNullableTypeToConvert
                    && factory.CanConvert(notNullableTypeToConvert)
                );
        }
        if (_converter.TryGetTypedConverter(out _, out var typed))
        {
            return typeToConvert.IsAssignableFrom(typed);
        }
        return false;
    }
    public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
    {
        if (TryGetCreateConverter(typeToConvert, options, out var converter, out var outerType, out var innerType))
        {
            var type = outerType == innerType
                ? typeof(NullableConverter<>).MakeGenericType(outerType)
                : typeof(NullableConverter<,>).MakeGenericType(outerType, innerType);
            return Activator.CreateInstance(type, new object?[] { converter, _readNullable, _writeNullable }) as JsonConverter;
        }
        return null;
    }
    bool TryGetCreateConverter(Type typeToConvert, JsonSerializerOptions options,[NotNullWhen(true)]out JsonConverter converter, [NotNullWhen(true)] out Type outerType, [NotNullWhen(true)] out Type innerType)
    {
        outerType = typeToConvert;
        if (_converter.TryGetFactory(out var factory))
        {
            if (factory.TryCreateTypedConverter(typeToConvert, options, out converter, out innerType))
                return outerType.IsAssignableFrom(innerType);
            else if (Nullable.GetUnderlyingType(typeToConvert) is { } notNullableTypeToConvert
                    && factory.TryCreateTypedConverter(notNullableTypeToConvert, options, out converter, out innerType))
                return outerType.IsAssignableFrom(innerType);
            return false;
        }
        if (_converter.TryGetTypedConverter(out converter, out innerType))
            return outerType.IsAssignableFrom(innerType);
        return false;
    }
}
public class NullableConverter<T1,T2> : JsonConverter<T1>
    where T1 : T2?
{
    readonly JsonConverter<T2> _converter;
    readonly NullableType _readNullable;
    readonly NullableType _writeNullable;
    public NullableConverter(JsonConverter<T2> converter, NullableType readNullable, NullableType writeNullable)
    {
        _converter = converter;
        _readNullable = readNullable;
        _writeNullable = writeNullable;
    }

    public override T1? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.IsNullable(_readNullable))
            return default;
        typeToConvert = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        return (T1)_converter.Read(ref reader, typeToConvert, options)!;
    }
    public override void Write(Utf8JsonWriter writer, T1 value, JsonSerializerOptions options)
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
public class NullableConverter<T> : JsonConverter<T>
{
    readonly JsonConverter<T> _converter;
    readonly NullableType _readNullable;
    readonly NullableType _writeNullable;
    public NullableConverter(JsonConverter<T> converter, NullableType readNullable, NullableType writeNullable)
    {
        _converter = converter;
        _readNullable = readNullable;
        _writeNullable = writeNullable;
    }
    public override T? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.IsNullable(_readNullable))
            return default;
        typeToConvert = Nullable.GetUnderlyingType(typeToConvert) ?? typeToConvert;
        return _converter.Read(ref reader, typeToConvert, options)!;
    }
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
/// <summary>
/// nullにする値
/// </summary>
[Flags]
public enum NullableType
{
    /// <summary>
    /// 未設定
    /// </summary>
    None = default,
    /// <summary>
    /// Null
    /// </summary>
    Null = 1,
    /// <summary>
    /// False
    /// </summary>
    False = 2,
    /// <summary>
    /// Empty String
    /// </summary>
    EmptyString = 4,
}

internal static class NullableTypeExtensions
{
    public static bool IsNullable(in this Utf8JsonReader self, NullableType nullable)
    {
        if ((nullable & NullableType.Null) == NullableType.Null)
            return true;
        if ((nullable & NullableType.False) == NullableType.False)
            return true;
        if ((nullable & NullableType.EmptyString)== NullableType.EmptyString)
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
