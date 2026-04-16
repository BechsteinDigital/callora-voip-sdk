using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Abstraction for playback frame sinks (call or conference).
/// </summary>
internal interface IPlaybackFrameSink : IAsyncDisposable
{
    /// <summary>
    /// Logical target token used in logs and filenames.
    /// </summary>
    string TargetToken { get; }

    /// <summary>
    /// Sends one playback frame to the target.
    /// </summary>
    ValueTask SendAsync(MediaFrame frame, CancellationToken ct = default);
}
