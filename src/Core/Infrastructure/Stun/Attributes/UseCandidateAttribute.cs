namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// ICE USE-CANDIDATE attribute (RFC 8445 §16.1).
/// Comprehension-required flag attribute included in ICE connectivity check Binding Requests
/// by the controlling agent to nominate a candidate pair.
/// This attribute carries no value (wire length = 0); its presence alone is the signal.
/// </summary>
internal sealed class UseCandidateAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.UseCandidate;
}
