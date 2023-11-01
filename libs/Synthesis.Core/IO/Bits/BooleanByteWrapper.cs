namespace Synthesis.Core.IO.Bits;

/// <summary>
/// Provides utility methods to set and get flags in a byte representing boolean values.
/// </summary>
public static class BooleanByteWrapper
{
    /// <summary>
    /// Sets a flag at the specified offset in the given byte value.
    /// </summary>
    /// <param name="flag">The original byte value containing the flags.</param>
    /// <param name="offset">The bit offset at which to set the flag (must be less than 8).</param>
    /// <param name="value">The boolean value to set at the specified offset.</param>
    /// <returns>The modified byte value with the flag set at the specified offset.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is greater than or equal to 8.</exception>
    public static byte SetFlag(byte flag, byte offset, bool value)
    {
        if (offset >= 8)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset must be lesser than 8.");

        return value ? (byte)(flag | (1 << offset)) : (byte)(flag & (byte.MaxValue - (1 << offset)));
    }

    /// <summary>
    /// Gets the boolean value of the flag at the specified offset in the given byte value.
    /// </summary>
    /// <param name="flag">The byte value containing the flags.</param>
    /// <param name="offset">The bit offset at which to get the flag (must be less than 8).</param>
    /// <returns><c>true</c> if the flag at the specified offset is set; otherwise, <c>false</c>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the offset is greater than or equal to 8.</exception>
    public static bool GetFlag(byte flag, byte offset)
    {
        if (offset >= 8)
            throw new ArgumentOutOfRangeException(nameof(offset), offset, "Offset must be lesser than 8.");

        return (flag & (byte)(1 << offset)) is not 0;
    }
}