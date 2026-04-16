namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN NONCE attribute (RFC 5389 §15.8).
/// Carries a server-generated, opaque nonce value for replay protection in the
/// long-term credential mechanism.
/// The server issues a nonce in 401 challenges; the client echoes it back in
/// subsequent authenticated requests. The server SHOULD limit nonce lifetime to
/// prevent indefinite reuse.
/// </summary>
internal sealed class NonceAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.Nonce;

    /// <summary>The opaque nonce string as issued by the server (less than 128 characters).</summary>
    public required string Value { get; init; }
}
