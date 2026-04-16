using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN MAPPED-ADDRESS attribute (RFC 5389 §15.1).
/// Contains the public IP endpoint of the client as observed by the STUN server.
/// Prefer <see cref="XorMappedAddressAttribute"/> where available; this attribute
/// is kept for compatibility with legacy STUN servers.
/// </summary>
internal sealed class MappedAddressAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.MappedAddress;

    /// <summary>The public endpoint as observed by the STUN server.</summary>
    public required IPEndPoint EndPoint { get; init; }
}
