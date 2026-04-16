namespace CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

/// <summary>
/// SIP header canonicalization helpers including compact form mapping.
/// </summary>
internal static class SipHeaderNames
{
    /// <summary>
    /// Returns canonical SIP header name for full or compact header tokens.
    /// </summary>
    public static string Canonicalize(string headerName)
    {
        if (string.IsNullOrWhiteSpace(headerName))
            return string.Empty;

        var normalized = headerName.Trim();
        return normalized.ToLowerInvariant() switch
        {
            "v" => "Via",
            "f" => "From",
            "t" => "To",
            "i" => "Call-ID",
            "m" => "Contact",
            "e" => "Content-Encoding",
            "l" => "Content-Length",
            "c" => "Content-Type",
            "k" => "Supported",
            "s" => "Subject",
            "u" => "Allow-Events",
            "o" => "Event",
            "r" => "Refer-To",
            _ => normalized
        };
    }
}
