namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN USERNAME attribute (RFC 5389 §15.3).
/// Identifies the user for short-term or long-term credential authentication.
/// In ICE short-term credentials the value is the concatenation of the local and remote
/// ICE ufrag values separated by a colon (e.g. "localFrag:remoteFrag").
/// </summary>
internal sealed class UsernameAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.Username;

    /// <summary>The UTF-8 username string.</summary>
    public required string Value { get; init; }
}
