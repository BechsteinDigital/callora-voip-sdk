using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Recording frame source that taps the mixed conference output bus.
/// </summary>
internal sealed class ConferenceRecordingFrameSource : IRecordingFrameSource
{
    private readonly IMixedMediaBus _conference;
    private readonly ILogger _logger;
    private IDisposable? _subscription;
    private bool _disposed;

    /// <summary>
    /// Creates a conference recording frame source.
    /// </summary>
    public ConferenceRecordingFrameSource(IMixedMediaBus conference, ILogger logger)
    {
        _conference = conference ?? throw new ArgumentNullException(nameof(conference));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public string SourceToken => _conference.BusToken;

    /// <inheritdoc />
    public event Action<MediaFrame>? FrameReceived;

    /// <inheritdoc />
    public ValueTask StartAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (_disposed)
            throw new ObjectDisposedException(nameof(ConferenceRecordingFrameSource));

        _subscription ??= _conference.SubscribeMixedFrames(OnMixedFrame);
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (_disposed)
            return ValueTask.CompletedTask;

        _disposed = true;
        _subscription?.Dispose();
        _subscription = null;
        return ValueTask.CompletedTask;
    }

    private void OnMixedFrame(MediaFrame frame)
    {
        try
        {
            FrameReceived?.Invoke(frame);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Recording frame callback failed for conference {ConferenceId}.",
                _conference.BusToken);
        }
    }
}
