using System.Net;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Client;

/// <summary>
/// Resolves a STUN server hostname to a concrete endpoint using DNS SRV records
/// per RFC 5389 §9, with A/AAAA fallback.
/// </summary>
internal interface IStunServerResolver
{
    /// <summary>
    /// Resolves <paramref name="host"/> to an endpoint suitable for the given transport.
    /// <para>
    /// Resolution order per RFC 5389 §9:
    /// 1. DNS SRV query for the transport-specific service name
    ///    (<c>_stun._udp</c>, <c>_stun._tcp</c>, or <c>_stuns._tcp</c>).
    /// 2. Fallback to A/AAAA lookup with the transport's default port
    ///    (3478 for UDP/TCP, 5349 for TLS).
    /// </para>
    /// </summary>
    /// <param name="host">Domain name or IP address of the STUN server.</param>
    /// <param name="transport">Network transport used to connect.</param>
    /// <param name="ct">Cancellation token.</param>
    Task<IPEndPoint> ResolveAsync(string host, StunTransport transport, CancellationToken ct = default);
}
