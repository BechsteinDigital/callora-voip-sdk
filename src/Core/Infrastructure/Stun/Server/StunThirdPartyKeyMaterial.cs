namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Keying material shared between authorization server and STUN server for RFC 7635 token validation.
/// </summary>
internal sealed class StunThirdPartyKeyMaterial
{
    /// <summary>
    /// Symmetric key K used to decrypt and authenticate the self-contained token.
    /// For <see cref="StunThirdPartyTokenEncryptionAlgorithm.Aes256Gcm"/> this must be 32 bytes.
    /// </summary>
    public required byte[] SymmetricKey { get; init; }

    /// <summary>
    /// Encryption algorithm used for this key id.
    /// </summary>
    public StunThirdPartyTokenEncryptionAlgorithm EncryptionAlgorithm { get; init; }
        = StunThirdPartyTokenEncryptionAlgorithm.Aes256Gcm;

    /// <summary>
    /// GCM tag size in bytes for Aes256Gcm tokens.
    /// </summary>
    public int GcmTagSizeBytes { get; init; } = 16;
}
