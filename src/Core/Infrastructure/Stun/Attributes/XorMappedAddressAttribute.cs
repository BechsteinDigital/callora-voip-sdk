using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN XOR-MAPPED-ADDRESS attribute (RFC 5389 §15.2).
/// Contains the XOR-obfuscated public endpoint of the client as observed by the STUN server.
/// XOR obfuscation prevents Application Layer Gateways (ALGs) from silently rewriting the
/// embedded address, making this the preferred attribute over <see cref="MappedAddressAttribute"/>.
/// The codec de-XORs the address during decode, so <see cref="EndPoint"/> is always the real value.
/// </summary>
internal sealed class XorMappedAddressAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.XorMappedAddress;

    /// <summary>The resolved (de-XOR'd) public endpoint as observed by the STUN server.</summary>
    public required IPEndPoint EndPoint { get; init; }
}
