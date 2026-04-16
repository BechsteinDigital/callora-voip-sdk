namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN SOFTWARE attribute (RFC 5389 §15.10).
/// Comprehension-optional attribute carrying a human-readable description of the software
/// generating the message. Useful for diagnostics and interoperability testing.
/// The value SHOULD include manufacturer and version information.
/// </summary>
internal sealed class SoftwareAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.Software;

    /// <summary>UTF-8 software description string (up to 763 bytes on the wire).</summary>
    public required string Description { get; init; }
}
