using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Represents a SIP transaction failure that ended with a non-success final SIP response.
/// </summary>
internal sealed class SipFinalResponseException : InvalidOperationException
{
    /// <summary>
    /// Creates one exception for a non-success final SIP response.
    /// </summary>
    public SipFinalResponseException(
        string message,
        SipResponseEnvelope finalResponse)
        : base(message)
    {
        FinalResponse = finalResponse;
    }

    /// <summary>
    /// Creates one exception for a non-success final SIP response with inner exception context.
    /// </summary>
    public SipFinalResponseException(
        string message,
        SipResponseEnvelope finalResponse,
        Exception innerException)
        : base(message, innerException)
    {
        FinalResponse = finalResponse;
    }

    /// <summary>
    /// Final non-success SIP response observed by the transaction.
    /// </summary>
    public SipResponseEnvelope FinalResponse { get; }
}
