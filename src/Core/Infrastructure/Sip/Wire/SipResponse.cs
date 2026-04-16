namespace CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

/// <summary>
/// Parsed SIP response model used by the infrastructure signaling layer.
/// </summary>
internal sealed class SipResponse
{
    /// <summary>
    /// Creates a SIP response model.
    /// </summary>
    public SipResponse(
        int statusCode,
        string reasonPhrase,
        IReadOnlyDictionary<string, string> headers,
        string body)
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
        Headers = headers;
        Body = body;
    }

    /// <summary>
    /// SIP status code (for example 180, 200, 401, 486).
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// Reason phrase from the SIP status line.
    /// </summary>
    public string ReasonPhrase { get; }

    /// <summary>
    /// Header map with case-insensitive keys.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// Message body, typically SDP in 2xx responses to INVITE.
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// Returns a header value or null when missing.
    /// </summary>
    public string? Header(string name)
    {
        var canonical = SipHeaderNames.Canonicalize(name);
        if (!SipHeaderRowRules.IsApplicableToResponse(canonical))
            return null;

        if (Headers.TryGetValue(canonical, out var value))
            return SipHeaderValueStorage.FirstRow(value);
        if (Headers.TryGetValue(name, out value))
            return SipHeaderValueStorage.FirstRow(value);
        return null;
    }

    /// <summary>
    /// Returns all header row values for one header name.
    /// </summary>
    public IReadOnlyList<string> HeaderValues(string name)
    {
        var canonical = SipHeaderNames.Canonicalize(name);
        if (!SipHeaderRowRules.IsApplicableToResponse(canonical))
            return Array.Empty<string>();

        if (Headers.TryGetValue(canonical, out var canonicalValue))
            return SipHeaderValueStorage.SplitRows(canonicalValue);
        if (Headers.TryGetValue(name, out var rawValue))
            return SipHeaderValueStorage.SplitRows(rawValue);
        return Array.Empty<string>();
    }
}
