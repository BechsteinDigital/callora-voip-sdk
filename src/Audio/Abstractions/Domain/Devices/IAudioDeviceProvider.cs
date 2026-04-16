using CalloraVoipSdk.Core.Application.Ports.Audio;

namespace CalloraVoipSdk.Audio.Abstractions.Domain.Devices;

/// <summary>
/// Marker abstraction for pluggable audio providers.
/// </summary>
public interface IAudioDeviceProvider : IAudioDevice
{
}
