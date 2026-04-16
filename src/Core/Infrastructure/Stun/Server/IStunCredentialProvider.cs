using CalloraVoipSdk.Core.Infrastructure.Stun.Auth;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Resolves STUN credentials for inbound authenticated requests.
/// Enables multi-user deployments where credentials are selected per USERNAME/REALM tuple.
/// </summary>
internal interface IStunCredentialProvider
{
    /// <summary>
    /// Tries to resolve credentials for the supplied username and optional realm.
    /// </summary>
    /// <param name="username">USERNAME attribute value from the STUN request.</param>
    /// <param name="realm">
    /// REALM attribute value from the STUN request, if present.
    /// Short-term requests typically pass null.
    /// </param>
    /// <param name="credentials">Resolved credential set on success.</param>
    /// <returns>True when a matching credential set was found; otherwise false.</returns>
    bool TryGetCredentials(string username, string? realm, out StunCredentials credentials);
}
