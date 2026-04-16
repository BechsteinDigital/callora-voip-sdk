namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// Passthrough STUN attribute for unrecognised type codes.
/// Preserves both the raw wire type code and the raw value bytes so that
/// decoders can forward unknown comprehension-optional attributes without discarding them.
/// Comprehension-required unknown attributes (codes 0x0000–0x7FFF) should cause an error response;
/// comprehension-optional unknowns (0x8000–0xFFFF) may be silently ignored.
/// </summary>
internal sealed class UnknownRawAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.Unknown;

    /// <inheritdoc />
    public override ushort RawAttributeType { get; }

    /// <summary>Raw value bytes as decoded from the wire (excludes the 4-byte attribute header).</summary>
    public required ReadOnlyMemory<byte> Value { get; init; }

    /// <summary>Initialises an unknown attribute preserving its raw wire type code.</summary>
    public UnknownRawAttribute(ushort rawType)
    {
        RawAttributeType = rawType;
    }
}
