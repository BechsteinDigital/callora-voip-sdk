namespace CalloraVoipSdk.Core.Application.Ports.Audio;

/// <summary>
/// Runtime device format applied to capture/playback streams.
/// </summary>
public sealed class AudioDeviceFormat
{
    /// <summary>
    /// Default mono 16-bit telephony format (8 kHz).
    /// </summary>
    public static readonly AudioDeviceFormat Default = new();

    /// <summary>
    /// Sample rate in Hz.
    /// </summary>
    public int SampleRate { get; init; } = 8000;

    /// <summary>
    /// PCM bit depth per sample. Current platform implementations support 16.
    /// </summary>
    public int BitsPerSample { get; init; } = 16;

    /// <summary>
    /// Channel count. Current SDK call audio is mono (1).
    /// </summary>
    public int Channels { get; init; } = 1;
}
