using CalloraVoipSdk.Audio.Abstractions.Domain.Devices;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Audio;

namespace CalloraVoipSdk.Audio.Headless;

/// <summary>
/// No-op audio provider for server and test environments.
/// </summary>
public sealed class HeadlessAudioDevice : IAudioDeviceProvider
{
    /// <inheritdoc />
    public string Name => "HeadlessAudioDevice";

    /// <inheritdoc />
    public void Connect(IMediaReceiver receiver, IMediaSender sender, AudioConnectionParameters parameters)
    {
        ArgumentNullException.ThrowIfNull(receiver);
        ArgumentNullException.ThrowIfNull(sender);
        ArgumentNullException.ThrowIfNull(parameters);
    }

    /// <inheritdoc />
    public void Disconnect()
    {
    }
}
