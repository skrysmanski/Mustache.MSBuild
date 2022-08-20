using System.Diagnostics.CodeAnalysis;

using JetBrains.Annotations;

namespace AppMotor.NetStandardCompat.Extensions;

/// <summary>
/// Extension methods for <c>string</c>.
/// </summary>
public static class StringExtensions
{
    /// <summary>
    /// Same as <see cref="string.IsNullOrEmpty"/> but with nullable reference type (NRT) support.
    /// </summary>
    [PublicAPI, Pure]
    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrEmpty(str);
    }

    /// <summary>
    /// Same as <see cref="string.IsNullOrWhiteSpace"/> but with nullable reference type (NRT) support.
    /// </summary>
    [PublicAPI, Pure]
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str)
    {
        return string.IsNullOrWhiteSpace(str);
    }
}
