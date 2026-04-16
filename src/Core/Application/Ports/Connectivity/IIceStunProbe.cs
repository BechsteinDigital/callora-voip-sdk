using System.Net;
using CalloraVoipSdk;

namespace CalloraVoipSdk.Core.Application.Ports.Connectivity;

/// <summary>
/// Port for ICE STUN interactions used by the application-layer ICE agent.
/// </summary>
internal interface IIceStunProbe
{
    /// <summary>
    /// Tries to resolve a server-reflexive endpoint for <paramref name="localEndPoint"/>
    /// via one configured STUN server.
    /// </summary>
    Task<IPEndPoint?> TryGetServerReflexiveEndPointAsync(
        IPEndPoint localEndPoint,
        IceServerConfiguration server,
        CancellationToken ct = default);

    /// <summary>
    /// Executes one ICE connectivity check toward <paramref name="remoteEndPoint"/>.
    /// </summary>
    Task<bool> TryCheckConnectivityAsync(
        IPEndPoint localEndPoint,
        IPEndPoint remoteEndPoint,
        string localIceUfrag,
        string remoteIceUfrag,
        string remoteIcePassword,
        TimeSpan timeout,
        CancellationToken ct = default);
}
