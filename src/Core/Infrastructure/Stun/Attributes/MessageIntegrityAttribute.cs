namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN MESSAGE-INTEGRITY attribute (RFC 5389 §15.4).
/// Contains the HMAC-SHA1 over all message bytes preceding this attribute,
/// with the header length field adjusted to include this attribute before hashing.
/// On encoding this attribute is always computed by <see cref="Wire.StunMessageCodec"/>;
/// any pre-existing instance in a message's attribute list is ignored during encode.
/// On decode the raw HMAC bytes are surfaced here for optional verification.
/// </summary>
internal sealed class MessageIntegrityAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.MessageIntegrity;

    /// <summary>Raw 20-byte HMAC-SHA1 value as decoded from the wire.</summary>
    public required ReadOnlyMemory<byte> Hmac { get; init; }
}
