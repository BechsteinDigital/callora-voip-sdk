namespace CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

/// <summary>
/// Parsed representation of an SDP <c>a=fingerprint</c> attribute (RFC 8122 / RFC 5763).
/// Used to convey the DTLS certificate fingerprint in SDP offer/answer.
/// </summary>
internal sealed class SdpFingerprint
{
    /// <summary>Hash algorithm token, e.g. <c>sha-256</c> or <c>sha-1</c>.</summary>
    public required string Algorithm { get; init; }

    /// <summary>Hex-encoded fingerprint value, colon-delimited, e.g. <c>AA:BB:CC:…</c>.</summary>
    public required string Value { get; init; }

    /// <summary>
    /// Tries to parse the attribute value that follows <c>a=fingerprint:</c>.
    /// Returns <see langword="null"/> on malformed input.
    /// </summary>
    public static SdpFingerprint? TryParse(string attrValue)
    {
        if (string.IsNullOrWhiteSpace(attrValue))
            return null;

        var space = attrValue.IndexOf(' ');
        if (space <= 0 || space == attrValue.Length - 1)
            return null;

        return new SdpFingerprint
        {
            Algorithm = attrValue[..space].Trim(),
            Value = attrValue[(space + 1)..].Trim()
        };
    }

    /// <summary>
    /// Serializes to the attribute value string (without the leading <c>a=fingerprint:</c>).
    /// </summary>
    public string Serialize() => $"{Algorithm} {Value}";
}
