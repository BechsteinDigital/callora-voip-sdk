using System.Net;
using System.Net.Sockets;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Pending RFC 6062 TCP peer connection created by CONNECT and awaiting CONNECTION-BIND.
/// </summary>
internal sealed class TurnTcpPendingConnection : IAsyncDisposable
{
    /// <summary>
    /// TURN CONNECTION-ID assigned to this pending connection.
    /// </summary>
    public required uint ConnectionId { get; init; }

    /// <summary>
    /// Allocation key that initiated this peer connection.
    /// </summary>
    public required string AllocationKey { get; init; }

    /// <summary>
    /// Peer endpoint connected by the TURN server.
    /// </summary>
    public required IPEndPoint PeerEndPoint { get; init; }

    /// <summary>
    /// Underlying TCP client socket to the peer.
    /// </summary>
    public required TcpClient PeerClient { get; init; }

    /// <summary>
    /// Read/write stream for peer data relay.
    /// </summary>
    public required Stream PeerStream { get; init; }

    /// <summary>
    /// Creation timestamp in UTC for stale cleanup.
    /// </summary>
    public required DateTimeOffset CreatedAtUtc { get; init; }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        try
        {
            PeerClient.Dispose();
        }
        catch (ObjectDisposedException)
        {
        }

        return ValueTask.CompletedTask;
    }
}
