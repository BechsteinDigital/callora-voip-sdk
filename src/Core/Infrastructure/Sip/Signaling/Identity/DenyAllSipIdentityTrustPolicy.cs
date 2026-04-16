using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Safe default trust policy that treats all peers as untrusted for asserted identity.
/// </summary>
internal sealed class DenyAllSipIdentityTrustPolicy : ISipIdentityTrustPolicy
{
    /// <summary>
    /// Singleton instance used by default in signaling services.
    /// </summary>
    public static DenyAllSipIdentityTrustPolicy Instance { get; } = new();

    /// <inheritdoc />
    public bool IsTrusted(IPEndPoint remoteEndPoint, SipTransportProtocol transport) => false;
}
