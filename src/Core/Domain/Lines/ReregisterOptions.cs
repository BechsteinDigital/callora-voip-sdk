namespace CalloraVoipSdk.Core.Domain.Lines;

/// <summary>
/// Controls automatic re-registration behavior when a SIP registration is lost due to a network
/// disruption or server restart.
/// </summary>
public sealed class ReregisterOptions
{
    /// <summary>
    /// Default options: auto-reregister enabled, unlimited retries, exponential backoff 2 s – 60 s.
    /// </summary>
    public static readonly ReregisterOptions Default = new();

    /// <summary>
    /// Disabled options: no automatic re-registration; line transitions directly to
    /// <see cref="LineState.RegistrationFailed"/> on disruption.
    /// </summary>
    public static readonly ReregisterOptions Disabled = new() { AutoReregister = false };

    /// <summary>
    /// When <see langword="true"/> the SDK automatically attempts to re-register after a
    /// connection disruption. Default: <see langword="true"/>.
    /// </summary>
    public bool AutoReregister { get; init; } = true;

    /// <summary>
    /// Maximum number of reconnect attempts before the line permanently transitions to
    /// <see cref="LineState.Failed"/>.  A value of <c>0</c> means unlimited retries.
    /// Default: <c>0</c>.
    /// </summary>
    public int MaxRetries { get; init; } = 0;

    /// <summary>
    /// Delay before the first reconnect attempt and base for exponential backoff.
    /// Default: 2 seconds.
    /// </summary>
    public TimeSpan InitialRetryDelay { get; init; } = TimeSpan.FromSeconds(2);

    /// <summary>
    /// Upper bound on the delay between reconnect attempts (caps exponential growth).
    /// Default: 60 seconds.
    /// </summary>
    public TimeSpan MaxRetryDelay { get; init; } = TimeSpan.FromSeconds(60);
}
