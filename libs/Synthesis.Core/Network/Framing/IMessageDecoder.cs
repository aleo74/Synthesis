using System.Diagnostics.CodeAnalysis;
using Synthesis.Core.Network.Metadata;

namespace Synthesis.Core.Network.Framing;

/// <summary>
/// Represents an interface for decoding Dofus protocol messages from a memory buffer.
/// </summary>
public interface IMessageDecoder
{
    /// <summary>
    /// Attempts to decode a Dofus protocol message from the provided memory buffer.
    /// </summary>
    /// <param name="buffer">The memory buffer containing the message data.</param>
    /// <param name="message">When this method returns, contains the decoded <see cref="DofusMessage"/> if decoding is successful, or <see langword="null"/> if decoding failed.</param>
    /// <returns><see langword="true"/> if decoding was successful; otherwise, <see langword="false"/>.</returns>
    bool TryDecode(ref Memory<byte> buffer, [NotNullWhen(true)] out DofusMessage? message);
}
