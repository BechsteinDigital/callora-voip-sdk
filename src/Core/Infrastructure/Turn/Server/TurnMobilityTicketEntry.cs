namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Server-side mobility ticket state mapping to an allocation key.
/// </summary>
internal sealed class TurnMobilityTicketEntry
{
    /// <summary>
    /// Allocation key associated with the ticket.
    /// </summary>
    public required string AllocationKey { get; init; }

    /// <summary>
    /// Ticket expiry in UTC.
    /// </summary>
    public required DateTimeOffset ExpiresAtUtc { get; init; }
}
