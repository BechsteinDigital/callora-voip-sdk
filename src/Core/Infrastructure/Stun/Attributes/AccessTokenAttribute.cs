namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN ACCESS-TOKEN attribute (RFC 7635 §6.2).
/// Contains an opaque third-party authorization token issued by an authorization server.
/// The STUN client must not inspect token contents.
/// </summary>
internal sealed class AccessTokenAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.AccessToken;

    /// <summary>
    /// Raw token bytes as carried on the wire.
    /// </summary>
    public ReadOnlyMemory<byte> Token { get; init; }
}
