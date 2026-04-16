namespace CalloraVoipSdk.Core.Domain.Lines;

/// <summary>
/// Describes why a phone line permanently failed to re-register and entered
/// <see cref="LineState.Failed"/>.
/// </summary>
public enum ReregisterFailReason
{
    /// <summary>
    /// The maximum number of reconnect attempts configured in
    /// <see cref="ReregisterOptions.MaxRetries"/> was exceeded without a successful registration.
    /// </summary>
    MaxRetriesExceeded,

    /// <summary>
    /// The SIP server rejected the credentials with a 401 or 403 response.
    /// Retrying with the same credentials would not succeed.
    /// </summary>
    AuthenticationFailed
}
