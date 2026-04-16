namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Service contract for SIP registrar signaling flows.
/// </summary>
internal interface ISipRegistrationService
{
    /// <summary>
    /// Sends REGISTER and waits for final success response.
    /// </summary>
    Task<SipRegistrationResult> RegisterAsync(
        SipRegistrationRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Sends unREGISTER (Contact with <c>Expires: 0</c>) for the specific binding
    /// and waits for final success response.
    /// </summary>
    Task<SipRegistrationResult> UnregisterAsync(
        SipRegistrationRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Sends unREGISTER with wildcard <c>Contact: *</c> and <c>Expires: 0</c>
    /// to remove all bindings for the address-of-record (RFC 3261 §10.2.2).
    /// </summary>
    Task<SipRegistrationResult> UnregisterAllAsync(
        SipRegistrationRequest request,
        CancellationToken ct = default);

    /// <summary>
    /// Sends REGISTER without a Contact header to fetch the current binding list
    /// from the registrar (RFC 3261 §10.2.3). Returns bindings via
    /// <see cref="SipRegistrationResult.RegisteredBindings"/>.
    /// </summary>
    Task<SipRegistrationResult> FetchBindingsAsync(
        SipRegistrationRequest request,
        CancellationToken ct = default);
}
