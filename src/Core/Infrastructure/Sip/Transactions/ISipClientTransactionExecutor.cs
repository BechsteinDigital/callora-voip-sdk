namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transactions;

/// <summary>
/// Executes SIP client transactions with response correlation and retransmit timers.
/// </summary>
internal interface ISipClientTransactionExecutor
{
    /// <summary>
    /// Sends one SIP request and waits for its final response.
    /// </summary>
    Task<SipClientTransactionResult> ExecuteAsync(
        SipClientTransactionRequest request,
        CancellationToken ct = default);
}

