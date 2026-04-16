namespace CalloraVoipSdk.Core.Infrastructure.Common.Timing;

/// <summary>
/// Cancellation handle for one scheduled callback.
/// </summary>
internal sealed class ScheduledActionHandle : IDisposable
{
    private readonly long _id;
    private readonly Action<long> _cancel;
    private int _disposed;

    /// <summary>
    /// Creates a cancellation handle for one scheduler entry.
    /// </summary>
    public ScheduledActionHandle(long id, Action<long> cancel)
    {
        _id = id;
        _cancel = cancel ?? throw new ArgumentNullException(nameof(cancel));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
            return;

        _cancel(_id);
    }
}
