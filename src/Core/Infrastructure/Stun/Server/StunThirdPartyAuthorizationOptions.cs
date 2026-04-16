namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Configuration for RFC 7635 third-party authorization on the STUN server.
/// </summary>
internal sealed class StunThirdPartyAuthorizationOptions
{
    /// <summary>
    /// STUN server name advertised in THIRD-PARTY-AUTHORIZATION (RFC 7635 §6.1).
    /// </summary>
    public required string ServerName { get; init; }

    /// <summary>
    /// Access-token validator used to verify ACCESS-TOKEN and derive MESSAGE-INTEGRITY keys.
    /// </summary>
    public required IStunAccessTokenValidator AccessTokenValidator { get; init; }
}
