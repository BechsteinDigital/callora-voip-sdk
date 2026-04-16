namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Tracks one reliable provisional INVITE response awaiting PRACK acknowledgment.
/// </summary>
internal sealed class SipReliableProvisionalEntry
{
    private readonly TaskCompletionSource<bool> _prackReceived =
        new(TaskCreationOptions.RunContinuationsAsynchronously);

    /// <summary>
    /// Creates a pending reliable provisional entry.
    /// </summary>
    public SipReliableProvisionalEntry(
        int rseq,
        int inviteCseq)
    {
        if (rseq <= 0)
            throw new ArgumentOutOfRangeException(nameof(rseq), "rseq must be > 0.");
        if (inviteCseq <= 0)
            throw new ArgumentOutOfRangeException(nameof(inviteCseq), "inviteCseq must be > 0.");

        RSeq = rseq;
        InviteCSeq = inviteCseq;
    }

    /// <summary>
    /// Reliable provisional sequence number.
    /// </summary>
    public int RSeq { get; }

    /// <summary>
    /// CSeq number of the INVITE transaction this provisional belongs to.
    /// </summary>
    public int InviteCSeq { get; }

    /// <summary>
    /// Waits until PRACK acknowledgment arrives or the wait is canceled.
    /// </summary>
    public Task WaitForPrackAsync(CancellationToken ct) =>
        _prackReceived.Task.WaitAsync(ct);

    /// <summary>
    /// Marks this provisional response as acknowledged by PRACK.
    /// </summary>
    public bool TryAcknowledge() => _prackReceived.TrySetResult(true);

    /// <summary>
    /// Cancels any waiters due to dialog shutdown/disposal.
    /// </summary>
    public void Cancel() => _prackReceived.TrySetCanceled();
}
