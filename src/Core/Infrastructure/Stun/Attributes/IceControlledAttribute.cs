namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// ICE ICE-CONTROLLED attribute (RFC 8445 §16.1).
/// Comprehension-required attribute included in ICE connectivity check Binding Requests
/// sent by the controlled agent.
/// Carries a 64-bit random tiebreaker value generated at the start of the ICE session.
/// When both agents believe they are controlled (role conflict), the agent with the
/// lower tiebreaker switches to controlling.
/// Wire format: 8-byte unsigned integer, big-endian.
/// </summary>
internal sealed class IceControlledAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.IceControlled;

    /// <summary>The 64-bit random tiebreaker value for role conflict resolution.</summary>
    public required ulong TieBreaker { get; init; }
}
