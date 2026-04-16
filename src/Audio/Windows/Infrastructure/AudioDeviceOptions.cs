namespace CalloraVoipSdk.Audio.Windows;

public sealed class AudioDeviceOptions
{
    /// <summary>
    /// WaveIn device number. -1 = default system microphone.
    /// Use <see cref="WindowsAudioDevice.GetInputDevices"/> to enumerate.
    /// </summary>
    public int InputDeviceNumber { get; init; } = -1;

    /// <summary>
    /// WaveOut device number. -1 = default system speaker.
    /// Use <see cref="WindowsAudioDevice.GetOutputDevices"/> to enumerate.
    /// </summary>
    public int OutputDeviceNumber { get; init; } = -1;

    /// <summary>
    /// RTP audio sample rate. Must match the selected codec.
    /// G.711 = 8000, G.722 = 16000.
    /// </summary>
    public int SampleRate { get; init; } = 8000;

    /// <summary>PCM bit depth. Standard: 16.</summary>
    public int BitsPerSample { get; init; } = 16;

    /// <summary>Mono (1) or stereo (2). SIP audio is always mono.</summary>
    public int Channels { get; init; } = 1;
}
