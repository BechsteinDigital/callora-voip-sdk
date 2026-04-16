using CalloraVoipSdk.Core.Infrastructure.Stun.Auth;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Result of a successful TURN Refresh transaction.
/// </summary>
internal sealed class TurnRefreshResult
{
    /// <summary>
    /// Updated allocation lifetime in seconds.
    /// </summary>
    public uint LifetimeSeconds { get; init; }

    /// <summary>
    /// Credentials including latest realm/nonce values for follow-up TURN requests.
    /// </summary>
    public StunCredentials? EffectiveCredentials { get; init; }

    /// <summary>
    /// RFC 8016 mobility ticket returned by the server after refresh, when provided.
    /// </summary>
    public byte[]? MobilityTicket { get; init; }
}
