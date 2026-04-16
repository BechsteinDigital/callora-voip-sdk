using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Audio;

namespace CalloraVoipSdk.Core.Infrastructure.Audio;

/// <summary>
/// Default audio device: sends silence, discards received audio.
/// Used until a real audio device is configured.
/// </summary>
internal sealed class SilenceAudioDevice : IAudioDevice, IAudioDeviceRuntimeControl
{
    /// <summary>Shared singleton instance for silence-device fallback scenarios.</summary>
    public static readonly SilenceAudioDevice Instance = new();

    /// <inheritdoc />
    public string Name => "Silence";

    /// <inheritdoc />
    public void Connect(IMediaReceiver receiver, IMediaSender sender, AudioConnectionParameters parameters) { }

    /// <inheritdoc />
    public void Disconnect() { }

    /// <inheritdoc />
    public IReadOnlyList<AudioDeviceDescriptor> GetAvailableInputDevices() =>
    [
        new AudioDeviceDescriptor("-1", "Silence Input", isDefault: true)
    ];

    /// <inheritdoc />
    public IReadOnlyList<AudioDeviceDescriptor> GetAvailableOutputDevices() =>
    [
        new AudioDeviceDescriptor("-1", "Silence Output", isDefault: true)
    ];

    /// <inheritdoc />
    public AudioDeviceRuntimeSnapshot GetRuntimeSnapshot() => new(
        isConnected: false,
        inputDeviceId: "-1",
        outputDeviceId: "-1",
        inputMuted: true,
        outputMuted: true,
        inputVolume: 0f,
        outputVolume: 0f,
        format: AudioDeviceFormat.Default);

    /// <inheritdoc />
    public void SwitchInputDevice(string? deviceId) { }

    /// <inheritdoc />
    public void SwitchOutputDevice(string? deviceId) { }

    /// <inheritdoc />
    public void SetInputVolume(float volume) { }

    /// <inheritdoc />
    public void SetOutputVolume(float volume) { }

    /// <inheritdoc />
    public void SetInputMuted(bool isMuted) { }

    /// <inheritdoc />
    public void SetOutputMuted(bool isMuted) { }

    /// <inheritdoc />
    public void UpdateFormat(AudioDeviceFormat format) { }
}
