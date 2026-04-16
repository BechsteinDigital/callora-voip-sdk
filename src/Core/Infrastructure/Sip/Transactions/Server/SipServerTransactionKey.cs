using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transactions.Server;

/// <summary>
/// Unique server-transaction identity based on RFC3261 matching fields.
/// </summary>
internal readonly record struct SipServerTransactionKey
{
    /// <summary>
    /// SIP Call-ID associated with the transaction.
    /// </summary>
    public required string CallId { get; init; }

    /// <summary>
    /// Top Via branch value.
    /// </summary>
    public required string Branch { get; init; }

    /// <summary>
    /// Top Via sent-by value (`host[:port]`) used in RFC3261 matching.
    /// </summary>
    public required string SentBy { get; init; }

    /// <summary>
    /// Numeric CSeq value.
    /// </summary>
    public required int CSeqNumber { get; init; }

    /// <summary>
    /// CSeq method token.
    /// </summary>
    public required string Method { get; init; }

    /// <summary>
    /// Builds a key from an inbound request. ACK maps to INVITE transaction key.
    /// </summary>
    public static bool TryFromRequest(
        SipRequest request,
        out SipServerTransactionKey key)
    {
        key = default;
        var callId = request.Header("Call-ID");
        var topVia = SipProtocol.ExtractTopViaEntry(request.Header("Via"));
        var viaBranch = SipProtocol.ExtractViaBranch(request.Header("Via"));
        var viaSentBy = SipProtocol.ExtractViaSentBy(request.Header("Via"));
        var cseqHeader = request.Header("CSeq");
        var cseqNumber = SipProtocol.ExtractCSeqNumber(cseqHeader);
        var cseqMethod = SipProtocol.ExtractCSeqMethod(cseqHeader);
        if (string.IsNullOrWhiteSpace(callId)
            || string.IsNullOrWhiteSpace(topVia)
            || cseqNumber <= 0
            || string.IsNullOrWhiteSpace(cseqMethod))
        {
            return false;
        }

        var requestMethod = request.Method.ToUpperInvariant();
        var normalizedMethod = requestMethod == "ACK" ? "INVITE" : cseqMethod;
        string normalizedBranch;
        string normalizedSentBy;

        if (!string.IsNullOrWhiteSpace(viaBranch) && SipProtocol.HasMagicCookie(viaBranch))
        {
            if (string.IsNullOrWhiteSpace(viaSentBy))
                return false;

            normalizedBranch = viaBranch;
            normalizedSentBy = viaSentBy;
        }
        else
        {
            var requestUri = request.RequestUri.Trim();
            var fromTag = SipProtocol.ExtractTag(request.Header("From")) ?? string.Empty;
            var toTag = SipProtocol.ExtractTag(request.Header("To")) ?? string.Empty;
            normalizedBranch = $"legacy|{topVia}|{requestUri}|from={fromTag}|to={toTag}";
            normalizedSentBy = viaSentBy ?? topVia;
        }

        key = new SipServerTransactionKey
        {
            CallId = callId,
            Branch = normalizedBranch,
            SentBy = normalizedSentBy,
            CSeqNumber = cseqNumber,
            Method = normalizedMethod.ToUpperInvariant()
        };
        return true;
    }
}
