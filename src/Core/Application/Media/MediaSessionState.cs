namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Runtime state for recording/playback media sessions.
/// </summary>
public enum MediaSessionState
{
    /// <summary>
    /// Session is actively processing media frames.
    /// </summary>
    Running = 0,

    /// <summary>
    /// Session is temporarily paused.
    /// </summary>
    Paused = 1,

    /// <summary>
    /// Session stopped gracefully and released resources.
    /// </summary>
    Stopped = 2,

    /// <summary>
    /// Session hit an unrecoverable runtime error.
    /// </summary>
    Faulted = 3,
}
