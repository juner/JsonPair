namespace JsonPair.Extensions;

/// <summary>
/// string case converter
/// </summary>
public static class StringCaseExtensions
{
    /// <summary>
    /// CamelCase to snake_case
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string ToSnakeCase(this string str)
    {
        ReadOnlySpan<char> camelSpan = str;
        Span<char> buffer = stackalloc char[camelSpan.Length * 2 - 1];
        var bufferPos = 0;
        for (var i = 0; i < camelSpan.Length; i++)
        {
            var target = camelSpan[i];
            if (char.IsUpper(target))
            {
                if (i > 0)
                    buffer[bufferPos++] = '_';
                buffer[bufferPos++] = char.ToLower(target);
            }
            else
            {
                buffer[bufferPos++] = target;
            }
        }
        return buffer[..bufferPos].ToString();
    }
}
