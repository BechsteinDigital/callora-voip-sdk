using System.Net;
using CalloraVoipSdk;

namespace CalloraVoipSdk.Core.Application.Ports.Connectivity;

/// <summary>
/// Port used by the application-layer ICE agent to request TURN relay allocations.
/// </summary>
internal interface IIceTurnRelayAllocator
{
    /// <summary>
    /// Tries to allocate a relay endpoint for ICE candidate gathering.
    /// </summary>
    Task<IceRelayAllocation?> TryAllocateRelayAsync(
        IPEndPoint localEndPoint,
        IceServerConfiguration server,
        CancellationToken ct = default);
}
