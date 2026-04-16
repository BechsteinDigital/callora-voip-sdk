using CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

namespace CalloraVoipSdk.Core.Infrastructure.Sdp.OfferAnswer;

/// <summary>
/// Result of SDP offer/answer negotiation.
/// </summary>
internal sealed class SdpOfferAnswerResult
{
    /// <summary>
    /// True when negotiation produced a compatible answer.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Negotiated answer model when success is true.
    /// </summary>
    public SdpSessionDescription? Answer { get; init; }

    /// <summary>
    /// Negotiated codec list for primary media stream.
    /// </summary>
    public IReadOnlyList<SdpCodecDefinition> NegotiatedCodecs { get; init; } = [];

    // -------------------------------------------------------------------------
    // Negotiated media parameters — convenience shortcuts for the media layer
    // -------------------------------------------------------------------------

    /// <summary>
    /// True when <c>a=rtcp-mux</c> was negotiated (RFC 5761).
    /// </summary>
    public bool RtcpMuxNegotiated { get; init; }

    /// <summary>
    /// DTLS fingerprint from the remote offer (RFC 8122).
    /// <see langword="null"/> when the remote side did not include a fingerprint.
    /// </summary>
    public SdpFingerprint? RemoteFingerprint { get; init; }

    /// <summary>
    /// DTLS setup role from the remote offer (RFC 4145).
    /// One of <c>actpass</c>, <c>active</c>, <c>passive</c>, or <c>holdconn</c>.
    /// <see langword="null"/> when not present in remote offer.
    /// </summary>
    public string? RemoteDtlsSetup { get; init; }

    /// <summary>
    /// First SDES crypto suite negotiated from the remote offer (RFC 4568).
    /// <see langword="null"/> when SDES was not offered or when DTLS is used instead.
    /// </summary>
    public SdpCryptoAttribute? NegotiatedCrypto { get; init; }
}
