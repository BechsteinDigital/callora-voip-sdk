namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Transport used by the TURN client to connect to the TURN server.
/// </summary>
internal enum TurnTransport
{
    /// <summary>TURN over UDP (default port 3478).</summary>
    Udp,

    /// <summary>TURN over TCP (default port 3478).</summary>
    Tcp,

    /// <summary>TURNS over TLS/TCP (default port 5349).</summary>
    Tls
}
