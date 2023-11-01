namespace Synthesis.Core.Extensions;

/// <summary>
/// A static class containing extension methods for working with collections.
/// </summary>
public static class CollectionExtensions
{
    /// <summary>
    /// Searches for the specified value in a read-only span and returns the index of the first occurrence within the specified range.
    /// </summary>
    /// <typeparam name="T">The type of elements in the span.</typeparam>
    /// <param name="span">The read-only span to search in.</param>
    /// <param name="value">The value to locate in the span.</param>
    /// <param name="start">The zero-based index at which to start the search.</param>
    /// <returns>
    /// The zero-based index of the first occurrence of the specified value within the range starting at the specified index,
    /// if found; otherwise, -1.
    /// </returns>
    public static int IndexOf<T>(this ReadOnlySpan<T> span, T value, int start) where T : IEquatable<T>
    {
        for (var i = start; i < span.Length; i++)
            if (span[i].Equals(value))
                return i;

        return -1;
    }
}