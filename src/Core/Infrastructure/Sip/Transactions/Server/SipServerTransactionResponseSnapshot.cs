namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transactions.Server;

/// <summary>
/// Immutable response snapshot stored for SIP server-transaction retransmission behavior.
/// </summary>
internal sealed class SipServerTransactionResponseSnapshot
{
    /// <summary>
    /// Creates one immutable response snapshot.
    /// </summary>
    public SipServerTransactionResponseSnapshot(
        int statusCode,
        string reasonPhrase,
        IReadOnlyDictionary<string, string> headers,
        string? body)
    {
        StatusCode = statusCode;
        ReasonPhrase = reasonPhrase;
        Headers = headers;
        Body = body;
    }

    /// <summary>
    /// SIP response status code.
    /// </summary>
    public int StatusCode { get; }

    /// <summary>
    /// SIP response reason phrase.
    /// </summary>
    public string ReasonPhrase { get; }

    /// <summary>
    /// SIP response headers.
    /// </summary>
    public IReadOnlyDictionary<string, string> Headers { get; }

    /// <summary>
    /// SIP response body.
    /// </summary>
    public string? Body { get; }
}
