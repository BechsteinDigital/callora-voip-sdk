using CalloraVoipSdk.Core.Application.Ports.Audio;

namespace CalloraVoipSdk;

/// <summary>
/// Runtime audio device control facade.
/// </summary>
public sealed class DeviceManager
{
    private readonly Func<IAudioDeviceRuntimeControl> _runtime;
    private readonly Action _throwIfDisposed;

    internal DeviceManager(
        Func<IAudioDeviceRuntimeControl> runtime,
        Action throwIfDisposed)
    {
        _runtime = runtime;
        _throwIfDisposed = throwIfDisposed;
    }

    public IReadOnlyList<AudioDeviceDescriptor> GetAvailableInputDevices()
    {
        _throwIfDisposed();
        return _runtime().GetAvailableInputDevices();
    }

    public IReadOnlyList<AudioDeviceDescriptor> GetAvailableOutputDevices()
    {
        _throwIfDisposed();
        return _runtime().GetAvailableOutputDevices();
    }

    public AudioDeviceRuntimeSnapshot GetRuntimeSnapshot()
    {
        _throwIfDisposed();
        return _runtime().GetRuntimeSnapshot();
    }

    public void SwitchInputDevice(string? deviceId)
    {
        _throwIfDisposed();
        _runtime().SwitchInputDevice(deviceId);
    }

    public void SwitchOutputDevice(string? deviceId)
    {
        _throwIfDisposed();
        _runtime().SwitchOutputDevice(deviceId);
    }

    public void SetInputVolume(float volume)
    {
        _throwIfDisposed();
        _runtime().SetInputVolume(volume);
    }

    public void SetOutputVolume(float volume)
    {
        _throwIfDisposed();
        _runtime().SetOutputVolume(volume);
    }

    public void SetInputMuted(bool isMuted)
    {
        _throwIfDisposed();
        _runtime().SetInputMuted(isMuted);
    }

    public void SetOutputMuted(bool isMuted)
    {
        _throwIfDisposed();
        _runtime().SetOutputMuted(isMuted);
    }

    public void UpdateFormat(AudioDeviceFormat format)
    {
        _throwIfDisposed();
        ArgumentNullException.ThrowIfNull(format);
        _runtime().UpdateFormat(format);
    }
}
