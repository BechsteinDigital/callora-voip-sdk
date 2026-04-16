namespace CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

/// <summary>
/// TURN attribute type codes (RFC 8656 §18).
/// </summary>
internal enum TurnAttributeType : ushort
{
    /// <summary>CHANNEL-NUMBER (0x000C).</summary>
    ChannelNumber = 0x000C,

    /// <summary>LIFETIME (0x000D).</summary>
    Lifetime = 0x000D,

    /// <summary>XOR-PEER-ADDRESS (0x0012).</summary>
    XorPeerAddress = 0x0012,

    /// <summary>DATA (0x0013).</summary>
    Data = 0x0013,

    /// <summary>XOR-RELAYED-ADDRESS (0x0016).</summary>
    XorRelayedAddress = 0x0016,

    /// <summary>REQUESTED-ADDRESS-FAMILY (0x0017).</summary>
    RequestedAddressFamily = 0x0017,

    /// <summary>EVEN-PORT (0x0018).</summary>
    EvenPort = 0x0018,

    /// <summary>REQUESTED-TRANSPORT (0x0019).</summary>
    RequestedTransport = 0x0019,

    /// <summary>DONT-FRAGMENT (0x001A).</summary>
    DontFragment = 0x001A,

    /// <summary>RESERVATION-TOKEN (0x0022).</summary>
    ReservationToken = 0x0022,

    /// <summary>RFC 6062 CONNECTION-ID (0x002A).</summary>
    ConnectionId = 0x002A,

    /// <summary>ADDITIONAL-ADDRESS-FAMILY (0x8000).</summary>
    AdditionalAddressFamily = 0x8000,

    /// <summary>ADDRESS-ERROR-CODE (0x8001).</summary>
    AddressErrorCode = 0x8001,

    /// <summary>ICMP (0x8004).</summary>
    Icmp = 0x8004,

    /// <summary>RFC 8016 MOBILITY-TICKET (0x8030).</summary>
    MobilityTicket = 0x8030
}
