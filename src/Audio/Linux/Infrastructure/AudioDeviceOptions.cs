namespace CalloraVoipSdk.Audio.Linux;

public sealed class AudioDeviceOptions
{
    /// <summary>
    /// PortAudio input device index. -1 = default system microphone.
    /// Use <see cref="LinuxAudioDevice.GetInputDevices"/> to enumerate.
    /// </summary>
    public int InputDeviceIndex { get; init; } = -1;

    /// <summary>
    /// PortAudio output device index. -1 = default system speaker.
    /// Use <see cref="LinuxAudioDevice.GetOutputDevices"/> to enumerate.
    /// </summary>
    public int OutputDeviceIndex { get; init; } = -1;

    /// <summary>G.711 = 8000 Hz. Must match negotiated codec.</summary>
    public int SampleRate { get; init; } = 8000;

    /// <summary>Frames per PortAudio callback buffer. 160 = 20 ms @ 8 kHz.</summary>
    public uint FramesPerBuffer { get; init; } = 160;
}
