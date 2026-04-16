namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN REQUESTED-ADDRESS-FAMILY attribute model (RFC 8656 §18.6).
/// </summary>
internal sealed class TurnRequestedAddressFamilyAttribute
{
    /// <summary>
    /// Requested relayed address family.
    /// </summary>
    public required TurnAddressFamily Family { get; init; }
}
