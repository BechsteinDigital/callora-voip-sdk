namespace CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

/// <summary>
/// Contract for SIP wire parsing and serialization.
/// This abstraction enables dependency injection and protocol-module swapping.
/// </summary>
internal interface ISipWireCodec
{
    /// <summary>
    /// Attempts to parse a SIP request from wire bytes.
    /// </summary>
    bool TryParseRequest(ReadOnlySpan<byte> payload, out SipRequest? request);

    /// <summary>
    /// Attempts to parse a SIP response from wire bytes.
    /// </summary>
    bool TryParseResponse(ReadOnlySpan<byte> payload, out SipResponse? response);

    /// <summary>
    /// Serializes a SIP request into wire bytes.
    /// </summary>
    byte[] SerializeRequest(
        string method,
        string requestUri,
        IReadOnlyDictionary<string, string> headers,
        string? body = null);

    /// <summary>
    /// Serializes a SIP response into wire bytes.
    /// </summary>
    byte[] SerializeResponse(
        int statusCode,
        string reasonPhrase,
        IReadOnlyDictionary<string, string> headers,
        string? body = null);
}

