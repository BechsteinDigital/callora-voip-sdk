namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Runtime lease information for one active inbound subscription.
/// </summary>
internal sealed class SipSubscriptionLease : IDisposable
{
    /// <summary>
    /// Creates one active subscription lease.
    /// </summary>
    public SipSubscriptionLease(
        SipSubscriptionIdentifier identifier,
        int expiresSeconds,
        CancellationTokenSource cancellation)
    {
        Identifier = identifier ?? throw new ArgumentNullException(nameof(identifier));
        if (expiresSeconds <= 0)
            throw new ArgumentOutOfRangeException(nameof(expiresSeconds), "expiresSeconds must be > 0.");

        ExpiresSeconds = expiresSeconds;
        ExpiresAtUtc = DateTimeOffset.UtcNow.AddSeconds(expiresSeconds);
        Cancellation = cancellation ?? throw new ArgumentNullException(nameof(cancellation));
    }

    /// <summary>
    /// Subscription identity.
    /// </summary>
    public SipSubscriptionIdentifier Identifier { get; }

    /// <summary>
    /// Lease duration in seconds.
    /// </summary>
    public int ExpiresSeconds { get; }

    /// <summary>
    /// Absolute expiration instant.
    /// </summary>
    public DateTimeOffset ExpiresAtUtc { get; }

    /// <summary>
    /// Cancellation token used for scheduled timeout task.
    /// </summary>
    public CancellationTokenSource Cancellation { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        Cancellation.Cancel();
        Cancellation.Dispose();
    }
}
