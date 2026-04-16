namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// Base class for all STUN attribute models.
/// Concrete subtypes represent specific attribute types.
/// Unrecognised attributes are stored as <see cref="UnknownRawAttribute"/>.
/// All serialisation is handled by <see cref="Wire.StunMessageCodec"/>.
/// </summary>
internal abstract class StunAttribute
{
    /// <summary>
    /// Typed attribute identifier.
    /// Unknown attributes report <see cref="StunAttributeType.Unknown"/>
    /// but preserve the original wire code via <see cref="RawAttributeType"/>.
    /// </summary>
    public abstract StunAttributeType AttributeType { get; }

    /// <summary>
    /// The raw 16-bit attribute type code as it appears on the wire.
    /// For known attributes this equals <c>(ushort)AttributeType</c>.
    /// For unknown attributes this preserves the original unrecognised code.
    /// </summary>
    public virtual ushort RawAttributeType => (ushort)AttributeType;
}
