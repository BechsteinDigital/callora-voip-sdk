using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Client;

/// <summary>
/// Result of a successful STUN Binding Request.
/// Contains the public endpoint of the local socket as observed by the STUN server.
/// </summary>
internal sealed class StunBindingResult
{
    /// <summary>
    /// The public IP endpoint (address + port) as seen by the STUN server.
    /// For clients behind NAT this reflects the translated address.
    /// </summary>
    public required IPEndPoint MappedEndPoint { get; init; }

    /// <summary>
    /// True when the address was obtained from an XOR-MAPPED-ADDRESS attribute (RFC 5389 §15.2),
    /// which is the preferred and ALG-safe form.
    /// False when falling back to the legacy MAPPED-ADDRESS attribute.
    /// </summary>
    public bool IsXorMapped { get; init; }
}
