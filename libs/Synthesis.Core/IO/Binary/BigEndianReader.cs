using System.Buffers.Binary;
using System.Text;

namespace Synthesis.Core.IO.Binary;

/// <summary>
/// Reads binary data in big-endian format.
/// </summary>
public sealed class BigEndianReader(ReadOnlyMemory<byte> buffer)
{
    /// <summary>
    /// Gets the total capacity of the reader.
    /// </summary>
    public int Capacity =>
        buffer.Length;
    
    /// <summary>
    /// Gets the current position within the reader.
    /// </summary>
    public int Cursor { get; private set; }
    
    /// <summary>
    /// Gets the remaining free capacity within the reader.
    /// </summary>
    public int FreeCapacity =>
        Capacity - Cursor;
    
    /// <summary>
    /// Reads an unsigned 8-bit integer from the current position.
    /// </summary>
    /// <returns>The read unsigned 8-bit integer value.</returns>
    public byte ReadUInt8() =>
        ReadSpan(sizeof(byte))[0];
    
    /// <summary>
    /// Reads a signed 8-bit integer from the current position.
    /// </summary>
    /// <returns>The read signed 8-bit integer value.</returns>
    public sbyte ReadInt8() =>
        (sbyte)ReadSpan(sizeof(byte))[0];

    /// <summary>
    /// Reads a boolean value from the current position.
    /// </summary>
    /// <returns>The read boolean value (true if not 0, otherwise false).</returns>
    public bool ReadBoolean() =>
        ReadSpan(sizeof(byte))[0] is not 0;
    
    /// <summary>
    /// Reads an unsigned 16-bit integer from the current position.
    /// </summary>
    /// <returns>The read unsigned 16-bit integer value.</returns>
    public ushort ReadUInt16() =>
        BinaryPrimitives.ReadUInt16BigEndian(ReadSpan(sizeof(ushort)));
    
    /// <summary>
    /// Reads a signed 16-bit integer from the current position.
    /// </summary>
    /// <returns>The read signed 16-bit integer value.</returns>
    public short ReadInt16() =>
        BinaryPrimitives.ReadInt16BigEndian(ReadSpan(sizeof(short)));
    
    /// <summary>
    /// Reads an unsigned 32-bit integer from the current position.
    /// </summary>
    /// <returns>The read unsigned 32-bit integer value.</returns>
    public uint ReadUInt32() =>
        BinaryPrimitives.ReadUInt32BigEndian(ReadSpan(sizeof(uint)));
    
    /// <summary>
    /// Reads a signed 32-bit integer from the current position.
    /// </summary>
    /// <returns>The read signed 32-bit integer value.</returns>
    public int ReadInt32() =>
        BinaryPrimitives.ReadInt32BigEndian(ReadSpan(sizeof(int)));
    
    /// <summary>
    /// Reads an unsigned 64-bit integer from the current position.
    /// </summary>
    /// <returns>The read unsigned 64-bit integer value.</returns>
    public ulong ReadUInt64() =>
        BinaryPrimitives.ReadUInt64BigEndian(ReadSpan(sizeof(ulong)));
    
    /// <summary>
    /// Reads a signed 64-bit integer from the current position.
    /// </summary>
    /// <returns>The read signed 64-bit integer value.</returns>
    public long ReadInt64() =>
        BinaryPrimitives.ReadInt64BigEndian(ReadSpan(sizeof(long)));
    
    /// <summary>
    /// Reads a signed 32-bit integer from the current position.
    /// </summary>
    /// <returns>The read signed 32-bit integer value as floating point.</returns>
    public float ReadFloat32() =>
        BinaryPrimitives.ReadSingleBigEndian(ReadSpan(sizeof(float)));
    
    /// <summary>
    /// Reads a signed 64-bit integer from the current position.
    /// </summary>
    /// <returns>The read signed 64-bit integer value as floating point.</returns>
    public double ReadFloat64() =>
        BinaryPrimitives.ReadDoubleBigEndian(ReadSpan(sizeof(double)));
    
    /// <summary>
    /// Reads a UTF-8 encoded string from the current position.
    /// </summary>
    /// <returns>The read string.</returns>
    public string ReadUtf() =>
        ReadUtfBytes(ReadUInt16());
    
    /// <summary>
    /// Reads a big UTF-8 encoded string from the current position.
    /// </summary>
    /// <returns>The read string.</returns>
    public string ReadBigUtf() =>
        ReadUtfBytes(ReadInt32());
    
    /// <summary>
    /// Reads a UTF-8 encoded string of the specified size from the current position.
    /// </summary>
    /// <param name="size">The size (in bytes) of the string to read.</param>
    /// <returns>The read string.</returns>
    public string ReadUtfBytes(int size) =>
        Encoding.UTF8.GetString(ReadBytes(size));

    /// <summary>
    /// Reads a memory of the specified size from the current position.
    /// </summary>
    /// <param name="size">The size (in bytes) of the memory to read.</param>
    /// <returns>The read memory as a <see cref="ReadOnlyMemory{T}"/>.</returns>
    /// <exception cref="OutOfMemoryException">Thrown when there is not enough free capacity to read the specified memory size.</exception>
    public ReadOnlyMemory<byte> ReadMemory(int size)
    {
        if (size > FreeCapacity)
            throw new OutOfMemoryException("Not enough free capacity to read memory.");
        
        var memory = buffer.Slice(Cursor, size);
        
        Cursor += size;
        
        return memory;
    }

    /// <summary>
    /// Reads a memory from the current position to the end.
    /// </summary>
    /// <returns>The read memory as a <see cref="ReadOnlyMemory{T}"/>.</returns>
    public ReadOnlyMemory<byte> ReadMemoryToEnd() =>
        ReadMemory(FreeCapacity);
    
    /// <summary>
    /// Reads a span of the specified size from the current position.
    /// </summary>
    /// <param name="size">The size (in bytes) of the span to read.</param>
    /// <returns>The read span as a <see cref="ReadOnlySpan{T}"/>.</returns>
    public ReadOnlySpan<byte> ReadSpan(int size) =>
        ReadMemory(size).Span;
    
    /// <summary>
    /// Reads a span from the current position to the end.
    /// </summary>
    /// <returns>The read span as a <see cref="ReadOnlySpan{T}"/>.</returns>
    public ReadOnlySpan<byte> ReadSpanToEnd() =>
        ReadMemoryToEnd().Span;
    
    /// <summary>
    /// Reads a byte array of the specified size from the current position.
    /// </summary>
    /// <param name="size">The size (in bytes) of the byte array to read.</param>
    /// <returns>The read byte array.</returns>
    public byte[] ReadBytes(int size) =>
        ReadMemory(size).ToArray();
    
    /// <summary>
    /// Reads a byte array from the current position to the end.
    /// </summary>
    /// <returns>The read byte array.</returns>
    public byte[] ReadBytesToEnd() =>
        ReadMemoryToEnd().ToArray();
    
    /// <summary>
    /// Seeks the reader to a specified position based on the provided origin and offset.
    /// </summary>
    /// <param name="origin">The seek origin (beginning, current, or end).</param>
    /// <param name="offset">The offset value.</param>
    public void Seek(SeekOrigin origin, int offset) =>
        Cursor = origin switch
        {
            SeekOrigin.Begin => offset,
            SeekOrigin.Current => Cursor + offset,
            SeekOrigin.End => Capacity - Math.Abs(offset),
            _ => throw new ArgumentOutOfRangeException(nameof(origin))
        };
}