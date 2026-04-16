using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Ports.Media;

/// <summary>
/// Resolves file codecs for application recording/playback workflows.
/// </summary>
internal interface IAudioFileCodecRegistry
{
    /// <summary>
    /// Tries resolving a codec implementation for the requested format.
    /// </summary>
    bool TryGetCodec(AudioFileFormat format, out IAudioFileCodec codec);
}
