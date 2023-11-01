using Synthesis.Core.IO.Binary;

namespace Synthesis.Core.Network.Metadata;

/// <summary>
/// Represents a base class for Dofus protocol messages.
/// </summary>
public abstract class DofusMessage
{
    /// <summary>
    /// Gets the unique identifier for the protocol message.
    /// </summary>
    protected internal abstract uint ProtocolId { get; }

    /// <summary>
    /// Serializes the message content to a binary format using a <see cref="BigEndianWriter"/>.
    /// </summary>
    /// <param name="writer">The <see cref="BigEndianWriter"/> used for serialization.</param>
    protected internal virtual void Serialize(BigEndianWriter writer)
    {
    }

    /// <summary>
    /// Deserializes the message content from a binary format using a <see cref="BigEndianReader"/>.
    /// </summary>
    /// <param name="reader">The <see cref="BigEndianReader"/> used for deserialization.</param>
    protected internal virtual void Deserialize(BigEndianReader reader)
    {
    }
}
