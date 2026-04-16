using CalloraVoipSdk.Core.Infrastructure.Stun.Auth;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Result of RFC 6062 CONNECT request.
/// </summary>
internal sealed class TurnConnectResult
{
    /// <summary>
    /// TURN CONNECTION-ID returned by the server.
    /// </summary>
    public required uint ConnectionId { get; init; }

    /// <summary>
    /// Effective credentials after auth challenge updates.
    /// </summary>
    public StunCredentials? EffectiveCredentials { get; init; }
}
