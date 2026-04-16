namespace CalloraVoipSdk.Core.Application.Ports.Audio;

/// <summary>
/// Snapshot of runtime audio device state used by SDK diagnostics and UI controls.
/// </summary>
public sealed class AudioDeviceRuntimeSnapshot
{
    /// <summary>
    /// Creates one runtime state snapshot.
    /// </summary>
    /// <param name="isConnected">True when the audio device is currently connected to media legs.</param>
    /// <param name="inputDeviceId">Selected input device id.</param>
    /// <param name="outputDeviceId">Selected output device id.</param>
    /// <param name="inputMuted">Input mute state.</param>
    /// <param name="outputMuted">Output mute state.</param>
    /// <param name="inputVolume">Input gain factor in range 0..2.</param>
    /// <param name="outputVolume">Output gain factor in range 0..2.</param>
    /// <param name="format">Active capture/playback device format.</param>
    public AudioDeviceRuntimeSnapshot(
        bool isConnected,
        string inputDeviceId,
        string outputDeviceId,
        bool inputMuted,
        bool outputMuted,
        float inputVolume,
        float outputVolume,
        AudioDeviceFormat format)
    {
        if (string.IsNullOrWhiteSpace(inputDeviceId))
            throw new ArgumentException("Input device id is required.", nameof(inputDeviceId));
        if (string.IsNullOrWhiteSpace(outputDeviceId))
            throw new ArgumentException("Output device id is required.", nameof(outputDeviceId));
        ArgumentNullException.ThrowIfNull(format);

        IsConnected = isConnected;
        InputDeviceId = inputDeviceId;
        OutputDeviceId = outputDeviceId;
        InputMuted = inputMuted;
        OutputMuted = outputMuted;
        InputVolume = inputVolume;
        OutputVolume = outputVolume;
        Format = format;
    }

    /// <summary>
    /// True when capture/playback streams are currently active.
    /// </summary>
    public bool IsConnected { get; }

    /// <summary>
    /// Selected input device id.
    /// </summary>
    public string InputDeviceId { get; }

    /// <summary>
    /// Selected output device id.
    /// </summary>
    public string OutputDeviceId { get; }

    /// <summary>
    /// True when microphone input is muted.
    /// </summary>
    public bool InputMuted { get; }

    /// <summary>
    /// True when speaker output is muted.
    /// </summary>
    public bool OutputMuted { get; }

    /// <summary>
    /// Input gain factor in range 0..2.
    /// </summary>
    public float InputVolume { get; }

    /// <summary>
    /// Output gain factor in range 0..2.
    /// </summary>
    public float OutputVolume { get; }

    /// <summary>
    /// Active capture/playback format.
    /// </summary>
    public AudioDeviceFormat Format { get; }
}
