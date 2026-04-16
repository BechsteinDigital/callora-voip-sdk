using CalloraVoipSdk.Core.Infrastructure.Common.Protocols;
using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Parses and formats SIP asserted/preferred identity headers (RFC 3325).
/// </summary>
internal static class SipAssertedIdentityHeader
{
    /// <summary>
    /// Tries to parse the first identity URI from one header value.
    /// </summary>
    public static bool TryParseFirstIdentityUri(
        string? headerValue,
        out string identityUri)
    {
        identityUri = string.Empty;
        if (string.IsNullOrWhiteSpace(headerValue))
            return false;

        foreach (var token in ProtocolCommonUtilities.SplitCommaSeparatedRespectingQuotes(headerValue))
        {
            var parsedUri = SipProtocol.ExtractUriFromNameAddr(token);
            var candidate = !string.IsNullOrWhiteSpace(parsedUri)
                ? parsedUri
                : token.Trim().Trim('"');
            if (string.IsNullOrWhiteSpace(candidate))
                continue;

            identityUri = candidate;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Formats one identity URI as a name-addr value suitable for identity headers.
    /// </summary>
    public static string FormatIdentityValue(string identityUri)
    {
        if (string.IsNullOrWhiteSpace(identityUri))
            throw new ArgumentException("Identity URI is required.", nameof(identityUri));

        var parsedUri = SipProtocol.ExtractUriFromNameAddr(identityUri);
        var normalized = !string.IsNullOrWhiteSpace(parsedUri)
            ? parsedUri
            : identityUri.Trim();
        return $"<{normalized}>";
    }
}
