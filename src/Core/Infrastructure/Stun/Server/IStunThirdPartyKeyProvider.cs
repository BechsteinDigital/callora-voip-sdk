namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Resolves shared keying material for RFC 7635 access-token validation.
/// </summary>
internal interface IStunThirdPartyKeyProvider
{
    /// <summary>
    /// Tries to resolve keying material for the given key id.
    /// In RFC 7635 deployments the key id is provided via STUN USERNAME.
    /// </summary>
    bool TryGetKeyMaterial(string keyId, out StunThirdPartyKeyMaterial keyMaterial);
}
