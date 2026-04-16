namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN CHANNEL-NUMBER attribute model (RFC 8656 §18.1).
/// </summary>
internal sealed class TurnChannelNumberAttribute
{
    /// <summary>
    /// Channel number in the TURN channel range (0x4000–0x7FFF).
    /// </summary>
    public required ushort ChannelNumber { get; init; }
}
