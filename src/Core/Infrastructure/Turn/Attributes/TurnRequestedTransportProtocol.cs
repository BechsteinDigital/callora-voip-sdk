namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN REQUESTED-TRANSPORT protocol numbers (RFC 8656 §18.8).
/// </summary>
internal enum TurnRequestedTransportProtocol : byte
{
    /// <summary>Transmission Control Protocol (TCP, protocol number 6).</summary>
    Tcp = 0x06,

    /// <summary>User Datagram Protocol (UDP, protocol number 17).</summary>
    Udp = 0x11
}
