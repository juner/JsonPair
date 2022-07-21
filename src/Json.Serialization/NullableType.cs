namespace Juners.Json.Serialization;

/// <summary>
/// nullにする値
/// </summary>
[Flags]
public enum JsonNullableType
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
    /// <summary>
    /// empty arrray
    /// </summary>
    EmptyArray = 8,
    /// <summary>
    /// any nullable
    /// </summary>
    Any = Null | False | EmptyString,
}
