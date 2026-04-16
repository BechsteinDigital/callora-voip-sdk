namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Parsed value model for SIP <c>Replaces</c> header (RFC 3891).
/// </summary>
internal sealed class SipReplacesHeaderValue
{
    /// <summary>
    /// Creates one immutable Replaces header model.
    /// </summary>
    public SipReplacesHeaderValue(
        string callId,
        string toTag,
        string fromTag,
        bool earlyOnly)
    {
        if (string.IsNullOrWhiteSpace(callId))
            throw new ArgumentException("Replaces call-id is required.", nameof(callId));
        if (string.IsNullOrWhiteSpace(toTag))
            throw new ArgumentException("Replaces to-tag is required.", nameof(toTag));
        if (string.IsNullOrWhiteSpace(fromTag))
            throw new ArgumentException("Replaces from-tag is required.", nameof(fromTag));

        CallId = callId.Trim();
        ToTag = toTag.Trim();
        FromTag = fromTag.Trim();
        EarlyOnly = earlyOnly;
    }

    /// <summary>
    /// Target dialog Call-ID.
    /// </summary>
    public string CallId { get; }

    /// <summary>
    /// Target dialog To tag parameter value.
    /// </summary>
    public string ToTag { get; }

    /// <summary>
    /// Target dialog From tag parameter value.
    /// </summary>
    public string FromTag { get; }

    /// <summary>
    /// True when <c>early-only</c> parameter is present.
    /// </summary>
    public bool EarlyOnly { get; }

    /// <summary>
    /// Tries to parse one Replaces header string.
    /// Returns false for malformed values or missing required parameters.
    /// </summary>
    public static bool TryParse(
        string? headerValue,
        out SipReplacesHeaderValue? parsedValue)
    {
        parsedValue = null;
        if (string.IsNullOrWhiteSpace(headerValue))
            return false;

        var segments = headerValue
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length == 0)
            return false;

        var callId = segments[0].Trim();
        if (string.IsNullOrWhiteSpace(callId))
            return false;

        string? toTag = null;
        string? fromTag = null;
        var earlyOnly = false;
        for (var i = 1; i < segments.Length; i++)
        {
            var token = segments[i];
            if (token.Equals("early-only", StringComparison.OrdinalIgnoreCase))
            {
                earlyOnly = true;
                continue;
            }

            var equalsIndex = token.IndexOf('=');
            if (equalsIndex <= 0)
                continue;

            var name = token[..equalsIndex].Trim();
            var value = token[(equalsIndex + 1)..].Trim().Trim('"');
            if (name.Equals("to-tag", StringComparison.OrdinalIgnoreCase))
            {
                toTag = value;
                continue;
            }

            if (name.Equals("from-tag", StringComparison.OrdinalIgnoreCase))
                fromTag = value;
        }

        if (string.IsNullOrWhiteSpace(toTag) || string.IsNullOrWhiteSpace(fromTag))
            return false;

        parsedValue = new SipReplacesHeaderValue(callId, toTag, fromTag, earlyOnly);
        return true;
    }
}
