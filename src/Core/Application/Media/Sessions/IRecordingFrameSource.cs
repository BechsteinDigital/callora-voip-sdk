using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Abstraction for recording frame sources (call or conference).
/// </summary>
internal interface IRecordingFrameSource : IAsyncDisposable
{
    /// <summary>
    /// Logical identifier of the source target.
    /// </summary>
    string SourceToken { get; }

    /// <summary>
    /// Raised when a new media frame is available for recording.
    /// </summary>
    event Action<MediaFrame>? FrameReceived;

    /// <summary>
    /// Starts frame acquisition.
    /// </summary>
    ValueTask StartAsync(CancellationToken ct = default);
}
