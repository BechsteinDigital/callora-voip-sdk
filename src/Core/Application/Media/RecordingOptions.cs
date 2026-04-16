namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Configuration for call/conference recording sessions.
/// </summary>
public sealed class RecordingOptions
{
    /// <summary>
    /// Output directory used for created recording files.
    /// </summary>
    public string OutputDirectory { get; init; } = AppContext.BaseDirectory;

    /// <summary>
    /// File name prefix used for generated recording files.
    /// </summary>
    public string FileNamePrefix { get; init; } = "recording";

    /// <summary>
    /// Target file format.
    /// </summary>
    public AudioFileFormat Format { get; init; } = AudioFileFormat.Wav;

    /// <summary>
    /// Optional max payload bytes per output file.
    /// When exceeded a new file part is started.
    /// </summary>
    public long? RotateAfterBytes { get; init; }

    /// <summary>
    /// Sample rate used for PCM-centric formats when media parameters are unavailable.
    /// </summary>
    public int SampleRateHz { get; init; } = 8000;

    /// <summary>
    /// Suggested samples per frame for file readers/writers.
    /// </summary>
    public int SamplesPerFrame { get; init; } = 160;

    /// <summary>
    /// Adds UTC timestamp suffix to generated file names when true.
    /// </summary>
    public bool IncludeUtcTimestamp { get; init; } = true;

    /// <summary>
    /// Optional encryption provider for finalized recording files.
    /// </summary>
    public IRecordingEncryptionProvider? EncryptionProvider { get; init; }

    /// <summary>
    /// When true, plaintext recording files are deleted after successful encryption.
    /// </summary>
    public bool DeletePlaintextAfterEncryption { get; init; } = true;

    /// <summary>
    /// When true, near-silent PCM16 frames are skipped during recording.
    /// </summary>
    public bool SkipSilence { get; init; }

    /// <summary>
    /// Absolute PCM16 sample threshold used for silence detection.
    /// </summary>
    public short SilenceThresholdPcm16 { get; init; } = 128;
}
