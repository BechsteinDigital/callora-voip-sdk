namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Public handle for one active recording session.
/// </summary>
public interface IRecordingSession : IAsyncDisposable
{
    /// <summary>
    /// Stable recording session identifier.
    /// </summary>
    Guid SessionId { get; }

    /// <summary>
    /// Current recording runtime state.
    /// </summary>
    MediaSessionState State { get; }

    /// <summary>
    /// Selected recording container format.
    /// </summary>
    AudioFileFormat Format { get; }

    /// <summary>
    /// All output files created by this session (including rotated parts).
    /// </summary>
    IReadOnlyList<string> OutputFiles { get; }

    /// <summary>
    /// Pauses media capture while keeping resources allocated.
    /// </summary>
    Task PauseAsync(CancellationToken ct = default);

    /// <summary>
    /// Resumes media capture.
    /// </summary>
    Task ResumeAsync(CancellationToken ct = default);

    /// <summary>
    /// Stops recording and finalizes output files.
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
