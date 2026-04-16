namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Listener transport used by <see cref="StunServer"/>.
/// </summary>
internal enum StunServerTransport
{
    /// <summary>
    /// UDP listener (RFC 5389 §7.2.1).
    /// </summary>
    Udp,

    /// <summary>
    /// TCP listener (RFC 5389 §7.2.2).
    /// </summary>
    Tcp,

    /// <summary>
    /// TLS-over-TCP listener (RFC 5389 §7.2.2).
    /// </summary>
    Tls
}
