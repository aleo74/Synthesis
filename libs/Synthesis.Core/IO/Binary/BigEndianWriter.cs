using System.Buffers;
using System.Buffers.Binary;
using System.Text;

namespace Synthesis.Core.IO.Binary;

/// <summary>
/// Writes binary data in big-endian format.
/// </summary>
public sealed class BigEndianWriter : IDisposable
{
    private byte[] _buffer = Array.Empty<byte>();
    private bool _isBufferRented;
    private int _cursor;

    /// <summary>
    /// Gets the capacity of the buffer.
    /// </summary>
    public int Capacity => 
        _buffer.Length;
    
    /// <summary>
    /// Gets the maximum position of the cursor.
    /// </summary>
    public int MaxCursor { get; private set; }

    /// <summary>
    /// Gets the cursor position in the buffer.
    /// </summary>
    public int Cursor
    {
        get => _cursor;
        private set
        {
            _cursor = value;

            if (MaxCursor < value) 
                MaxCursor = value;
        }
    }

    /// <summary>
    /// Gets the amount of free capacity in the buffer.
    /// </summary>
    public int FreeCapacity =>
        Capacity - Cursor;

    /// <summary>
    /// Gets the buffer as a <see cref="Memory{T}"/>.
    /// </summary>
    public Memory<byte> BufferAsMemory =>
        _buffer.AsMemory(0, MaxCursor);

    /// <summary>
    /// Gets the buffer as a <see cref="Span{T}"/>.
    /// </summary>
    public Span<byte> BufferAsSpan =>
        _buffer.AsSpan(0, MaxCursor);

    /// <summary>
    /// Gets the buffer as a byte array.
    /// </summary>
    public byte[] BufferAsArray =>
        _buffer.AsSpan(0, MaxCursor).ToArray();

    /// <summary>
    /// Writes an unsigned 8-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUInt8(byte value) =>
        GetSpan(sizeof(byte))[0] = value;

    /// <summary>
    /// Writes a signed 8-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteInt8(sbyte value) => 
        GetSpan(sizeof(sbyte))[0] = (byte)value;

    /// <summary>
    /// Writes a boolean to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteBoolean(bool value) => 
        GetSpan(sizeof(bool))[0] = (byte)(value ? 1 : 0);

    /// <summary>
    /// Writes an unsigned 16-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUInt16(ushort value) => 
        BinaryPrimitives.WriteUInt16BigEndian(GetSpan(sizeof(ushort)), value);

    /// <summary>
    /// Writes a signed 16-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteInt16(short value) => 
        BinaryPrimitives.WriteInt16BigEndian(GetSpan(sizeof(short)), value);

    /// <summary>
    /// Writes an unsigned 32-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUInt32(uint value) => 
        BinaryPrimitives.WriteUInt32BigEndian(GetSpan(sizeof(uint)), value);

    /// <summary>
    /// Writes a signed 32-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteInt32(int value) => 
        BinaryPrimitives.WriteInt32BigEndian(GetSpan(sizeof(int)), value);

    /// <summary>
    /// Writes an unsigned 64-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteUInt64(ulong value) =>
        BinaryPrimitives.WriteUInt64BigEndian(GetSpan(sizeof(ulong)), value);

    /// <summary>
    /// Writes a signed 64-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteInt64(long value) =>
        BinaryPrimitives.WriteInt64BigEndian(GetSpan(sizeof(long)), value);

    /// <summary>
    /// Writes a floating point signed 32-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteFloat32(float value) =>
        BinaryPrimitives.WriteSingleBigEndian(GetSpan(sizeof(float)), value);

    /// <summary>
    /// Writes a floating point signed 64-bit integer to the buffer.
    /// </summary>
    /// <param name="value">The value to write.</param>
    public void WriteFloat64(double value) => 
        BinaryPrimitives.WriteDoubleBigEndian(GetSpan(sizeof(double)), value);

    /// <summary>
    /// Writes a <see cref="Memory{T}"/> to the buffer.
    /// </summary>
    /// <param name="value">The memory to write.</param>
    public void WriteMemory(Memory<byte> value) =>
        WriteSpan(value.Span);

    /// <summary>
    /// Writes a <see cref="Span{T}"/> to the buffer.
    /// </summary>
    /// <param name="value">The span to write.</param>
    public void WriteSpan(Span<byte> value) => 
        value.CopyTo(GetSpan(value.Length));

    /// <summary>
    /// Writes a string as UTF-8 encoded bytes to the buffer.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public void WriteUtfBytes(string value) => 
        WriteSpan(Encoding.UTF8.GetBytes(value));

    /// <summary>
    /// Writes a string as UTF-8 encoded bytes to the buffer, preceded by a 16-bit length.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public void WriteUtf(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);

        WriteUInt16((ushort)bytes.Length);
        WriteSpan(bytes);
    }

    /// <summary>
    /// Writes a string as UTF-8 encoded bytes to the buffer, preceded by a 32-bit length.
    /// </summary>
    /// <param name="value">The string to write.</param>
    public void WriteBigUtf(string value)
    {
        var bytes = Encoding.UTF8.GetBytes(value);

        WriteInt32(bytes.Length);
        WriteSpan(bytes);
    }

    /// <summary>
    /// Seeks the cursor position within the buffer.
    /// </summary>
    /// <param name="origin">The origin from which the cursor is moved (Begin, Current, or End).</param>
    /// <param name="offset">The offset to move the cursor.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when an invalid seek origin is provided.</exception>
    public void Seek(SeekOrigin origin, int offset)
    {
        switch (origin)
        {
            case SeekOrigin.Begin:
                CheckAndResizeBuffer(offset, offset);
                Cursor = offset;
                break;
            case SeekOrigin.Current:
                CheckAndResizeBuffer(offset);
                Cursor += offset;
                break;
            case SeekOrigin.End:
                Cursor = Capacity - Math.Abs(offset);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(origin));
        }
    }
    
    /// <summary>
    /// Gets a portion of the buffer as a <see cref="ReadOnlyMemory{T}"/>.
    /// </summary>
    /// <param name="length">The length of the memory to get.</param>
    /// <returns>A <see cref="ReadOnlyMemory{T}"/> containing the specified portion of the buffer.</returns>
    public ReadOnlyMemory<byte> GetBufferAsMemory(int length) => 
        _buffer.AsMemory(0, length);
    
    /// <summary>
    /// Gets a portion of the buffer as a <see cref="ReadOnlySpan{T}"/>.
    /// </summary>
    /// <param name="length">The length of the span to get.</param>
    /// <returns>A <see cref="ReadOnlySpan{T}"/> containing the specified portion of the buffer.</returns>
    public ReadOnlySpan<byte> GetBufferAsSpan(int length) =>
        _buffer.AsSpan(0, length);
    
    /// <summary>
    /// Gets a portion of the buffer as an array.
    /// </summary>
    /// <param name="length">The length of the array to get.</param>
    /// <returns>An array containing the specified portion of the buffer.</returns>
    public byte[] GetBufferAsArray(int length) => 
        _buffer.AsSpan(0, length).ToArray();
    
    /// <summary>
    /// Disposes of the <see cref="BigEndianWriter"/> and returns the rented buffer to the pool if applicable.
    /// </summary>
    public void Dispose()
    {
        if (_isBufferRented)
        {
            ArrayPool<byte>.Shared.Return(_buffer);
            _isBufferRented = false;
        }
    }

    private Span<byte> GetSpan(int count)
    {
        CheckAndResizeBuffer(count);

        var span = _buffer.AsSpan(Cursor, count);

        Cursor += count;

        return span;
    }

    private void CheckAndResizeBuffer(int count, int? position = null)
    {
        position ??= Cursor;
        var bytesAvailable = Capacity - position.Value;

        if (count <= bytesAvailable)
            return;

        var currentCount = Capacity;
        var growBy = Math.Max(count, currentCount);

        if (count is 0)
            growBy = Math.Max(growBy, 256);

        var newCount = currentCount + growBy;

        if ((uint)newCount > int.MaxValue)
        {
            var needed = (uint)(currentCount - bytesAvailable + count);

            if (needed > Array.MaxLength)
                throw new OutOfMemoryException("The requested operation would exceed the maximum array length.");

            newCount = Array.MaxLength;
        }

        var newArray = ArrayPool<byte>.Shared.Rent(newCount);
        Array.Copy(_buffer, newArray, _buffer.Length);

        if (_isBufferRented)
            ArrayPool<byte>.Shared.Return(_buffer);

        _buffer = newArray;
        _isBufferRented = true;
    }
}