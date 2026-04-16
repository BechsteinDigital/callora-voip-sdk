namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Runtime limits and behavior for the TURN server.
/// </summary>
internal sealed class TurnServerOptions
{
    /// <summary>Default options instance.</summary>
    public static TurnServerOptions Default { get; } = new();

    /// <summary>
    /// TCP listen backlog for stream transports.
    /// </summary>
    public int TcpListenBacklog { get; init; } = 128;

    /// <summary>
    /// Maximum concurrent TCP/TLS client connections.
    /// 0 means unlimited.
    /// </summary>
    public int MaxConcurrentStreamConnections { get; init; } = 1024;

    /// <summary>
    /// Policy used when stream connection cap is reached.
    /// </summary>
    public TurnConnectionCapPolicy ConnectionCapPolicy { get; init; } = TurnConnectionCapPolicy.Backpressure;

    /// <summary>
    /// Maximum number of concurrently processed UDP datagrams.
    /// Set to 0 for unlimited processing (not recommended for production).
    /// </summary>
    public int MaxConcurrentUdpPacketHandlers { get; init; } = 256;

    /// <summary>
    /// Default allocation lifetime returned by Allocate success responses.
    /// </summary>
    public uint DefaultAllocationLifetimeSeconds { get; init; } = 600;

    /// <summary>
    /// Maximum allowed allocation lifetime.
    /// </summary>
    public uint MaxAllocationLifetimeSeconds { get; init; } = 3600;

    /// <summary>
    /// Permission lifetime in seconds.
    /// </summary>
    public uint PermissionLifetimeSeconds { get; init; } = 300;

    /// <summary>
    /// Channel binding lifetime in seconds.
    /// </summary>
    public uint ChannelBindingLifetimeSeconds { get; init; } = 600;

    /// <summary>
    /// Whether requests must be authenticated using long-term credentials.
    /// </summary>
    public bool RequireAuthentication { get; init; } = true;

    /// <summary>
    /// Enables RFC 8016 mobility ticket processing.
    /// </summary>
    public bool EnableMobility { get; init; }
}
