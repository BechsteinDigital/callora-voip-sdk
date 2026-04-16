namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN FINGERPRINT attribute (RFC 5389 §15.5).
/// Contains the CRC32 of all preceding message bytes XOR'd with 0x5354554E ("STUN" in ASCII).
/// Allows multiplexed receivers (e.g. RTP demultiplexers) to identify STUN packets without
/// inspecting the full message.
/// On encoding this attribute is always computed by <see cref="Wire.StunMessageCodec"/>;
/// any pre-existing instance in a message's attribute list is ignored during encode.
/// </summary>
internal sealed class FingerprintAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.Fingerprint;

    /// <summary>
    /// The raw fingerprint value stored on the wire (CRC32 XOR 0x5354554E).
    /// To recover the plain CRC32: <c>Value ^ 0x5354554E</c>.
    /// </summary>
    public uint Value { get; init; }
}
