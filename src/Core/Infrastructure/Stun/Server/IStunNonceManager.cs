namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Manages server-side STUN nonces for the long-term credential mechanism (RFC 5389 §10.2.2).
/// <para>
/// The server generates a cryptographically random nonce and includes it in every 401 challenge.
/// When the client retries with that nonce, the server verifies it is still within its validity
/// window. Expired or unknown nonces trigger a 438 Stale Nonce response so the client can
/// obtain a fresh nonce and retry.
/// </para>
/// </summary>
internal interface IStunNonceManager
{
    /// <summary>
    /// Generates a fresh nonce, registers it internally with an expiry, and returns the value.
    /// The nonce must be included in 401 Unauthorized challenge responses.
    /// </summary>
    string GenerateNonce();

    /// <summary>
    /// Returns true when <paramref name="nonce"/> was issued by this manager and has not yet expired.
    /// Returns false for unknown, expired, or malformed nonce values (treating all as stale).
    /// </summary>
    bool IsNonceValid(string nonce);
}
