namespace CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

/// <summary>
/// One decoded frame from a TURN stream transport.
/// </summary>
internal sealed class TurnStreamFrame
{
    /// <summary>
    /// True when this frame is ChannelData; false when it is a STUN packet.
    /// </summary>
    public required bool IsChannelData { get; init; }

    /// <summary>
    /// Channel number when <see cref="IsChannelData"/> is true.
    /// </summary>
    public ushort ChannelNumber { get; init; }

    /// <summary>
    /// Frame payload bytes: ChannelData body or full STUN packet.
    /// </summary>
    public required byte[] Payload { get; init; }
}
