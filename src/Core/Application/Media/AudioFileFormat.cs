namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Audio file container format used by recording and playback sessions.
/// </summary>
public enum AudioFileFormat
{
    /// <summary>
    /// RIFF/WAVE container with PCM16 payload.
    /// </summary>
    Wav = 0,

    /// <summary>
    /// MPEG Layer III frame stream.
    /// </summary>
    Mp3 = 1,
}
