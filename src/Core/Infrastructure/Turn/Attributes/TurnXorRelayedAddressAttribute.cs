using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN XOR-RELAYED-ADDRESS attribute model (RFC 8656 §18.5).
/// </summary>
internal sealed class TurnXorRelayedAddressAttribute
{
    /// <summary>
    /// Relayed endpoint allocated on the TURN server.
    /// </summary>
    public required IPEndPoint EndPoint { get; init; }
}
