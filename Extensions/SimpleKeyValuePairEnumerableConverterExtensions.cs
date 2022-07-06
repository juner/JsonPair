using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace JsonPair.Extensions;

/// <summary>
/// オブジェクトを key/value で列挙する為の拡張関数 <see cref="ToKeyValuePairEnumerable{T}(T, JsonSerializerOptions)"/> の為の拡張関数群
/// </summary>
public static class SimpleKeyValuePairEnumerableConverterExtensions
{
    /// <summary>
    /// <typeparamref name="T"/> を key/value 形式に列挙する
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="self"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairEnumerable<T>(this T self, JsonSerializerOptions? options = null)
    {
        options = new JsonSerializerOptions(options ?? new())
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };
        return ToEnumerable(self, options);
        static IEnumerable<KeyValuePair<string, string>> ToEnumerable(T self, JsonSerializerOptions options)
        {
            var namingPolicy = options.PropertyNamingPolicy ?? JsonNamingPolicy.CamelCase;
            var includeFields = options.IncludeFields;
            IEnumerable<MemberInfo> v = typeof(T).GetProperties();
            if (includeFields)
                v = v.Concat(typeof(T).GetFields());
            foreach (var member in v)
            {
                string name;
                if (member.GetCustomAttribute<JsonPropertyNameAttribute>() is { } nameAttr)
                {
                    name = nameAttr.Name;
                }
                else
                {
                    name = namingPolicy.ConvertName(member.Name);
                }
                var inputType = member.GetUnderlyingType();
                var isIgnore = InvokeIsIgnore<T>(member, self);
                if (isIgnore)
                    continue;
                var value = member.GetUnderlyingValue(self);
                var o = new JsonSerializerOptions(options);
                if (member.GetCustomAttribute<JsonConverterAttribute>() is { } conv
                    && GetConverterFromAttribute(conv, inputType) is JsonConverter converter)
                {
                    o.Converters.Add(converter);
                }
                var stringValue = JsonSerializer.Serialize(value, inputType, o);
                stringValue = RestoreStringLiteral(stringValue);
                yield return new(name, stringValue);
            }

        }
    }
    /// <summary>
    /// <paramref name="member"/>を元にその値のタイプを取得する
    /// </summary>
    /// <param name="member"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    static Type GetUnderlyingType(this MemberInfo member)
        => member.MemberType switch
        {
            MemberTypes.Field => ((FieldInfo)member).FieldType,
            MemberTypes.Property => ((PropertyInfo)member).PropertyType,
            _ => throw new ArgumentException($"not support memberType:{member.MemberType}"),
        };
    /// <summary>
    /// <paramref name="member"/>を元に<paramref name="self"/>に於けるその値を取得する
    /// </summary>
    /// <param name="member"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    static object? GetUnderlyingValue(this MemberInfo member, object? self)
        => member.MemberType switch
        {
            MemberTypes.Field => ((FieldInfo)member).GetValue(self),
            MemberTypes.Property => ((PropertyInfo)member).GetValue(self),
            _ => throw new ArgumentException($"not support memberType:{member.MemberType}"),
        };
    static MethodInfo? IS_IGNORE_CACHE;
    static MethodInfo IsIgnoreCache => IS_IGNORE_CACHE ??= typeof(SimpleKeyValuePairEnumerableConverterExtensions)
            .GetMethod(nameof(IsIgnore), BindingFlags.NonPublic | BindingFlags.Static)!;
    /// <summary>
    /// <see cref="IsIgnore{T1, T2}(MemberInfo, T2)"/> を プロパティの型を適用して呼び出す
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="member"></param>
    /// <param name="self"></param>
    /// <returns></returns>
    static bool InvokeIsIgnore<T>(MemberInfo member, object? self)
    {
        var inputType = member.GetUnderlyingType();
        var method = IsIgnoreCache.MakeGenericMethod(inputType, typeof(T))!;
#pragma warning disable CS8605 // null の可能性がある値をボックス化解除しています。
        return (bool)method.Invoke(null, new object?[] { member, self });
#pragma warning restore CS8605 // null の可能性がある値をボックス化解除しています。
    }
    /// <summary>
    /// skip対象かの判定
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    /// <param name="member"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    [MethodImpl(MethodImplOptions.NoInlining)]
    static bool IsIgnore<T1, T2>(MemberInfo member, T2 current)
    {
        // getter を持たない Property は skip
        if (member.MemberType is MemberTypes.Property && ((PropertyInfo)member).GetGetMethod() is null)
            return true;
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
        var value = (T1)member.GetUnderlyingValue(current);
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
        var propertyTypeCanBeNull = typeof(T1).CanBeNull();
        var ignoreDefaultValuesOnWrite = false;
        if (member.GetCustomAttribute<JsonIgnoreAttribute>() is { } ignore)
        {
            if (ignore.Condition is JsonIgnoreCondition.Always)
                return true;
            if (ignore.Condition is JsonIgnoreCondition.WhenWritingDefault)
            {
                ignoreDefaultValuesOnWrite = true;
            }
            else if (ignore.Condition is JsonIgnoreCondition.WhenWritingNull)
            {
                if (propertyTypeCanBeNull)
                    ignoreDefaultValuesOnWrite = true;
                else
                    throw new InvalidOperationException("ignore condition on value type invalid");
            }
        }
        if (ignoreDefaultValuesOnWrite)
        {
            if (value is null)
                return true;
            if (!propertyTypeCanBeNull)
            {
                var defaultValeu = default(T1);
                if (EqualityComparer<T1>.Default.Equals(defaultValeu, value))
                    return true;
            }

        }
        return false;
    }
    private static bool CanBeNull(this Type type) => !type.IsValueType || type.IsNullableOfT();
    private static readonly Type NULLABLETYPE = typeof(Nullable<>);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static bool IsNullableOfT(this Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == NULLABLETYPE;
    /// <summary>
    /// <see cref="JsonConverter"/>を <see cref="JsonConverterAttribute"/>を元に取得する
    /// </summary>
    /// <param name="converterAttribute"></param>
    /// <param name="typeToConvert"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    private static JsonConverter GetConverterFromAttribute(JsonConverterAttribute converterAttribute, Type typeToConvert)
    {
        JsonConverter? converter;
        var converterType = converterAttribute.ConverterType;
        if (converterType is null)
        {
            converter = converterAttribute.CreateConverter(typeToConvert);
            if (converter is null)
                throw new InvalidOperationException("serialization converter on attribute not compatible");
        }
        else
        {
            var ctor = converterType.GetConstructor(Type.EmptyTypes);
            if (!typeof(JsonConverter).IsAssignableFrom(converterType) || ctor is null || !ctor.IsPublic)
                throw new InvalidOperationException("serialization converter on attribute invalid");
#pragma warning disable CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
            converter = (JsonConverter)Activator.CreateInstance(converterType);
#pragma warning restore CS8600 // Null リテラルまたは Null の可能性がある値を Null 非許容型に変換しています。
        }
        Debug.Assert(converter is not null);
        return converter!;
    }
    /// <summary>
    /// json の 文字列リテラルを元に戻して返す
    /// </summary>
    /// <param name="stringValue"></param>
    /// <returns></returns>
    static string RestoreStringLiteral(string stringValue)
    {
        var span = stringValue.AsSpan();
        if (span.Length < 2
            || span[0] != '"'
            || span[^1] != '"')
        {
            return stringValue;
        }
        //　先頭と末尾 に " が存在する場合、エスケープされている可能性が高い

        // 先頭と末尾から " を除去
        stringValue = new string(span[1..^1]);
        // エスケープされている場合の対応 \" と \\ を 元の形式に戻す
        if (stringValue.Contains("\\\\", StringComparison.CurrentCulture)
            || stringValue.Contains("\\\"", StringComparison.CurrentCulture))
            stringValue = stringValue.Replace("\\\\", "\\").Replace("\\\"", "\"");
        return stringValue;
    }
    /// <summary>
    /// key/value の列挙を クエリ文字列として変換する
    /// </summary>
    /// <param name="param"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static string BuildQueryString(this IEnumerable<KeyValuePair<string, string>> param)
    {
        if (param is null)
            throw new ArgumentNullException(nameof(param));
        StringBuilder builder = new();
        foreach (var (key, value) in param
            .Where(v => !string.IsNullOrEmpty(v.Key))
            .Select(v => (v.Key, v.Value)))
        {
            if (builder.Length > 0)
                builder.Append('&');
            builder.Append(Uri.EscapeDataString(key));
            builder.Append('=');
            builder.Append(Uri.EscapeDataString(value));
        }
        return builder.ToString();
    }
}
