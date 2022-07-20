using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Juners.Json.Serialization.Internals;

internal static class JsonConverterExtensions
{
    /// <summary>
    /// <see cref="JsonConverter"/> -&gt; <see cref="JsonConverterFactory"/>
    /// </summary>
    /// <param name="self"></param>
    /// <param name="factory"></param>
    /// <returns></returns>
    public static bool TryGetFactory(this JsonConverter self, [NotNullWhen(true)] out JsonConverterFactory factory)
    {
        factory = default!;
        if (self is not JsonConverterFactory f)
            return false;
        factory = f;
        return true;
    }
    /// <summary>
    /// <see cref="JsonConverter"/> -&gt; <see cref="JsonConverter{T}"/>
    /// </summary>
    /// <param name="self"></param>
    /// <param name="converter"></param>
    /// <param name="typed"></param>
    /// <returns></returns>
    public static bool TryGetTypedConverter(this JsonConverter self, [NotNullWhen(true)] out JsonConverter converter, [NotNullWhen(true)] out Type typed)
    {
        converter = default!;
        typed = default!;
        var t = self.GetType();
        do
        {
            if (t is { IsGenericType: true} && t.GetGenericTypeDefinition() == typeof(JsonConverter<>))
            {
                converter = self;
                typed = t.GetGenericArguments()[0]!;
                return true;
            }
        } while ((t == t.BaseType) is { });
        return false;
    }
    /// <summary>
    /// <see cref="JsonConverter"/> -&gt; <see cref="JsonConverterFactory.CreateConverter(Type, JsonSerializerOptions)"/> -&gt; <see cref="JsonConverter{T}"/>
    /// </summary>
    /// <param name="con"></param>
    /// <param name="typeToConvert"></param>
    /// <param name="options"></param>
    /// <param name="converter"></param>
    /// <param name="withTyped"></param>
    /// <returns></returns>
    public static bool TryCreateTypedConverter(this JsonConverter con, Type typeToConvert, JsonSerializerOptions options, [NotNullWhen(true)]out JsonConverter converter, [NotNullWhen(true)] out Type withTyped)
    {
        var c = con;
        do
        {
            if (c.TryGetTypedConverter(out converter, out withTyped))
                return typeToConvert.IsAssignableFrom(withTyped);
            if (!c.TryGetFactory(out var f) || !f.CanConvert(typeToConvert))
                return false;
            c = f.CreateConverter(typeToConvert, options);
        } while (c is not null);
        return false;
    }
}
