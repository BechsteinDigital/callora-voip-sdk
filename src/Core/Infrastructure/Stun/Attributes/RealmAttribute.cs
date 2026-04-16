namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN REALM attribute (RFC 5389 §15.7).
/// Identifies the authentication realm for the long-term credential mechanism.
/// Sent by the server in 401 Unauthorized challenges and repeated by the client
/// in authenticated requests alongside the USERNAME and NONCE attributes.
/// Value is a UTF-8 string, MUST be less than 128 characters (SASLprep-prepared).
/// </summary>
internal sealed class RealmAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.Realm;

    /// <summary>The authentication realm string (UTF-8, SASLprep-prepared).</summary>
    public required string Value { get; init; }
}
