namespace CalloraVoipSdk.Core.Infrastructure.Srtp.Crypto;

/// <summary>
/// Master key and salt for one SRTP crypto context (RFC 3711 §3.2.1).
/// Passed in from SDP SDES negotiation (RFC 4568 §6.1).
/// </summary>
internal sealed class SrtpKeyMaterial
{
    /// <summary>
    /// Master key — 16 bytes for AES-128 suites, 32 bytes for AES-256 suites (RFC 3711 §3.2.1).
    /// </summary>
    public required ReadOnlyMemory<byte> MasterKey { get; init; }

    /// <summary>
    /// Master salt — always 14 bytes (112 bits) (RFC 3711 §3.2.1).
    /// </summary>
    public required ReadOnlyMemory<byte> MasterSalt { get; init; }

    /// <summary>
    /// Crypto suite that determines how this key material is used.
    /// </summary>
    public required SrtpCryptoSuite Suite { get; init; }

    /// <summary>
    /// Parses a base64-encoded SDES key-param string into key material.
    /// Format: "inline:&lt;base64(key+salt)&gt;" (RFC 4568 §6.1).
    /// </summary>
    public static SrtpKeyMaterial ParseInline(string keyParam, SrtpCryptoSuite suite)
    {
        ArgumentNullException.ThrowIfNull(keyParam);

        const string prefix = "inline:";
        if (!keyParam.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
            throw new FormatException($"SRTP key-param must start with 'inline:' but was '{keyParam}'.");

        var raw = Convert.FromBase64String(keyParam[prefix.Length..].Split('|')[0]);

        var keyLength = suite is SrtpCryptoSuite.AesCm256HmacSha1_80 or SrtpCryptoSuite.AesCm256HmacSha1_32
            ? 32 : 16;
        const int saltLength = 14;

        if (raw.Length < keyLength + saltLength)
            throw new FormatException(
                $"SRTP inline key too short: {raw.Length} bytes, expected at least {keyLength + saltLength}.");

        return new SrtpKeyMaterial
        {
            MasterKey  = raw[..keyLength],
            MasterSalt = raw[keyLength..(keyLength + saltLength)],
            Suite      = suite,
        };
    }
}
