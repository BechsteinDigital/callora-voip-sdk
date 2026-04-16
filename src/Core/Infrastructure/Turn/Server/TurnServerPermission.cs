using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Permission entry bound to a single peer endpoint.
/// </summary>
internal sealed class TurnServerPermission
{
    /// <summary>Permitted peer endpoint.</summary>
    public required IPEndPoint PeerEndPoint { get; init; }

    /// <summary>Permission expiration timestamp (UTC).</summary>
    public required DateTimeOffset ExpiresAtUtc { get; set; }
}
