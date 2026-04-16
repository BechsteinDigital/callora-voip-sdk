namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// One queued outbound INVITE destination candidate.
/// </summary>
internal sealed record SipOutboundInviteTarget(
    string RequestUri,
    string LogicalRemoteUri,
    IReadOnlyList<string> RouteSet,
    string NextHopUri);
