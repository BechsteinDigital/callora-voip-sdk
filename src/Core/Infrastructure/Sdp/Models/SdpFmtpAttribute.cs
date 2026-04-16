namespace CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

/// <summary>
/// Represents one SDP <c>a=fmtp</c> attribute (RFC 4566 §6.6).
/// Carries format-specific parameters for a given payload type.
/// </summary>
internal sealed class SdpFmtpAttribute
{
    /// <summary>
    /// RTP payload type this fmtp applies to.
    /// </summary>
    public required int PayloadType { get; init; }

    /// <summary>
    /// Raw parameter string (e.g. <c>0-16</c> for telephone-event, <c>annexb=no</c> for G.729).
    /// </summary>
    public required string Parameters { get; init; }

    /// <summary>
    /// Tries to parse one fmtp value string (<c>PT SP params</c>).
    /// Returns <see langword="null"/> on malformed input.
    /// </summary>
    public static SdpFmtpAttribute? TryParse(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var spaceIndex = value.IndexOf(' ');
        if (spaceIndex <= 0)
            return null;

        if (!int.TryParse(value[..spaceIndex].Trim(), out var payloadType))
            return null;

        var parameters = value[(spaceIndex + 1)..].Trim();
        return new SdpFmtpAttribute { PayloadType = payloadType, Parameters = parameters };
    }
}
