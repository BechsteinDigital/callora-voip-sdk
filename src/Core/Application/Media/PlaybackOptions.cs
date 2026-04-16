namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Runtime options for playback sessions.
/// </summary>
public sealed class PlaybackOptions
{
    /// <summary>
    /// Replays the source file repeatedly until the session is stopped.
    /// </summary>
    public bool Loop { get; init; }

    /// <summary>
    /// Starts the playback session in paused state.
    /// </summary>
    public bool StartPaused { get; init; }

    /// <summary>
    /// Optional fixed pacing delay per frame.
    /// When null, delay is derived from codec/file timing.
    /// </summary>
    public TimeSpan? FixedFrameDelay { get; init; }

    /// <summary>
    /// Samples per frame used for PCM file chunking when no explicit frame timing exists.
    /// </summary>
    public int SamplesPerFrame { get; init; } = 160;
}
