using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Infrastructure.Sip.Authentication;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transactions.Server;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Runtime dependencies required by a SIP dialog session.
/// </summary>
internal sealed class SipCallSessionDependencies
{
    public required ISipTransportRuntime Transport { get; init; }
    public required ISipDigestAuthenticator DigestAuthenticator { get; init; }
    public required ILogger Logger { get; init; }
    public required ISipServerTransactionEngine ServerTransactions { get; init; }
    public required ISipIdentityTrustPolicy IdentityTrustPolicy { get; init; }
    public required SipSessionSdpProvider SdpProvider { get; init; }
}
