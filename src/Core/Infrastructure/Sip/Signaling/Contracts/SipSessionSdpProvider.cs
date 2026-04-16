using System.Net;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Provides SDP building and parsing capabilities to the SIP signaling layer
/// via function delegates — decoupling the SIP module from any SDP namespace.
/// Wired by the adapter/composition layer (SipCoreCallChannel, VoipClient).
/// </summary>
internal sealed class SipSessionSdpProvider
{
    /// <summary>
    /// Builds a SDP offer body (initial INVITE or hold re-INVITE).
    /// </summary>
    public required Func<IPEndPoint, bool, string> BuildOffer { get; init; }

    /// <summary>
    /// Negotiates a SDP answer against a remote offer. Returns null when negotiation fails.
    /// </summary>
    public required Func<string?, IPEndPoint, bool, string?> TryNegotiateAnswer { get; init; }

    /// <summary>
    /// Parses a remote SDP body and extracts RTP session parameters.
    /// Returns null when the SDP cannot be parsed or has no usable audio stream.
    /// </summary>
    public required Func<string, IPEndPoint, CallMediaParameters?> TryParseMediaParameters { get; init; }

    /// <summary>
    /// Returns true when the SDP signals remote hold semantics.
    /// </summary>
    public required Func<string?, bool> IsRemoteHold { get; init; }
}
