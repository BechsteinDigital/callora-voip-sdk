using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Input model for initiating an out-of-dialog SIP SUBSCRIBE (RFC 6665 §4.1).
/// </summary>
internal sealed record SipSubscribeRequest
{
    /// <summary>
    /// SIP username used in the local From URI (e.g. "alice").
    /// </summary>
    public string LocalUsername { get; init; } = string.Empty;

    /// <summary>
    /// SIP domain used in the local From URI (e.g. "example.org").
    /// </summary>
    public string LocalDomain { get; init; } = string.Empty;

    /// <summary>
    /// Target SIP URI to subscribe to (e.g. "sip:bob@example.org").
    /// </summary>
    public string RemoteUri { get; init; } = string.Empty;

    /// <summary>
    /// Event package name for the <c>Event</c> header (e.g. "presence", "dialog").
    /// </summary>
    public string EventType { get; init; } = string.Empty;

    /// <summary>
    /// Requested subscription lifetime in seconds. Defaults to 300.
    /// </summary>
    public int ExpiresSeconds { get; init; } = 300;

    /// <summary>
    /// Optional <c>Accept</c> header value for the SUBSCRIBE request.
    /// </summary>
    public string? AcceptHeader { get; init; }

    /// <summary>
    /// Optional SIP auth password used when challenged.
    /// </summary>
    public string? AuthPassword { get; init; }

    /// <summary>
    /// SIP signaling transport protocol.
    /// </summary>
    public SipTransportProtocol Transport { get; init; } = SipTransportProtocol.Udp;

    /// <summary>
    /// Transaction timeout. Defaults to 32 seconds.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(32);
}
