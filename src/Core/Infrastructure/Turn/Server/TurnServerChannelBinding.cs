using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Channel binding entry linking a channel number to a peer endpoint.
/// </summary>
internal sealed class TurnServerChannelBinding
{
    /// <summary>Channel number (0x4000..0x7FFF).</summary>
    public required ushort ChannelNumber { get; init; }

    /// <summary>Peer endpoint for the channel.</summary>
    public required IPEndPoint PeerEndPoint { get; init; }

    /// <summary>Binding expiration timestamp (UTC).</summary>
    public required DateTimeOffset ExpiresAtUtc { get; set; }
}
