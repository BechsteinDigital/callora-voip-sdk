namespace CalloraVoipSdk;

/// <summary>
/// Options for convenience registration flows.
/// </summary>
public sealed class ConnectOptions
{
    /// <summary>
    /// Default connect options.
    /// </summary>
    public static ConnectOptions Default { get; } = new();

    /// <summary>
    /// Maximum time to wait until the line reaches <see cref="Domain.Lines.LineState.Registered"/>.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(15);

    /// <summary>
    /// Completes with <see cref="ConnectStatus.Failed"/> as soon as the line enters
    /// <see cref="Domain.Lines.LineState.RegistrationFailed"/>.
    /// </summary>
    public bool FailFastOnRegistrationFailed { get; init; } = true;
}
