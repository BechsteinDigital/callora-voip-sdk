using System.Net;
using System.Net.WebSockets;
using System.Text;
using Microsoft.Extensions.Logging;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

/// <summary>
/// Represents one SIP-over-WebSocket connection with message receive loop and serialized sends.
/// </summary>
internal sealed class SipWebSocketConnection : IDisposable
{
    private readonly WebSocket _socket;
    private readonly ILogger _logger;
    private readonly Func<IPEndPoint, SipTransportProtocol, ReadOnlyMemory<byte>, Task> _onFrameAsync;
    private readonly Action _onClosed;
    private readonly CancellationTokenSource _stop = new();
    private readonly SemaphoreSlim _sendGate = new(1, 1);
    private readonly Task _receiveLoop;
    private int _disposed;

    /// <summary>
    /// Creates one WebSocket connection wrapper and starts receive loop.
    /// </summary>
    public SipWebSocketConnection(
        SipTransportProtocol protocol,
        WebSocket socket,
        IPEndPoint remoteEndPoint,
        ILogger logger,
        Func<IPEndPoint, SipTransportProtocol, ReadOnlyMemory<byte>, Task> onFrameAsync,
        Action onClosed)
    {
        Protocol = protocol;
        _socket = socket ?? throw new ArgumentNullException(nameof(socket));
        RemoteEndPoint = remoteEndPoint ?? throw new ArgumentNullException(nameof(remoteEndPoint));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _onFrameAsync = onFrameAsync ?? throw new ArgumentNullException(nameof(onFrameAsync));
        _onClosed = onClosed ?? throw new ArgumentNullException(nameof(onClosed));

        _receiveLoop = Task.Run(() => ReceiveLoopAsync(_stop.Token));
    }

    /// <summary>
    /// Remote endpoint associated with this WebSocket transport.
    /// </summary>
    public IPEndPoint RemoteEndPoint { get; }

    /// <summary>
    /// Effective SIP transport protocol (WS or WSS).
    /// </summary>
    public SipTransportProtocol Protocol { get; }

    /// <summary>
    /// Sends one SIP message as WebSocket text frame.
    /// </summary>
    public async Task SendAsync(ReadOnlyMemory<byte> payload, CancellationToken ct)
    {
        if (payload.Length == 0)
            return;

        await _sendGate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            if (_socket.State is not (WebSocketState.Open or WebSocketState.CloseReceived))
                throw new IOException("WebSocket is not open.");

            await _socket.SendAsync(payload, WebSocketMessageType.Text, endOfMessage: true, ct).ConfigureAwait(false);
        }
        finally
        {
            _sendGate.Release();
        }
    }

    /// <summary>
    /// Receive loop reading complete WS messages and forwarding them as SIP payloads.
    /// </summary>
    private async Task ReceiveLoopAsync(CancellationToken ct)
    {
        var buffer = new byte[8192];
        var aggregate = new MemoryStream();
        try
        {
            while (!ct.IsCancellationRequested && _socket.State is WebSocketState.Open or WebSocketState.CloseReceived)
            {
                var result = await _socket.ReceiveAsync(buffer, ct).ConfigureAwait(false);
                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                if (result.Count > 0)
                    await aggregate.WriteAsync(buffer.AsMemory(0, result.Count), ct).ConfigureAwait(false);
                if (!result.EndOfMessage)
                    continue;

                var payload = aggregate.ToArray();
                aggregate.SetLength(0);
                if (payload.Length == 0)
                    continue;

                await _onFrameAsync(RemoteEndPoint, Protocol, payload).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException)
        {
            // expected on dispose
        }
        catch (ObjectDisposedException)
        {
            // expected on dispose
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "SIP WebSocket receive loop failed for {Remote}.", RemoteEndPoint);
        }
        finally
        {
            aggregate.Dispose();
            _onClosed();
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        _stop.Cancel();
        try
        {
            _receiveLoop.GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "SIP WebSocket loop ended with exception during disposal.");
        }

        try
        {
            _socket.Abort();
            _socket.Dispose();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "SIP WebSocket dispose failed for {Remote}.", RemoteEndPoint);
        }

        _sendGate.Dispose();
        _stop.Dispose();
    }
}

