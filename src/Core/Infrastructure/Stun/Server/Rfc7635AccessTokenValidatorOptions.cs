namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Configuration for <see cref="Rfc7635AccessTokenValidator"/>.
/// </summary>
internal sealed class Rfc7635AccessTokenValidatorOptions
{
    /// <summary>
    /// STUN server name used as associated data (A) during token decryption (RFC 7635 §6.2).
    /// </summary>
    public required string ServerName { get; init; }

    /// <summary>
    /// Provider that resolves keying material by key id (USERNAME/kid).
    /// </summary>
    public required IStunThirdPartyKeyProvider KeyProvider { get; init; }

    /// <summary>
    /// Allowed clock skew Delta for replay/lifetime validation.
    /// RFC 7635 recommends 5 seconds.
    /// </summary>
    public TimeSpan AllowedClockSkew { get; init; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Maximum accepted access-token size in bytes after optional Base64 decoding.
    /// </summary>
    public int MaxTokenSizeBytes { get; init; } = 4096;

    /// <summary>
    /// When true, ASCII Base64 tokens are decoded before RFC 7635 parsing.
    /// </summary>
    public bool AcceptBase64EncodedTokens { get; init; } = true;

    /// <summary>
    /// Optional clock override for deterministic tests.
    /// </summary>
    public Func<DateTimeOffset>? UtcNow { get; init; }
}
