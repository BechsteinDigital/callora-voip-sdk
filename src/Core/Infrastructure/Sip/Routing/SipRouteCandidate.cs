using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Routing;

/// <summary>
/// One concrete SIP routing candidate resolved from DNS/IP rules.
/// </summary>
internal sealed class SipRouteCandidate
{
    /// <summary>
    /// Resolved remote endpoint used for socket send/connect.
    /// </summary>
    public required IPEndPoint EndPoint { get; init; }

    /// <summary>
    /// Effective transport selected for this route candidate.
    /// </summary>
    public required SipTransportProtocol Transport { get; init; }

    /// <summary>
    /// Optional debug source label (for example "naptr+srv", "srv", "a/aaaa").
    /// </summary>
    public string Source { get; init; } = "unknown";
}
