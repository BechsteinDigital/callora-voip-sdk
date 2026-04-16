using CalloraVoipSdk.Core.Domain.Lines;

namespace CalloraVoipSdk.Core.Domain.Events;

/// <summary>
/// Raised when a phone line has permanently failed to re-register and entered
/// <see cref="LineState.Failed"/>.  No further reconnect attempts will be made.
/// </summary>
public sealed class LineReconnectFailedEventArgs : EventArgs
{
    /// <summary>The reason the re-registration failed permanently.</summary>
    public ReregisterFailReason Reason { get; }

    /// <summary>
    /// Total number of reconnect attempts made before the line gave up.
    /// </summary>
    public int AttemptCount { get; }

    /// <summary>The phone line that permanently failed to reconnect.</summary>
    public IPhoneLine Line { get; }

    internal LineReconnectFailedEventArgs(ReregisterFailReason reason, int attemptCount, IPhoneLine line)
        => (Reason, AttemptCount, Line) = (reason, attemptCount, line);
}
