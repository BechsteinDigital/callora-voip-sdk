namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Describes one media session state transition.
/// </summary>
public sealed class MediaSessionStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Creates state transition metadata.
    /// </summary>
    public MediaSessionStateChangedEventArgs(
        MediaSessionState oldState,
        MediaSessionState newState,
        string reason,
        DateTimeOffset occurredAt)
    {
        OldState = oldState;
        NewState = newState;
        Reason = reason;
        OccurredAt = occurredAt;
    }

    /// <summary>
    /// State before the transition.
    /// </summary>
    public MediaSessionState OldState { get; }

    /// <summary>
    /// State after the transition.
    /// </summary>
    public MediaSessionState NewState { get; }

    /// <summary>
    /// Human-readable transition reason.
    /// </summary>
    public string Reason { get; }

    /// <summary>
    /// UTC timestamp of the transition.
    /// </summary>
    public DateTimeOffset OccurredAt { get; }
}
