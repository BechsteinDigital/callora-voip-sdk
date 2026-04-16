using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN XOR-PEER-ADDRESS attribute model (RFC 8656 §18.3).
/// </summary>
internal sealed class TurnXorPeerAddressAttribute
{
    /// <summary>
    /// Peer endpoint decoded from the XOR-encoded wire representation.
    /// </summary>
    public required IPEndPoint EndPoint { get; init; }
}
