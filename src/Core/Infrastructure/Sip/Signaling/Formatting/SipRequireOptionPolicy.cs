using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Validates SIP Require option tags against supported feature set.
/// </summary>
internal static class SipRequireOptionPolicy
{
    private static readonly HashSet<string> SupportedOptionTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "100rel",
        "timer",
        "replaces",
        "norefersub"   // RFC 4488: suppresses implicit REFER subscription when Refer-Sub: false
    };

    /// <summary>
    /// Returns true when all Require option tags are supported.
    /// When false, unsupportedHeaderValue contains a comma-separated Unsupported header value.
    /// </summary>
    public static bool TryValidateRequireHeader(
        string? requireHeader,
        out string unsupportedHeaderValue)
    {
        unsupportedHeaderValue = string.Empty;
        if (string.IsNullOrWhiteSpace(requireHeader))
            return true;

        var unsupported = new List<string>();
        foreach (var token in requireHeader.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
        {
            var normalized = token.Trim().Trim('"');
            if (normalized.Length == 0)
                continue;
            if (SupportedOptionTags.Contains(normalized))
                continue;
            if (unsupported.Contains(normalized, StringComparer.OrdinalIgnoreCase))
                continue;

            unsupported.Add(normalized);
        }

        if (unsupported.Count == 0)
            return true;

        unsupportedHeaderValue = string.Join(", ", unsupported);
        return false;
    }

    /// <summary>
    /// Returns true when all Require option tags on one SIP request are supported.
    /// </summary>
    public static bool TryValidateRequestRequireHeader(
        SipRequest request,
        out string unsupportedHeaderValue)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (request.Headers.TryGetValue("Require", out var rawRequireValue))
            return TryValidateRequireHeader(rawRequireValue, out unsupportedHeaderValue);

        return TryValidateRequireHeader(request.Header("Require"), out unsupportedHeaderValue);
    }

    /// <summary>
    /// Returns true when all INVITE Require tags are supported.
    /// </summary>
    public static bool TryValidateInviteRequireHeader(
        string? requireHeader,
        out string unsupportedHeaderValue) =>
        TryValidateRequireHeader(requireHeader, out unsupportedHeaderValue);
}
