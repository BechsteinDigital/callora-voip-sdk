namespace CalloraVoipSdk.Core.Infrastructure.Stun.Attributes;

/// <summary>
/// STUN THIRD-PARTY-AUTHORIZATION attribute (RFC 7635 §6.1).
/// Signals support for third-party authorization and carries the STUN server name.
/// </summary>
internal sealed class ThirdPartyAuthorizationAttribute : StunAttribute
{
    /// <inheritdoc />
    public override StunAttributeType AttributeType => StunAttributeType.ThirdPartyAuthorization;

    /// <summary>
    /// STUN server name that the client must present to its authorization server.
    /// </summary>
    public required string ServerName { get; init; }
}
