namespace CalloraVoipSdk.Core.Infrastructure.Stun.Client;

/// <summary>
/// Internal exception used within the STUN client to surface 401 / 438 error responses
/// as part of the long-term credential challenge/response flow (RFC 5389 §10.2).
/// This type is never exposed to callers of <see cref="IStunClient"/>; the client catches
/// it internally and retries with updated credentials.
/// </summary>
internal sealed class StunChallengeException : StunException
{
    /// <summary>STUN error code (401 Unauthorized or 438 Stale Nonce).</summary>
    public int ErrorCode { get; }

    /// <summary>Authentication realm from the server's challenge, or <c>null</c> if absent.</summary>
    public string? Realm { get; }

    /// <summary>Server-issued nonce from the challenge, or <c>null</c> if absent.</summary>
    public string? Nonce { get; }

    /// <summary>
    /// Initialises the exception with the error code, reason phrase, and optional challenge attributes.
    /// </summary>
    public StunChallengeException(int errorCode, string reason, string? realm, string? nonce)
        : base($"STUN challenge {errorCode}: {reason}")
    {
        ErrorCode = errorCode;
        Realm     = realm;
        Nonce     = nonce;
    }
}
