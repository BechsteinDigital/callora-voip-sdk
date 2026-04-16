using CalloraVoipSdk.Core.Application.Media;

namespace CalloraVoipSdk.Core.Application.Ports.Audio;

/// <summary>
/// Audio device abstraction used by the application layer.
/// Implement this interface to plug in platform-specific audio I/O.
/// </summary>
public interface IAudioDevice
{
    /// <summary>Human-readable device name for diagnostics.</summary>
    string Name { get; }

    /// <summary>
    /// Connects the device to a call's media pipeline.
    /// After this call the device starts capturing microphone audio and playing received audio.
    /// <paramref name="parameters"/> carries the negotiated codec so the device can open
    /// hardware streams at the correct sample rate and apply the right encode/decode path.
    /// </summary>
    void Connect(IMediaReceiver receiver, IMediaSender sender, AudioConnectionParameters parameters);

    /// <summary>Disconnects from the current call and stops all audio I/O.</summary>
    void Disconnect();
}
