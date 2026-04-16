namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Abstraction for a mixed PCM16 media bus that supports recording taps and playback injection.
/// </summary>
public interface IMixedMediaBus
{
    /// <summary>
    /// Stable token used for diagnostics and session labeling.
    /// </summary>
    string BusToken { get; }

    /// <summary>
    /// Subscribes to mixed outbound frames.
    /// </summary>
    IDisposable SubscribeMixedFrames(Action<MediaFrame> onFrame);

    /// <summary>
    /// Injects one playback frame into the bus.
    /// </summary>
    Task InjectPlaybackFrameAsync(MediaFrame frame, CancellationToken ct = default);
}
