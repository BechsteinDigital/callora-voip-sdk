namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

/// <summary>
/// Wire transport protocol options for SIP signaling.
/// </summary>
internal enum SipTransportProtocol
{
    /// <summary>
    /// SIP over UDP.
    /// </summary>
    Udp = 0,

    /// <summary>
    /// SIP over TCP.
    /// </summary>
    Tcp = 1,

    /// <summary>
    /// SIP over TLS (SIPS).
    /// </summary>
    Tls = 2,

    /// <summary>
    /// SIP over WebSocket.
    /// </summary>
    Ws = 3,

    /// <summary>
    /// SIP over secure WebSocket.
    /// </summary>
    Wss = 4
}
