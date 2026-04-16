using CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transactions;

/// <summary>
/// Result model for one completed SIP client transaction.
/// </summary>
internal sealed class SipClientTransactionResult
{
    /// <summary>
    /// Final non-provisional SIP response.
    /// </summary>
    public required SipResponseEnvelope FinalResponse { get; init; }

    /// <summary>
    /// Provisional responses observed before final completion.
    /// </summary>
    public IReadOnlyList<SipResponseEnvelope> ProvisionalResponses { get; init; } = [];

    /// <summary>
    /// Number of request send attempts including retransmits.
    /// </summary>
    public int SendAttempts { get; init; }
}

