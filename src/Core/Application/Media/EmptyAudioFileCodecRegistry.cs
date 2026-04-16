using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Application.Media;

internal sealed class EmptyAudioFileCodecRegistry : IAudioFileCodecRegistry
{
    public static readonly EmptyAudioFileCodecRegistry Instance = new();

    public bool TryGetCodec(AudioFileFormat format, out IAudioFileCodec codec)
    {
        codec = null!;
        return false;
    }
}
