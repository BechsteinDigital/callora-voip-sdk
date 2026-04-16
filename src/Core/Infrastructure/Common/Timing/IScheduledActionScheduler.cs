namespace CalloraVoipSdk.Core.Infrastructure.Common.Timing;

/// <summary>
/// Schedules one-shot callbacks to run after a delay.
/// </summary>
internal interface IScheduledActionScheduler : IDisposable
{
    /// <summary>
    /// Schedules one callback for execution after the specified delay.
    /// Returns a handle that can cancel the callback before execution.
    /// </summary>
    IDisposable Schedule(
        TimeSpan delay,
        Action callback);
}
