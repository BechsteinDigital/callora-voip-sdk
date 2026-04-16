using CalloraVoipSdk.Core.Infrastructure.Common.Protocols;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Parses and formats SIP Reason header values (RFC 3326).
/// </summary>
internal static class SipReasonHeader
{
    /// <summary>
    /// Tries to parse the first reason-value from a Reason header.
    /// Returns false when the header does not contain a valid reason-value.
    /// </summary>
    public static bool TryParseFirst(
        string? headerValue,
        out SipDialogTerminationReason? reason)
    {
        reason = null;
        if (string.IsNullOrWhiteSpace(headerValue))
            return false;

        var reasonValue = ProtocolCommonUtilities
            .SplitCommaSeparatedRespectingQuotes(headerValue)
            .FirstOrDefault();
        if (string.IsNullOrWhiteSpace(reasonValue))
            return false;

        var segments = reasonValue
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return false;

        var protocol = segments[0].Trim();
        if (string.IsNullOrWhiteSpace(protocol))
            return false;

        int? cause = null;
        string? text = null;

        for (var i = 1; i < segments.Length; i++)
        {
            var parameter = segments[i];
            var equalsIndex = parameter.IndexOf('=');
            if (equalsIndex <= 0)
                continue;

            var name = parameter[..equalsIndex].Trim();
            var value = parameter[(equalsIndex + 1)..].Trim();
            if (name.Equals("cause", StringComparison.OrdinalIgnoreCase))
            {
                if (int.TryParse(value, out var parsedCause))
                    cause = parsedCause;
                continue;
            }

            if (name.Equals("text", StringComparison.OrdinalIgnoreCase))
            {
                text = UnquoteAndUnescape(value);
            }
        }

        reason = new SipDialogTerminationReason(protocol, cause, text);
        return true;
    }

    /// <summary>
    /// Formats one reason value for Reason header emission.
    /// </summary>
    public static string Format(SipDialogTerminationReason reason)
    {
        ArgumentNullException.ThrowIfNull(reason);

        var formatted = reason.Protocol;
        if (reason.Cause is { } cause)
            formatted = $"{formatted};cause={cause}";
        if (!string.IsNullOrWhiteSpace(reason.Text))
        {
            var escapedText = ProtocolCommonUtilities.EscapeQuotedHeaderValue(reason.Text);
            formatted = $"{formatted};text=\"{escapedText}\"";
        }

        return formatted;
    }

    /// <summary>
    /// Builds one SIP protocol reason from a status code and reason phrase.
    /// </summary>
    public static SipDialogTerminationReason CreateSipStatusReason(
        int statusCode,
        string reasonPhrase) =>
        new(
            protocol: "SIP",
            cause: statusCode,
            text: reasonPhrase);

    /// <summary>
    /// Removes one optional surrounding quote pair and unescapes quoted-pair escapes.
    /// </summary>
    private static string UnquoteAndUnescape(string value)
    {
        var trimmed = value.Trim();
        if (trimmed.Length >= 2
            && trimmed[0] == '"'
            && trimmed[^1] == '"')
        {
            trimmed = trimmed[1..^1];
        }

        return trimmed
            .Replace("\\\"", "\"", StringComparison.Ordinal)
            .Replace("\\\\", "\\", StringComparison.Ordinal);
    }
}
