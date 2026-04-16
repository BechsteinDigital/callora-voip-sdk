namespace CalloraVoipSdk;

/// <summary>
/// Transport protocol used to connect to STUN/TURN servers.
/// </summary>
public enum IceTransport
{
    /// <summary>
    /// UDP transport (default port 3478).
    /// </summary>
    Udp,

    /// <summary>
    /// TCP transport (default port 3478).
    /// </summary>
    Tcp,

    /// <summary>
    /// TLS-over-TCP transport (default port 5349).
    /// </summary>
    Tls
}
