namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Supported encryption algorithms for RFC 7635 self-contained access tokens.
/// </summary>
internal enum StunThirdPartyTokenEncryptionAlgorithm
{
    /// <summary>
    /// AEAD AES-256-GCM (RFC 7635 §4.1.1, mandatory to implement).
    /// </summary>
    Aes256Gcm
}
