using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Routing;

/// <summary>
/// Input model for one SIP routing resolution request.
/// Carries the logical destination plus the preferred signaling transport.
/// </summary>
internal sealed class SipRouteResolutionRequest
{
    /// <summary>
    /// Target host from SIP URI or account configuration.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Explicit target port, when provided by caller.
    /// If null or less/equal zero, RFC3263 DNS fallback chain is used.
    /// </summary>
    public int? Port { get; init; }

    /// <summary>
    /// Preferred transport requested by upper signaling layer.
    /// </summary>
    public SipTransportProtocol PreferredTransport { get; init; }
}

