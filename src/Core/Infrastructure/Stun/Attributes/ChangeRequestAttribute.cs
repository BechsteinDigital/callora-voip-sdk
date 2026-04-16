namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// RFC 3489 CHANGE-REQUEST attribute (type 0x0003).
/// Sent by legacy RFC 3489 clients to ask the server to respond from a different IP address
/// and/or port, enabling classic NAT behaviour discovery.
/// <para>
/// This attribute is deprecated in RFC 5389.
/// RFC 5389-compliant servers MUST NOT support CHANGE-REQUEST behaviour (they cannot
/// respond from arbitrary addresses), and SHOULD return a 400 Bad Request error response
/// to any request that includes this attribute (§12).
/// The attribute is decoded here to allow the server handler to detect and reject it
/// explicitly rather than treating it as an unknown comprehension-required attribute
/// and returning a misleading 420 error.
/// </para>
/// Wire format: 4-byte flags field. Bit 2 (0x04) = change IP; bit 1 (0x02) = change port.
/// </summary>
internal sealed class ChangeRequestAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.ChangeRequest;

    /// <summary>
    /// True when the client requests the server to respond from a different IP address.
    /// Always unsupported in RFC 5389 servers.
    /// </summary>
    public bool ChangeIp { get; init; }

    /// <summary>
    /// True when the client requests the server to respond from a different port.
    /// Always unsupported in RFC 5389 servers.
    /// </summary>
    public bool ChangePort { get; init; }
}
