using Synthesis.Core.Network.Metadata;
using Synthesis.Core.Network.Transport;

namespace Synthesis.Core.Network.Dispatcher;

/// <summary>
/// Represents an interface for dispatching Dofus protocol messages to a Dofus session.
/// </summary>
public interface IMessageDispatcher
{
    /// <summary>
    /// Dispatches a Dofus protocol message to a Dofus session asynchronously.
    /// </summary>
    /// <param name="session">The <see cref="DofusSession"/> to which the message will be dispatched.</param>
    /// <param name="message">The <see cref="DofusMessage"/> to be dispatched.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    Task DispatchAsync(DofusSession session, DofusMessage message);
}