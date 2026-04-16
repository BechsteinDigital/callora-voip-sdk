namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Handle for an active out-of-dialog SIP subscription (RFC 6665 §4.1).
/// Dispose or call <see cref="UnsubscribeAsync"/> to terminate the subscription.
/// </summary>
internal sealed class SipSubscriptionHandle : IAsyncDisposable
{
    private readonly Func<CancellationToken, Task> _unsubscribeFactory;
    private int _disposed;

    internal SipSubscriptionHandle(Func<CancellationToken, Task> unsubscribeFactory)
    {
        _unsubscribeFactory = unsubscribeFactory
            ?? throw new ArgumentNullException(nameof(unsubscribeFactory));
    }

    /// <summary>
    /// Raised when an inbound NOTIFY is received for this subscription.
    /// </summary>
    public event EventHandler<SipNotifyReceivedEventArgs>? NotifyReceived;

    /// <summary>
    /// Terminates the subscription by sending SUBSCRIBE with Expires: 0.
    /// </summary>
    public async Task UnsubscribeAsync(CancellationToken ct = default)
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        await _unsubscribeFactory(ct).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;
        try
        {
            await _unsubscribeFactory(CancellationToken.None).ConfigureAwait(false);
        }
        catch
        {
            // best-effort on dispose
        }
    }

    /// <summary>
    /// Dispatches inbound NOTIFY payload to registered handlers.
    /// </summary>
    internal void RaiseNotifyReceived(SipNotifyReceivedEventArgs args) =>
        NotifyReceived?.Invoke(this, args);
}
