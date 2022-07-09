namespace JsonPair.Extensions;

/// <summary>
/// string case converter
/// </summary>
public static class StringCaseExtensions
{
    /// <summary>
    /// camelCase to snake_case
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string CamelCaseToSnakeCase(this string str)
    {
        ReadOnlySpan<char> camelSpan = str;
        Span<char> buffer = stackalloc char[(camelSpan.Length * 2) - 1];
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
    /// <summary>
    /// snake_case to camelCase
    /// </summary>
    /// <param name="str"></param>
    /// <param name="usePascal">use PascalCase</param>
    /// <returns></returns>
    public static string SnakeCaseToCamelCase(this string str, bool usePascal = false)
    {
        ReadOnlySpan<char> snakeSpan = str;
        Span<char> buffer = stackalloc char[snakeSpan.Length];
        var bufferPos = 0;
        var isUpper = usePascal;
        for (var i = 0; i < snakeSpan.Length; i++)
        {
            var target = snakeSpan[i];
            if (target == '_')
            {
                isUpper = true;
                continue;
            }
            if (isUpper)
            {
                buffer[bufferPos++] = char.ToUpper(target);
                isUpper = false;
            }
            else
                buffer[bufferPos++] = char.ToLower(target);
        }
        return buffer[..bufferPos].ToString();
    }
}
