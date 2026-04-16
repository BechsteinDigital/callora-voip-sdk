using System.Net.Sockets;
using CalloraVoipSdk.Core.Infrastructure.Stun.Auth;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Persistent RFC 6062 TCP data connection bound via CONNECTION-BIND.
/// </summary>
internal sealed class TurnTcpDataConnection : IAsyncDisposable
{
    private readonly TcpClient _client;
    private readonly Stream _stream;

    /// <summary>
    /// Creates a bound data connection wrapper.
    /// </summary>
    public TurnTcpDataConnection(TcpClient client, Stream stream, StunCredentials? effectiveCredentials)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(stream);
        _client = client;
        _stream = stream;
        EffectiveCredentials = effectiveCredentials;
    }

    /// <summary>
    /// Effective credentials after optional auth challenge updates.
    /// </summary>
    public StunCredentials? EffectiveCredentials { get; }

    /// <summary>
    /// Sends raw bytes to the TURN server over this bound data connection.
    /// </summary>
    public async Task SendAsync(ReadOnlyMemory<byte> bytes, CancellationToken ct = default)
    {
        await _stream.WriteAsync(bytes, ct).ConfigureAwait(false);
        await _stream.FlushAsync(ct).ConfigureAwait(false);
    }

    /// <summary>
    /// Receives raw bytes from the bound data connection.
    /// </summary>
    public Task<int> ReceiveAsync(Memory<byte> buffer, CancellationToken ct = default)
        => _stream.ReadAsync(buffer, ct).AsTask();

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        try
        {
            _stream.Dispose();
        }
        catch (ObjectDisposedException)
        {
        }

        _client.Dispose();
        return ValueTask.CompletedTask;
    }
}
