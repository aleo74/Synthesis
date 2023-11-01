using System.Buffers;
using System.Net.Sockets;
using Synthesis.Core.Network.Dispatcher;
using Synthesis.Core.Network.Framing;
using Synthesis.Core.Network.Metadata;

namespace Synthesis.Core.Network.Transport;

/// <summary>
/// Represents a session for handling Dofus protocol messages over a socket connection.
/// </summary>
public abstract class DofusSession(Socket socket, IMessageDecoder decoder, IMessageEncoder encoder, IMessageDispatcher dispatcher) : IDisposable
{
    private readonly CancellationTokenSource _cts = new();
    private readonly MemoryPool<byte> _memoryPool = MemoryPool<byte>.Shared;
    private bool _disposed;

    private string? _sessionId;

    /// <summary>
    /// Gets the unique identifier for the session.
    /// </summary>
    public string SessionId =>
        _sessionId ??= Guid.NewGuid().ToString("N");

    /// <summary>
    /// Gets a cancellation token associated with the session.
    /// </summary>
    public CancellationToken CancellationToken =>
        _cts.Token;

    /// <summary>
    /// Starts the Dofus session to receive and process incoming messages.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    internal async Task StartAsync()
    {
        while (!_cts.IsCancellationRequested)
        {
            using var owner = _memoryPool.Rent();

            var bytesRead = await socket.ReceiveAsync(owner.Memory, SocketFlags.None, CancellationToken).ConfigureAwait(false);

            if (bytesRead is 0)
                break;

            var buffer = owner.Memory[..bytesRead];

            while (decoder.TryDecode(ref buffer, out var message))
                await dispatcher.DispatchAsync(this, message).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Sends a specific type of Dofus message asynchronously.
    /// </summary>
    /// <typeparam name="TMessage">The type of Dofus message to send.</typeparam>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask SendAsync<TMessage>() where TMessage : DofusMessage, new()
    {
        return SendAsync(new TMessage());
    }

    /// <summary>
    /// Sends a Dofus message asynchronously.
    /// </summary>
    /// <param name="message">The Dofus message to send.</param>
    /// <returns>A <see cref="ValueTask"/> representing the asynchronous operation.</returns>
    public ValueTask SendAsync(DofusMessage message)
    {
        var buffer = encoder.Encode(message);

        var task = socket.SendAsync(buffer, SocketFlags.None, CancellationToken);

        return task.IsCompletedSuccessfully
            ? ValueTask.CompletedTask
            : FlushAsync(task);

        static async ValueTask FlushAsync(ValueTask<int> flushTask)
        {
            await flushTask.ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Disconnects the Dofus session asynchronously.
    /// </summary>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public Task DisconnectAsync()
    {
        return _cts.CancelAsync();
    }
    
    /// <inheritdoc/>
    public void Dispose()
    {
        if (_disposed)
            return;

        _disposed = true;

        if (!_cts.IsCancellationRequested)
            _cts.Cancel();

        try
        {
            socket.Shutdown(SocketShutdown.Both);
        }
        catch (SocketException)
        {
            // ignored
        }

        socket.Close();
        socket.Dispose();
        _cts.Dispose();

        GC.SuppressFinalize(this);
    }
}