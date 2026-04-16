namespace CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

/// <summary>
/// Represents one SDP <c>a=crypto</c> attribute (RFC 4568 §9.1, SDES key management).
/// </summary>
internal sealed class SdpCryptoAttribute
{
    /// <summary>Crypto context tag (unique per media section).</summary>
    public required int Tag { get; init; }

    /// <summary>
    /// Cryptographic suite identifier, e.g. <c>AES_CM_128_HMAC_SHA1_80</c>.
    /// </summary>
    public required string CryptoSuite { get; init; }

    /// <summary>
    /// Key parameter string, e.g. <c>inline:base64key==</c>.
    /// </summary>
    public required string KeyParams { get; init; }

    /// <summary>
    /// Optional session parameter string (lifetime, MKI, etc.).
    /// </summary>
    public string? SessionParams { get; init; }

    /// <summary>
    /// Tries to parse a crypto value string (<c>tag SP suite SP key-params [SP session-params]</c>).
    /// Returns <see langword="null"/> on malformed input.
    /// </summary>
    public static SdpCryptoAttribute? TryParse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        // Split into at most 4 parts: tag, suite, key-params, session-params
        var parts = value.Split(' ', 4, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length < 3)
            return null;

        if (!int.TryParse(parts[0], out var tag))
            return null;

        return new SdpCryptoAttribute
        {
            Tag = tag,
            CryptoSuite = parts[1],
            KeyParams = parts[2],
            SessionParams = parts.Length > 3 ? parts[3] : null
        };
    }

    /// <summary>
    /// Serializes the attribute value (without the leading <c>a=crypto:</c>).
    /// </summary>
    public string Serialize()
    {
        var s = $"{Tag} {CryptoSuite} {KeyParams}";
        return SessionParams is not null ? $"{s} {SessionParams}" : s;
    }
}
