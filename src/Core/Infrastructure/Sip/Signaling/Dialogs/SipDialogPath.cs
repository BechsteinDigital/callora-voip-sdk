namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Dialog routing snapshot containing remote tag, target URI, and route set.
/// </summary>
internal sealed class SipDialogPath
{
    /// <summary>
    /// Remote dialog tag from To/From headers.
    /// </summary>
    public required string RemoteTag { get; init; }

    /// <summary>
    /// Request-URI target for in-dialog requests.
    /// </summary>
    public required string RemoteTargetUri { get; set; }

    /// <summary>
    /// Route header set used for in-dialog routing.
    /// </summary>
    public required IReadOnlyList<string> RouteSet { get; init; }
}

