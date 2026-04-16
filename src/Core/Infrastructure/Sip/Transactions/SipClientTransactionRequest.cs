using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;
using CalloraVoipSdk.Core.Infrastructure.Sip.Transport;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Transactions;

/// <summary>
/// Input model for one SIP client transaction execution.
/// </summary>
internal sealed class SipClientTransactionRequest
{
    /// <summary>
    /// SIP method sent by this transaction.
    /// </summary>
    public required string Method { get; init; }

    /// <summary>
    /// Request URI used on wire start-line.
    /// </summary>
    public required string RequestUri { get; init; }

    /// <summary>
    /// Request headers for the outbound SIP request.
    /// </summary>
    public required IReadOnlyDictionary<string, string> Headers { get; init; }

    /// <summary>
    /// Optional request body.
    /// </summary>
    public string? Body { get; init; }

    /// <summary>
    /// Remote endpoint target for this transaction.
    /// </summary>
    public required IPEndPoint RemoteEndPoint { get; init; }

    /// <summary>
    /// Transport protocol used for this transaction.
    /// </summary>
    public SipTransportProtocol Transport { get; init; } = SipTransportProtocol.Udp;

    /// <summary>
    /// Overall timeout waiting for final response.
    /// </summary>
    public TimeSpan Timeout { get; init; } = TimeSpan.FromSeconds(32);

    /// <summary>
    /// RFC3261 timer T1 baseline.
    /// </summary>
    public TimeSpan T1 { get; init; } = TimeSpan.FromMilliseconds(500);

    /// <summary>
    /// RFC3261 timer T2 upper retransmit interval.
    /// </summary>
    public TimeSpan T2 { get; init; } = TimeSpan.FromSeconds(4);

    /// <summary>
    /// Optional callback invoked for each provisional response.
    /// </summary>
    public Action<SipResponseEnvelope>? OnProvisionalResponse { get; init; }

    /// <summary>
    /// Optional callback invoked for retransmitted final INVITE failure responses
    /// (3xx-6xx over unreliable transport) during completed-state absorption.
    /// </summary>
    public Action<SipResponseEnvelope>? OnInviteFailureFinalResponseRetransmission { get; init; }

    /// <summary>
    /// Optional retention window for INVITE failure completed-state absorption (Timer D).
    /// Null uses RFC default behavior.
    /// </summary>
    public TimeSpan? InviteFailureCompletedRetention { get; init; }

    /// <summary>
    /// Optional retention window for non-INVITE completed-state absorption (Timer K/T4).
    /// Null uses RFC default behavior.
    /// </summary>
    public TimeSpan? NonInviteCompletedRetention { get; init; }
}
