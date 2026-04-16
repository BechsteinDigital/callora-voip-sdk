namespace CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

/// <summary>
/// SDP media direction modes used in offer/answer negotiation.
/// </summary>
internal enum SdpMediaDirection
{
    SendRecv,
    SendOnly,
    RecvOnly,
    Inactive
}

