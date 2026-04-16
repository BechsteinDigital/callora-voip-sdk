using CalloraVoipSdk.Core.Infrastructure.Stun.Server;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Authentication dependencies used by the TURN server.
/// </summary>
internal sealed class TurnAuthOptions
{
    /// <summary>
    /// Default realm returned in 401 challenges.
    /// </summary>
    public required string Realm { get; init; }

    /// <summary>
    /// Credential provider used for USERNAME/REALM lookup.
    /// </summary>
    public required IStunCredentialProvider CredentialProvider { get; init; }

    /// <summary>
    /// Nonce manager used for challenge issuance and stale nonce checks.
    /// </summary>
    public required IStunNonceManager NonceManager { get; init; }
}
