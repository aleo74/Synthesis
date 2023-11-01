using Synthesis.Core.Network.Metadata;

namespace Synthesis.Core.Network.Framing;

/// <summary>
/// Represents an interface for encoding Dofus protocol messages into a binary format.
/// </summary>
public interface IMessageEncoder
{
    /// <summary>
    /// Encodes a Dofus protocol message into a binary format.
    /// </summary>
    /// <param name="message">The <see cref="DofusMessage"/> to be encoded.</param>
    /// <returns>A <see cref="ReadOnlyMemory{T}"/> containing the binary representation of the encoded message.</returns>
    ReadOnlyMemory<byte> Encode(DofusMessage message);
}