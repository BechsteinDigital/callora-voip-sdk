using System.Net;
using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Response envelope that keeps remote endpoint metadata with one parsed SIP response.
/// </summary>
internal readonly record struct SipResponseEnvelope(
    IPEndPoint RemoteEndPoint,
    SipResponse Response);
