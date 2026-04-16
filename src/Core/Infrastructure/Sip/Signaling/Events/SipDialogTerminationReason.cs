namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Represents one parsed SIP Reason header value (RFC 3326) associated with dialog termination.
/// </summary>
internal sealed class SipDialogTerminationReason
{
    /// <summary>
    /// Creates one immutable dialog termination reason value.
    /// </summary>
    public SipDialogTerminationReason(
        string protocol,
        int? cause = null,
        string? text = null,
        int? retryAfterSeconds = null)
    {
        if (string.IsNullOrWhiteSpace(protocol))
            throw new ArgumentException("Reason protocol is required.", nameof(protocol));

        Protocol = protocol.Trim();
        Cause = cause;
        Text = string.IsNullOrWhiteSpace(text) ? null : text.Trim();
        RetryAfterSeconds = retryAfterSeconds;
    }

    /// <summary>
    /// Reason protocol token, for example <c>SIP</c> or <c>Q.850</c>.
    /// </summary>
    public string Protocol { get; }

    /// <summary>
    /// Optional numeric cause code parameter.
    /// </summary>
    public int? Cause { get; }

    /// <summary>
    /// Optional human-readable reason text parameter.
    /// </summary>
    public string? Text { get; }

    /// <summary>
    /// Seconds to wait before retrying the request, parsed from the <c>Retry-After</c> header
    /// of a 503 Service Unavailable response (RFC 7339 §5.3 / RFC 3261 §20.33).
    /// Non-null only when the termination was caused by a 503 that carried a <c>Retry-After</c> header.
    /// </summary>
    public int? RetryAfterSeconds { get; }
}
