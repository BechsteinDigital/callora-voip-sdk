namespace CalloraVoipSdk.Core.Application.Ports.Media;

/// <summary>
/// Reads media frames from one persistent file.
/// </summary>
internal interface IAudioFileReader : IAsyncDisposable
{
    /// <summary>
    /// Reads the next frame from the file.
    /// Returns <see langword="null"/> at end-of-file.
    /// </summary>
    ValueTask<AudioFileFrame?> ReadNextFrameAsync(CancellationToken ct = default);
}
