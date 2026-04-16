using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Internal state record for one active out-of-dialog subscription.
/// </summary>
internal sealed class SipOutboundSubscriptionEntry
{
    public required string CallId { get; init; }
    public required string EventType { get; init; }
    public required string RequestUri { get; init; }
    public required string LocalUri { get; init; }
    public required string RemoteUri { get; init; }
    public required string LocalTag { get; init; }
    public string? RemoteTag { get; set; }
    public required string AuthUsername { get; init; }
    public string? AuthPassword { get; init; }
    public string? AcceptHeader { get; init; }
    public required IPEndPoint RemoteEndPoint { get; init; }
    public required SipTransportProtocol Transport { get; init; }
    public required TimeSpan Timeout { get; init; }
    public int LocalCSeq { get; set; }
    public int ExpiresSeconds { get; set; }
    public SipSubscriptionHandle? Handle { get; set; }
    public CancellationTokenSource RefreshCts { get; } = new();
}
