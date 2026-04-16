namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// ICE ICE-CONTROLLING attribute (RFC 8445 §16.1).
/// Comprehension-required attribute included in ICE connectivity check Binding Requests
/// sent by the controlling agent.
/// Carries a 64-bit random tiebreaker value generated at the start of the ICE session.
/// When both agents believe they are controlling (role conflict), the agent with the
/// higher tiebreaker keeps the controlling role; the other switches to controlled.
/// Wire format: 8-byte unsigned integer, big-endian.
/// </summary>
internal sealed class IceControllingAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.IceControlling;

    /// <summary>The 64-bit random tiebreaker value for role conflict resolution.</summary>
    public required ulong TieBreaker { get; init; }
}
