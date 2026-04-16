using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Defines trust-boundary decisions for SIP identity headers such as
/// <c>P-Asserted-Identity</c> (RFC 3325).
/// </summary>
internal interface ISipIdentityTrustPolicy
{
    /// <summary>
    /// Returns true when an incoming peer is trusted to provide asserted identity headers.
    /// </summary>
    bool IsTrusted(IPEndPoint remoteEndPoint, SipTransportProtocol transport);
}
