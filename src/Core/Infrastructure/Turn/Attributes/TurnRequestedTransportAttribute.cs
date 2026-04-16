namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN REQUESTED-TRANSPORT attribute model (RFC 8656 §18.8).
/// </summary>
internal sealed class TurnRequestedTransportAttribute
{
    /// <summary>
    /// Requested transport protocol for the allocation.
    /// </summary>
    public required TurnRequestedTransportProtocol Protocol { get; init; }
}
