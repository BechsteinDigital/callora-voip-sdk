namespace CalloraVoipSdk;

/// <summary>
/// Mutable ICE options model for DI-based SDK configuration.
/// </summary>
public sealed class IceOptions
{
    /// <summary>
    /// Enables ICE candidate gathering and connectivity checks in call setup.
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// ICE helper servers used for candidate gathering.
    /// </summary>
    public List<IceServerConfiguration> Servers { get; set; } = [];

    /// <summary>
    /// Timeout applied per connectivity-check attempt.
    /// </summary>
    public TimeSpan ConnectivityCheckTimeout { get; set; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Number of retries per candidate pair before the pair is considered failed.
    /// </summary>
    public int ConnectivityCheckRetries { get; set; } = 1;

    internal IceConfiguration ToConfiguration() => new()
    {
        Enabled = Enabled,
        Servers = Servers,
        ConnectivityCheckTimeout = ConnectivityCheckTimeout,
        ConnectivityCheckRetries = ConnectivityCheckRetries
    };
}
