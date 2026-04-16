namespace CalloraVoipSdk.Core.Application.Ports.Audio;

/// <summary>
/// Describes one selectable audio endpoint for runtime input/output switching.
/// </summary>
public sealed class AudioDeviceDescriptor
{
    /// <summary>
    /// Creates one immutable device descriptor.
    /// </summary>
    /// <param name="id">
    /// Stable device identifier understood by the concrete audio implementation.
    /// Use <c>-1</c> to represent the platform default device.
    /// </param>
    /// <param name="name">Human-readable device name.</param>
    /// <param name="isDefault">True when this descriptor targets the platform default device.</param>
    public AudioDeviceDescriptor(string id, string name, bool isDefault)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Device id is required.", nameof(id));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Device name is required.", nameof(name));

        Id = id;
        Name = name;
        IsDefault = isDefault;
    }

    /// <summary>
    /// Device identifier expected by runtime control methods.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Human-readable device label.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Indicates whether this descriptor points to the platform default endpoint.
    /// </summary>
    public bool IsDefault { get; }
}
