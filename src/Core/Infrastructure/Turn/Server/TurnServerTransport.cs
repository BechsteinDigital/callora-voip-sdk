namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Listener transport used by the TURN server.
/// </summary>
internal enum TurnServerTransport
{
    /// <summary>TURN over UDP.</summary>
    Udp,

    /// <summary>TURN over TCP.</summary>
    Tcp,

    /// <summary>TURNS over TLS/TCP.</summary>
    Tls
}
