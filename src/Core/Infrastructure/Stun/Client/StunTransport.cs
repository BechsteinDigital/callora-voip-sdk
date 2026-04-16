namespace CalloraVoipSdk.Core.Infrastructure.Stun.Client;

/// <summary>
/// Network transport used when connecting to a STUN server (RFC 5389 §7.2, §9).
/// Determines which DNS SRV record family is queried and which default port applies.
/// </summary>
internal enum StunTransport
{
    /// <summary>
    /// UDP transport (RFC 5389 §7.2.1).
    /// Default port: 3478. DNS SRV service: <c>_stun._udp</c>.
    /// </summary>
    Udp,

    /// <summary>
    /// TCP transport (RFC 5389 §7.2.2).
    /// Default port: 3478. DNS SRV service: <c>_stun._tcp</c>.
    /// </summary>
    Tcp,

    /// <summary>
    /// TLS-over-TCP transport (RFC 5389 §7.2.2).
    /// Default port: 5349. DNS SRV service: <c>_stuns._tcp</c>.
    /// </summary>
    Tls
}
