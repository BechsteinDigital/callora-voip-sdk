namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Optional provider that encrypts finalized recording files.
/// </summary>
public interface IRecordingEncryptionProvider
{
    /// <summary>
    /// File extension (without dot) for encrypted output files.
    /// </summary>
    string OutputFileExtension { get; }

    /// <summary>
    /// Encrypts one recorded input file into <paramref name="encryptedOutputPath"/>.
    /// </summary>
    ValueTask EncryptFileAsync(
        string inputFilePath,
        string encryptedOutputPath,
        CancellationToken ct = default);
}
