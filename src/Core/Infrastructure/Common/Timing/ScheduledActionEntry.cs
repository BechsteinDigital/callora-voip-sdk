namespace CalloraVoipSdk.Core.Infrastructure.Common.Timing;

/// <summary>
/// Internal scheduled callback entry.
/// </summary>
internal sealed class ScheduledActionEntry
{
    /// <summary>
    /// Creates one scheduled callback entry.
    /// </summary>
    public ScheduledActionEntry(
        long id,
        long dueAtTicks,
        Action callback)
    {
        Id = id;
        DueAtTicks = dueAtTicks;
        Callback = callback;
    }

    /// <summary>
    /// Unique scheduler-local identifier.
    /// </summary>
    public long Id { get; }

    /// <summary>
    /// Due timestamp expressed as UTC ticks.
    /// </summary>
    public long DueAtTicks { get; }

    /// <summary>
    /// Callback to execute when due.
    /// </summary>
    public Action Callback { get; }

    /// <summary>
    /// Marks entry as canceled without requiring queue removal in-place.
    /// </summary>
    public bool IsCanceled { get; set; }
}
