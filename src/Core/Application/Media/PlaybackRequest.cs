namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Input payload for starting a playback session.
/// </summary>
public sealed class PlaybackRequest
{
    /// <summary>
    /// Absolute or relative path to the source media file.
    /// </summary>
    public required string FilePath { get; init; }

    /// <summary>
    /// Explicit source format.
    /// When null the format is inferred from <see cref="FilePath"/> extension.
    /// </summary>
    public AudioFileFormat? Format { get; init; }

    /// <summary>
    /// Playback runtime options.
    /// </summary>
    public PlaybackOptions Options { get; init; } = new();

    /// <summary>
    /// Optional fallback sample rate for PCM playback contexts.
    /// </summary>
    public int SampleRateHz { get; init; } = 8000;
}
