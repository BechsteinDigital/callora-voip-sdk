namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Validates RFC 7635 ACCESS-TOKEN attributes and resolves the key used for MESSAGE-INTEGRITY.
/// Implementations encapsulate token format, decryption, replay checks, and key lookup.
/// </summary>
internal interface IStunAccessTokenValidator
{
    /// <summary>
    /// Validates an incoming ACCESS-TOKEN and returns the derived HMAC key when valid.
    /// </summary>
    /// <param name="accessToken">Opaque token bytes from ACCESS-TOKEN attribute.</param>
    /// <param name="username">USERNAME attribute value (kid in RFC 7635 deployments).</param>
    /// <param name="hmacKey">Resolved key for MESSAGE-INTEGRITY verification.</param>
    /// <returns>True when the token is valid and <paramref name="hmacKey"/> is resolved.</returns>
    bool TryResolveHmacKey(ReadOnlyMemory<byte> accessToken, string username, out byte[] hmacKey);
}
