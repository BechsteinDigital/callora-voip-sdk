namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Represents one REGISTER transaction failure with SIP response context.
/// </summary>
internal sealed class SipRegistrationFailedException : InvalidOperationException
{
    /// <summary>
    /// Creates an exception carrying the failing SIP status code and reason phrase.
    /// </summary>
    public SipRegistrationFailedException(
        string message,
        int statusCode,
        string? reasonPhrase)
        : base(message)
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
    }

    /// <summary>
    /// SIP status code returned by the registrar.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// SIP reason phrase returned by the registrar.
    /// </summary>
    public string? ReasonPhrase { get; }
}
