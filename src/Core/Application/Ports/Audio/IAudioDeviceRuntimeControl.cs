namespace CalloraVoipSdk.Core.Application.Ports.Audio;

/// <summary>
/// Optional runtime control port for audio devices.
/// Provides hot-switch, mute, volume, and format controls while calls are active.
/// </summary>
public interface IAudioDeviceRuntimeControl
{
    /// <summary>
    /// Lists available input endpoints.
    /// </summary>
    IReadOnlyList<AudioDeviceDescriptor> GetAvailableInputDevices();

    /// <summary>
    /// Lists available output endpoints.
    /// </summary>
    IReadOnlyList<AudioDeviceDescriptor> GetAvailableOutputDevices();

    /// <summary>
    /// Returns a consistent point-in-time runtime snapshot.
    /// </summary>
    AudioDeviceRuntimeSnapshot GetRuntimeSnapshot();

    /// <summary>
    /// Switches microphone endpoint at runtime.
    /// </summary>
    /// <param name="deviceId">
    /// Device identifier from <see cref="GetAvailableInputDevices"/>.
    /// Use <c>-1</c>, <c>null</c>, or empty to select the platform default device.
    /// </param>
    void SwitchInputDevice(string? deviceId);

    /// <summary>
    /// Switches speaker endpoint at runtime.
    /// </summary>
    /// <param name="deviceId">
    /// Device identifier from <see cref="GetAvailableOutputDevices"/>.
    /// Use <c>-1</c>, <c>null</c>, or empty to select the platform default device.
    /// </param>
    void SwitchOutputDevice(string? deviceId);

    /// <summary>
    /// Sets microphone input gain factor.
    /// </summary>
    /// <param name="volume">Linear gain in range 0..2 (0 = silence, 1 = neutral).</param>
    void SetInputVolume(float volume);

    /// <summary>
    /// Sets speaker output gain factor.
    /// </summary>
    /// <param name="volume">Linear gain in range 0..2 (0 = silence, 1 = neutral).</param>
    void SetOutputVolume(float volume);

    /// <summary>
    /// Mutes or unmutes microphone input.
    /// </summary>
    void SetInputMuted(bool isMuted);

    /// <summary>
    /// Mutes or unmutes speaker playback.
    /// </summary>
    void SetOutputMuted(bool isMuted);

    /// <summary>
    /// Updates runtime capture/playback format.
    /// The implementation applies supported changes immediately.
    /// </summary>
    /// <param name="format">Requested runtime format.</param>
    void UpdateFormat(AudioDeviceFormat format);
}
