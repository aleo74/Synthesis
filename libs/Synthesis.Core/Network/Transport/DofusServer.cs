using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;
using Synthesis.Core.Network.Dispatcher;
using Synthesis.Core.Network.Framing;

namespace Synthesis.Core.Network.Transport;

/// <summary>
/// Represents a Dofus server capable of handling multiple sessions over TCP connections.
/// </summary>
/// <typeparam name="TSession">The type of Dofus session associated with the server.</typeparam>
public abstract class DofusServer<TSession>(ILoggerFactory loggerFactory, IMessageDecoder decoder, IMessageEncoder encoder, IMessageDispatcher dispatcher)
    where TSession : DofusSession
{
    private readonly CancellationTokenSource _cts = new();
    private readonly ILogger<DofusServer<TSession>> _logger = loggerFactory.CreateLogger<DofusServer<TSession>>();
    private readonly Socket _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

    /// <summary>
    /// Starts the Dofus server and listens on the specified port(s) for incoming connections.
    /// </summary>
    /// <param name="ports">The port(s) to bind and listen for incoming connections.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    public async Task StartAsync(params int[] ports)
    {
        try
        {
            foreach (var port in ports)
                _socket.Bind(new IPEndPoint(IPAddress.Any, port));
        }
        catch (SocketException e) when (e.SocketErrorCode is SocketError.AddressAlreadyInUse)
        {
            _logger.LogError(e, "Failed to bind to port: {Message}", e.Message);
            return;
        }

        _socket.Listen();

        while (!_cts.IsCancellationRequested)
        {
            var socket = await _socket.AcceptAsync().ConfigureAwait(false);

            var session = CreateSession(socket, decoder, encoder, dispatcher);

            _ = OnSessionConnectedAsync(session)
                .ContinueWith(_ => session.StartAsync())
                .Unwrap()
                .ContinueWith(_ => session.Dispose())
                .ContinueWith(_ => OnSessionDisconnectedAsync(session))
                .Unwrap()
                .ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Creates a new Dofus session based on the provided socket, decoder, encoder, and dispatcher.
    /// </summary>
    /// <param name="socket">The <see cref="Socket"/> associated with the session.</param>
    /// <param name="decoder">The message decoder for the session.</param>
    /// <param name="encoder">The message encoder for the session.</param>
    /// <param name="dispatcher">The message dispatcher for the session.</param>
    /// <returns>An instance of the Dofus session.</returns>
    protected abstract TSession CreateSession(Socket socket, IMessageDecoder decoder, IMessageEncoder encoder, IMessageDispatcher dispatcher);

    /// <summary>
    /// Invoked when a new Dofus session is successfully connected.
    /// </summary>
    /// <param name="session">The connected Dofus session.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task OnSessionConnectedAsync(TSession session);

    /// <summary>
    /// Invoked when a Dofus session is disconnected.
    /// </summary>
    /// <param name="session">The disconnected Dofus session.</param>
    /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
    protected abstract Task OnSessionDisconnectedAsync(TSession session);
}