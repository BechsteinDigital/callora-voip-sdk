namespace CalloraVoipSdk.Core.Infrastructure.Stun.Auth;

/// <summary>
/// Immutable STUN credential set for short-term or long-term credential mechanisms (RFC 5389 §10).
/// <para>
/// Short-term credentials (ICE): provide <see cref="Username"/> and <see cref="Password"/> only.
/// HMAC key = SASLprep(password) as UTF-8 bytes.
/// </para>
/// <para>
/// Long-term credentials (TURN): provide <see cref="Username"/>, <see cref="Password"/>,
/// and <see cref="Realm"/>. HMAC key = MD5(username ":" realm ":" SASLprep(password)).
/// </para>
/// </summary>
internal sealed class StunCredentials
{
    /// <summary>The username (ICE ufrag, TURN username, etc.).</summary>
    public required string Username { get; init; }

    /// <summary>The password in clear text (will be SASLprep'd during key derivation).</summary>
    public required string Password { get; init; }

    /// <summary>
    /// Authentication realm for long-term credentials.
    /// When non-null, long-term key derivation (MD5) is used instead of short-term.
    /// </summary>
    public string? Realm { get; init; }

    /// <summary>
    /// Server-issued nonce for long-term credentials.
    /// Must be echoed back in authenticated requests; ignored for short-term credentials.
    /// </summary>
    public string? Nonce { get; init; }

    /// <summary>True when these are long-term credentials (Realm is present).</summary>
    public bool IsLongTerm => Realm is not null;

    /// <summary>
    /// Derives the HMAC-SHA1 key for MESSAGE-INTEGRITY computation and verification.
    /// </summary>
    public byte[] DeriveHmacKey() =>
        IsLongTerm
            ? StunKeyDerivation.LongTermKey(Username, Realm!, Password)
            : StunKeyDerivation.ShortTermKey(Password);

    /// <summary>
    /// Returns a new <see cref="StunCredentials"/> with the given realm and nonce applied,
    /// switching the credential set to long-term mode if not already.
    /// Used during the long-term credential challenge/response flow (RFC 5389 §10.2).
    /// </summary>
    public StunCredentials WithRealmAndNonce(string realm, string nonce)
        => new() { Username = Username, Password = Password, Realm = realm, Nonce = nonce };

    /// <summary>
    /// Returns a new <see cref="StunCredentials"/> with only the nonce replaced.
    /// Used when the server issues a 438 Stale Nonce response with a fresh nonce.
    /// </summary>
    public StunCredentials WithNonce(string nonce)
        => new() { Username = Username, Password = Password, Realm = Realm, Nonce = nonce };
}
