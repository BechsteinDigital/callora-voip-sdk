namespace CalloraVoipSdk;

/// <summary>
/// Defines one ICE helper server entry used by the call media runtime.
/// </summary>
public sealed class IceServerConfiguration
{
    /// <summary>
    /// Server type (<see cref="IceServerType.Stun"/> or <see cref="IceServerType.Turn"/>).
    /// </summary>
    public IceServerType Type { get; init; } = IceServerType.Stun;

    /// <summary>
    /// Hostname or IP address of the ICE helper server.
    /// </summary>
    public required string Host { get; init; }

    /// <summary>
    /// Optional explicit server port. When null, protocol defaults are used.
    /// </summary>
    public int? Port { get; init; }

    /// <summary>
    /// Transport protocol used to connect to the server.
    /// </summary>
    public IceTransport Transport { get; init; } = IceTransport.Udp;

    /// <summary>
    /// Optional username for authenticated STUN/TURN requests.
    /// </summary>
    public string? Username { get; init; }

    /// <summary>
    /// Optional password for authenticated STUN/TURN requests.
    /// </summary>
    public string? Password { get; init; }
}
