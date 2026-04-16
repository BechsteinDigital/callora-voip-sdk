using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Active TCP/TLS TURN client connection with serialized write access.
/// </summary>
internal sealed class TurnStreamConnection : IAsyncDisposable
{
    private readonly SemaphoreSlim _writeGate = new(1, 1);

    /// <summary>
    /// Connection identifier.
    /// </summary>
    public Guid Id { get; } = Guid.NewGuid();

    /// <summary>
    /// Remote endpoint of the connected client.
    /// </summary>
    public required IPEndPoint RemoteEndPoint { get; init; }

    /// <summary>
    /// Transport kind associated with this connection.
    /// </summary>
    public required TurnServerTransport Transport { get; init; }

    /// <summary>
    /// Underlying stream (TCP or TLS).
    /// </summary>
    public required Stream Stream { get; init; }

    /// <summary>
    /// Sends bytes atomically with respect to other writers.
    /// </summary>
    public async Task SendAsync(ReadOnlyMemory<byte> bytes, CancellationToken ct)
    {
        await _writeGate.WaitAsync(ct).ConfigureAwait(false);
        try
        {
            await Stream.WriteAsync(bytes, ct).ConfigureAwait(false);
        }
        finally
        {
            _writeGate.Release();
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _writeGate.Dispose();
        return ValueTask.CompletedTask;
    }
}
