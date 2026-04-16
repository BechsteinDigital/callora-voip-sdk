namespace CalloraVoipSdk.Core.Infrastructure.Sdp.Models;

/// <summary>
/// Codec descriptor used by SDP media lines and negotiation logic.
/// </summary>
internal sealed class SdpCodecDefinition
{
    /// <summary>
    /// RTP payload type number.
    /// </summary>
    public required int PayloadType { get; init; }

    /// <summary>
    /// Codec name (for example PCMU, PCMA, G722).
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Codec sampling rate in Hz.
    /// </summary>
    public required int ClockRate { get; init; }

    /// <summary>
    /// Number of audio channels (encoding-params in rtpmap, default 1).
    /// </summary>
    public int Channels { get; init; } = 1;
}

