namespace CalloraVoipSdk;

/// <summary>
/// Controls ICE gathering and connectivity checks for call media sessions.
/// </summary>
public sealed class IceConfiguration
{
    /// <summary>
    /// Enables ICE candidate gathering and connectivity checks in call setup.
    /// </summary>
    public bool Enabled { get; init; }

    /// <summary>
    /// ICE helper servers used for candidate gathering.
    /// </summary>
    public IReadOnlyList<IceServerConfiguration> Servers { get; init; } = [];

    /// <summary>
    /// Timeout applied per connectivity-check attempt.
    /// </summary>
    public TimeSpan ConnectivityCheckTimeout { get; init; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Number of retries per candidate pair before the pair is considered failed.
    /// </summary>
    public int ConnectivityCheckRetries { get; init; } = 1;
}
