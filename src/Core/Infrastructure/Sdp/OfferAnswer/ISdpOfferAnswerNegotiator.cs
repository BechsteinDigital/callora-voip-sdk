using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

namespace CalloraVoipSdk.Core.Infrastructure.Sdp.OfferAnswer;

/// <summary>
/// Performs SDP offer generation and offer/answer negotiation.
/// </summary>
internal interface ISdpOfferAnswerNegotiator
{
    /// <summary>
    /// Creates a local offer model for one media endpoint and codec set.
    /// When <paramref name="options"/> carries DTLS parameters the profile is set to
    /// <c>UDP/TLS/RTP/SAVPF</c> and <c>a=fingerprint</c>/<c>a=setup</c> are emitted.
    /// </summary>
    SdpSessionDescription CreateOffer(
        IPEndPoint localEndPoint,
        IReadOnlyList<SdpCodecDefinition> codecs,
        SdpMediaDirection direction,
        SdpMediaOptions? options = null);

    /// <summary>
    /// Negotiates a local answer against a remote offer and local capabilities.
    /// Carries through rtcp-mux, BUNDLE/MID, SDES crypto, and DTLS fingerprint/setup
    /// from the remote offer. When <paramref name="localOptions"/> is provided its
    /// DTLS and ICE parameters are added to the answer.
    /// </summary>
    SdpOfferAnswerResult NegotiateAnswer(
        SdpSessionDescription remoteOffer,
        IPEndPoint localEndPoint,
        IReadOnlyList<SdpCodecDefinition> localCapabilities,
        SdpMediaDirection localDirection,
        SdpMediaOptions? localOptions = null);
}
