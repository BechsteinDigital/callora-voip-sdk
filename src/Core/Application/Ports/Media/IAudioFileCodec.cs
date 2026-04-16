namespace CalloraVoipSdk.Core.Application.Ports.Media;

/// <summary>
/// Format-specific codec capable of reading and writing media files.
/// </summary>
internal interface IAudioFileCodec
{
    /// <summary>
    /// Opens a writer for the given output file path.
    /// </summary>
    ValueTask<IAudioFileWriter> CreateWriterAsync(
        string filePath,
        AudioFileCodecContext context,
        CancellationToken ct = default);

    /// <summary>
    /// Opens a reader for the given source file path.
    /// </summary>
    ValueTask<IAudioFileReader> CreateReaderAsync(
        string filePath,
        AudioFileCodecContext context,
        CancellationToken ct = default);
}
