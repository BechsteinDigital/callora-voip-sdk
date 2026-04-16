using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Ports.Media;

/// <summary>
/// Writes media frames into one persistent file.
/// </summary>
internal interface IAudioFileWriter : IAsyncDisposable
{
    /// <summary>
    /// Number of payload bytes written so far.
    /// </summary>
    long BytesWritten { get; }

    /// <summary>
    /// Appends one frame to the output file.
    /// </summary>
    ValueTask WriteFrameAsync(MediaFrame frame, CancellationToken ct = default);
}
