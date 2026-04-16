using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

/// <summary>
/// Default registry for audio file codecs used by recording and playback services.
/// </summary>
internal sealed class AudioFileCodecRegistry : IAudioFileCodecRegistry
{
    private readonly IReadOnlyDictionary<AudioFileFormat, IAudioFileCodec> _codecs;

    /// <summary>
    /// Creates a codec registry with WAV and MP3 adapters.
    /// </summary>
    public AudioFileCodecRegistry()
    {
        _codecs = new Dictionary<AudioFileFormat, IAudioFileCodec>
        {
            [AudioFileFormat.Wav] = new WavAudioFileCodec(),
            [AudioFileFormat.Mp3] = new Mp3AudioFileCodec(),
        };
    }

    /// <inheritdoc />
    public bool TryGetCodec(AudioFileFormat format, out IAudioFileCodec codec)
        => _codecs.TryGetValue(format, out codec!);
}
