namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN EVEN-PORT attribute model (RFC 8656 §18.7).
/// </summary>
internal sealed class TurnEvenPortAttribute
{
    /// <summary>
    /// True when the server should reserve the next higher port.
    /// </summary>
    public bool ReserveNextPort { get; init; }
}
