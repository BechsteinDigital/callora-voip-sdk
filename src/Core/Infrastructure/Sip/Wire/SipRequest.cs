namespace CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

/// <summary>
/// Parsed SIP request model used by the infrastructure signaling layer.
/// </summary>
internal sealed class SipRequest
{
    /// <summary>
    /// Creates a SIP request model.
    /// </summary>
    public SipRequest(
        string method,
        string requestUri,
        IReadOnlyDictionary<string, string> headers,
        string body)
    {
        Method = method;
        RequestUri = requestUri;
        Headers = headers;
        Body = body;
    }

    /// <summary>
    /// SIP method (REGISTER, INVITE, ACK, BYE, ...).
    /// </summary>
    public string Method { get; }

    /// <summary>
    /// Request URI from the SIP start line.
    /// </summary>
    public string RequestUri { get; }

    /// <summary>
    /// Header map with case-insensitive keys.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// Message body, typically SDP for INVITE messages.
    /// </summary>
    public string Body { get; }

    /// <summary>
    /// Returns a header value or null when missing.
    /// </summary>
    public string? Header(string name)
    {
        var canonical = SipHeaderNames.Canonicalize(name);
        if (!SipHeaderRowRules.IsApplicableToRequest(canonical))
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
        if (!SipHeaderRowRules.IsApplicableToRequest(canonical))
            return Array.Empty<string>();

        if (Headers.TryGetValue(canonical, out var canonicalValue))
            return SipHeaderValueStorage.SplitRows(canonicalValue);
        if (Headers.TryGetValue(name, out var rawValue))
            return SipHeaderValueStorage.SplitRows(rawValue);
        return Array.Empty<string>();
    }
}
