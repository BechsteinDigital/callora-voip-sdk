using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN ALTERNATE-SERVER attribute (RFC 5389 §15.11).
/// Comprehension-optional attribute included in a 300 Try Alternate error response
/// to redirect the client to a different STUN server.
/// Wire format is identical to MAPPED-ADDRESS (non-XOR'd).
/// The client SHOULD retry the request to the indicated server, but MUST NOT
/// follow more than one redirect to prevent infinite loops.
/// </summary>
internal sealed class AlternateServerAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.AlternateServer;

    /// <summary>The endpoint of the alternate STUN server the client should contact.</summary>
    public required IPEndPoint EndPoint { get; init; }
}
