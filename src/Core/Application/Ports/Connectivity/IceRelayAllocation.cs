using System.Net;

namespace CalloraVoipSdk.Core.Application.Ports.Connectivity;

/// <summary>
/// Result payload for one successful TURN relay allocation used by ICE gathering.
/// </summary>
internal sealed class IceRelayAllocation
{
    /// <summary>
    /// Relayed endpoint allocated on the TURN server.
    /// </summary>
    public required IPEndPoint RelayedEndPoint { get; init; }

    /// <summary>
    /// Optional server-reflexive endpoint observed by the TURN server.
    /// </summary>
    public IPEndPoint? MappedEndPoint { get; init; }

    /// <summary>
    /// Reported allocation lifetime.
    /// </summary>
    public TimeSpan Lifetime { get; init; }
}
