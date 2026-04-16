namespace CalloraVoipSdk.Core.Infrastructure.Stun.Server;

/// <summary>
/// Runtime limits and backpressure configuration for <see cref="StunServer"/>.
/// </summary>
internal sealed class StunServerOptions
{
    /// <summary>
    /// Default options instance used when no explicit configuration is supplied.
    /// </summary>
    public static StunServerOptions Default { get; } = new();

    /// <summary>
    /// TCP listener backlog passed to <see cref="System.Net.Sockets.TcpListener.Start(int)"/>.
    /// Must be positive.
    /// </summary>
    public int TcpListenBacklog { get; init; } = 256;

    /// <summary>
    /// Maximum number of simultaneously active TCP/TLS client connections.
    /// Set to 0 for unlimited.
    /// </summary>
    public int MaxConcurrentStreamConnections { get; init; } = 1024;

    /// <summary>
    /// Connection handling behavior when <see cref="MaxConcurrentStreamConnections"/> is reached.
    /// </summary>
    public StunConnectionCapPolicy ConnectionCapPolicy { get; init; } = StunConnectionCapPolicy.Backpressure;

    /// <summary>
    /// Maximum number of concurrently processed UDP datagrams.
    /// Set to 0 for unlimited processing (not recommended for production).
    /// </summary>
    public int MaxConcurrentUdpPacketHandlers { get; init; } = 256;
}
