namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// RFC 8016 MOBILITY-TICKET attribute model.
/// </summary>
internal sealed class TurnMobilityTicketAttribute
{
    /// <summary>
    /// Opaque ticket payload.
    /// </summary>
    public required ReadOnlyMemory<byte> Ticket { get; init; }
}
