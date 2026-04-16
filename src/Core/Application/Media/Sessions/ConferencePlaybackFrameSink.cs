using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Playback sink that broadcasts frames into an active conference.
/// </summary>
internal sealed class ConferencePlaybackFrameSink : IPlaybackFrameSink
{
    private readonly IMixedMediaBus _conference;
    private bool _disposed;

    /// <summary>
    /// Creates a conference playback sink.
    /// </summary>
    public ConferencePlaybackFrameSink(IMixedMediaBus conference)
    {
        _conference = conference ?? throw new ArgumentNullException(nameof(conference));
    }

    /// <inheritdoc />
    public string TargetToken => _conference.BusToken;

    /// <inheritdoc />
    public ValueTask SendAsync(MediaFrame frame, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(ConferencePlaybackFrameSink));

        return new ValueTask(_conference.InjectPlaybackFrameAsync(frame, ct));
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        _disposed = true;
        return ValueTask.CompletedTask;
    }
}
