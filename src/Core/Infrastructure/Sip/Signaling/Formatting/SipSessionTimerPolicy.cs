using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Session timer helpers for RFC4028-style header normalization and validation.
/// </summary>
internal static class SipSessionTimerPolicy
{
    /// <summary>
    /// Default session refresh interval used when peer does not request one explicitly.
    /// </summary>
    public const int DefaultSessionExpiresSeconds = 1800;

    /// <summary>
    /// Minimum acceptable session interval.
    /// </summary>
    public const int MinSessionExpiresSeconds = 90;

    /// <summary>
    /// Applies outbound session-timer offer headers for INVITE/UPDATE refresh capability.
    /// </summary>
    public static void ApplyOutboundOfferHeaders(IDictionary<string, string> headers)
    {
        headers["Supported"] = AppendToken(headers.TryGetValue("Supported", out var existingSupported) ? existingSupported : null, "timer");
        headers["Session-Expires"] = $"{DefaultSessionExpiresSeconds};refresher=uac";
        headers["Min-SE"] = MinSessionExpiresSeconds.ToString();
    }

    /// <summary>
    /// Validates inbound request session interval and returns normalized response value.
    /// </summary>
    public static bool TryValidateInboundRequest(
        SipRequest request,
        out int rejectionStatusCode,
        out string rejectionReasonPhrase,
        out string normalizedSessionExpiresValue)
    {
        rejectionStatusCode = 0;
        rejectionReasonPhrase = string.Empty;
        normalizedSessionExpiresValue = $"{DefaultSessionExpiresSeconds};refresher=uas";

        var sessionExpires = request.Header("Session-Expires");
        if (string.IsNullOrWhiteSpace(sessionExpires))
            return true;

        if (!TryParseSessionExpires(sessionExpires, out var intervalSeconds))
        {
            rejectionStatusCode = 400;
            rejectionReasonPhrase = "Bad Request";
            return false;
        }

        if (intervalSeconds < MinSessionExpiresSeconds)
        {
            rejectionStatusCode = 422;
            rejectionReasonPhrase = "Session Interval Too Small";
            return false;
        }

        normalizedSessionExpiresValue = $"{intervalSeconds};refresher=uas";
        return true;
    }

    /// <summary>
    /// Applies response headers for successful session timer negotiation.
    /// </summary>
    public static void ApplyResponseHeaders(
        IDictionary<string, string> headers,
        string normalizedSessionExpiresValue)
    {
        headers["Supported"] = AppendToken(headers.TryGetValue("Supported", out var existingSupported) ? existingSupported : null, "timer");
        headers["Session-Expires"] = normalizedSessionExpiresValue;
        headers["Min-SE"] = MinSessionExpiresSeconds.ToString();
    }

    /// <summary>
    /// Applies Min-SE guidance headers for 422 response generation.
    /// </summary>
    public static void ApplyTooSmallResponseHeaders(IDictionary<string, string> headers)
    {
        headers["Min-SE"] = MinSessionExpiresSeconds.ToString();
        headers["Supported"] = AppendToken(headers.TryGetValue("Supported", out var existingSupported) ? existingSupported : null, "timer");
    }

    /// <summary>
    /// Resolves negotiated interval and refresher role from a Session-Expires header value.
    /// Returns false when no usable session timer header is present.
    /// </summary>
    public static bool TryResolveNegotiation(
        string? sessionExpiresHeader,
        bool localIsRequester,
        out int intervalSeconds,
        out bool localIsRefresher)
    {
        intervalSeconds = 0;
        localIsRefresher = false;
        if (string.IsNullOrWhiteSpace(sessionExpiresHeader))
            return false;

        if (!TryParseSessionExpires(sessionExpiresHeader, out intervalSeconds))
            return false;

        var refresherRole = TryParseRefresherRole(sessionExpiresHeader);
        localIsRefresher = refresherRole switch
        {
            "uac" => localIsRequester,
            "uas" => !localIsRequester,
            _ => localIsRequester
        };
        return intervalSeconds > 0;
    }

    /// <summary>
    /// Tries to parse Session-Expires interval from header value.
    /// </summary>
    private static bool TryParseSessionExpires(string headerValue, out int intervalSeconds)
    {
        intervalSeconds = 0;
        var trimmed = headerValue.Trim();
        var semicolonIndex = trimmed.IndexOf(';');
        var intervalText = semicolonIndex >= 0 ? trimmed[..semicolonIndex] : trimmed;
        return int.TryParse(intervalText.Trim(), out intervalSeconds) && intervalSeconds > 0;
    }

    /// <summary>
    /// Tries to parse refresher role parameter from Session-Expires value.
    /// </summary>
    private static string? TryParseRefresherRole(string headerValue)
    {
        var segments = headerValue.Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length <= 1)
            return null;

        for (var i = 1; i < segments.Length; i++)
        {
            var segment = segments[i];
            var equalsIndex = segment.IndexOf('=');
            if (equalsIndex <= 0)
                continue;

            var name = segment[..equalsIndex].Trim();
            if (!name.Equals("refresher", StringComparison.OrdinalIgnoreCase))
                continue;

            var value = segment[(equalsIndex + 1)..].Trim().Trim('"');
            if (value.Equals("uac", StringComparison.OrdinalIgnoreCase))
                return "uac";
            if (value.Equals("uas", StringComparison.OrdinalIgnoreCase))
                return "uas";
        }

        return null;
    }

    /// <summary>
    /// Appends one token to a comma-separated header token list.
    /// </summary>
    private static string AppendToken(string? currentValue, string token)
    {
        if (string.IsNullOrWhiteSpace(currentValue))
            return token;

        var hasToken = currentValue
            .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
            .Any(candidate => candidate.Equals(token, StringComparison.OrdinalIgnoreCase));
        return hasToken ? currentValue : $"{currentValue}, {token}";
    }
}
