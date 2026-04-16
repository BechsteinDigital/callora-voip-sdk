namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN UNKNOWN-ATTRIBUTES attribute (RFC 5389 §15.9).
/// Included in 420 Unknown Attribute error responses to inform the requester
/// which comprehension-required attribute type codes were not understood.
/// Each code is a 16-bit value; the list is padded to a 4-byte boundary.
/// </summary>
internal sealed class UnknownAttributesAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.UnknownAttributes;

    /// <summary>
    /// The comprehension-required attribute type codes (range 0x0000–0x7FFF)
    /// that were not recognised by the receiver.
    /// </summary>
    public required IReadOnlyList<ushort> UnknownTypeCodes { get; init; }
}
