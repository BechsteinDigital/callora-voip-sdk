using CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Client;

/// <summary>
/// Allocation options for TURN Allocate requests.
/// </summary>
internal sealed class TurnAllocateOptions
{
    /// <summary>
    /// Requested transport protocol for the relay allocation.
    /// Defaults to UDP.
    /// </summary>
    public TurnRequestedTransportProtocol RequestedTransport { get; init; } = TurnRequestedTransportProtocol.Udp;

    /// <summary>
    /// Optional requested relayed address family.
    /// When null, no REQUESTED-ADDRESS-FAMILY attribute is sent.
    /// </summary>
    public TurnAddressFamily? RequestedAddressFamily { get; init; }

    /// <summary>
    /// Optional requested allocation lifetime in seconds.
    /// </summary>
    public uint? LifetimeSeconds { get; init; }

    /// <summary>
    /// Whether to include DONT-FRAGMENT.
    /// </summary>
    public bool DontFragment { get; init; }

    /// <summary>
    /// Whether to request an even relay port and reserve the next one.
    /// </summary>
    public bool ReserveEvenPort { get; init; }

    /// <summary>
    /// Optional reservation token for paired-port allocations.
    /// </summary>
    public ulong? ReservationToken { get; init; }

    /// <summary>
    /// When true, requests an RFC 8016 mobility ticket by sending a zero-length
    /// MOBILITY-TICKET attribute in Allocate.
    /// </summary>
    public bool RequestMobilityTicket { get; init; }
}
