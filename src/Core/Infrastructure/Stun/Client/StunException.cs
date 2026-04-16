namespace CalloraVoipSdk.Core.Infrastructure.Stun.Client;

/// <summary>
/// Exception raised when a STUN operation fails due to a protocol error,
/// an error response from the server, or exhaustion of all retransmission attempts.
/// </summary>
internal class StunException : Exception
{
    /// <summary>Initialises with a detail message.</summary>
    public StunException(string message) : base(message) { }

    /// <summary>Initialises with a detail message and a causal inner exception.</summary>
    public StunException(string message, Exception inner) : base(message, inner) { }
}
