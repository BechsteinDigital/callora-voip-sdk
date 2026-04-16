namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Public handle for one active playback session.
/// </summary>
public interface IPlaybackSession : IAsyncDisposable
{
    /// <summary>
    /// Stable playback session identifier.
    /// </summary>
    Guid SessionId { get; }

    /// <summary>
    /// Current playback runtime state.
    /// </summary>
    MediaSessionState State { get; }

    /// <summary>
    /// Source file path used for playback.
    /// </summary>
    string SourceFilePath { get; }

    /// <summary>
    /// Source file format.
    /// </summary>
    AudioFileFormat Format { get; }

    /// <summary>
    /// Pauses playback.
    /// </summary>
    Task PauseAsync(CancellationToken ct = default);

    /// <summary>
    /// Resumes playback.
    /// </summary>
    Task ResumeAsync(CancellationToken ct = default);

    /// <summary>
    /// Stops playback and releases resources.
    /// </summary>
    Task StopAsync(CancellationToken ct = default);

    /// <summary>
    /// Raised whenever the session changes runtime state.
    /// </summary>
    event EventHandler<MediaSessionStateChangedEventArgs>? StateChanged;

    /// <summary>
    /// Raised when the session enters a faulted state.
    /// </summary>
    event EventHandler<MediaSessionErrorEventArgs>? Error;
}
