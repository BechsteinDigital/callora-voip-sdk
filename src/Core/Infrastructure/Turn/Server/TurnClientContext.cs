using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Logical TURN client context derived from the active transport endpoint/connection.
/// </summary>
internal readonly record struct TurnClientContext(
    TurnServerTransport Transport,
    IPEndPoint RemoteEndPoint,
    TurnStreamConnection? StreamConnection)
{
    /// <summary>
    /// Stable key used to map requests to allocation state.
    /// </summary>
    public string ClientKey => Transport == TurnServerTransport.Udp
        ? $"udp:{RemoteEndPoint.Address}:{RemoteEndPoint.Port}"
        : StreamConnection!.ClientKey();
}

internal static class TurnStreamConnectionExtensions
{
    public static string ClientKey(this TurnStreamConnection connection)
        => $"stream:{connection.Id}";
}
