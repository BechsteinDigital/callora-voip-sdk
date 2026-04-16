namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN RESERVATION-TOKEN attribute model (RFC 8656 §18.10).
/// </summary>
internal sealed class TurnReservationTokenAttribute
{
    /// <summary>
    /// 64-bit reservation token.
    /// </summary>
    public required ulong Token { get; init; }
}
