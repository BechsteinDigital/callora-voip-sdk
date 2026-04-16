namespace CalloraVoipSdk.Core.Domain.Lines;

/// <summary>
/// Supported SIP signaling transports on line-account level.
/// </summary>
public enum SipTransport
{
    /// <summary>
    /// SIP over UDP.
    /// </summary>
    Udp,

    /// <summary>
    /// SIP over TCP.
    /// </summary>
    Tcp,

    /// <summary>
    /// SIP over TLS.
    /// </summary>
    Tls,

    /// <summary>
    /// SIP over WebSocket.
    /// </summary>
    Ws,

    /// <summary>
    /// SIP over secure WebSocket.
    /// </summary>
    Wss
}
