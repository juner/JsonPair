namespace Juners.Json.Extensions;

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
        => str.CamelCaseToSeparateCase('_');

    /// <summary>
    /// camelCase to kebab-case
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string CamelCaseToKebabCase(this string str)
        => str.CamelCaseToSeparateCase('-');

    /// <summary>
    /// camelCase to separate case
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static string CamelCaseToSeparateCase(this string str, char sep)
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
                    buffer[bufferPos++] = sep;
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
        => str.SeparateCaseToCamelCase('_', usePascal);

    /// <summary>
    /// kebab-case to camelCase
    /// </summary>
    /// <param name="str"></param>
    /// <param name="usePascal">use PascalCase</param>
    /// <returns></returns>
    public static string KebabCaseToCamelCase(this string str, bool usePascal = false)
        => str.SeparateCaseToCamelCase('-', usePascal);
    /// <summary>
    /// separatoe case to camelCase
    /// </summary>
    /// <param name="str"></param>
    /// <param name="sep">separator</param>
    /// <param name="usePascal">use PascalCase</param>
    /// <returns></returns>
    public static string SeparateCaseToCamelCase(this string str, char sep, bool usePascal = false)
    {
        ReadOnlySpan<char> snakeSpan = str;
        Span<char> buffer = stackalloc char[snakeSpan.Length];
        var bufferPos = 0;
        var isUpper = usePascal;
        for (var i = 0; i < snakeSpan.Length; i++)
        {
            var target = snakeSpan[i];
            if (target == sep)
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
