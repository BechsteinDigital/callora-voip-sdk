namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Packets;

/// <summary>
/// RTP data packet (RFC 3550 §5).
/// Carries one encoded audio/video frame with its timing and source identity.
/// </summary>
internal sealed class RtpPacket
{
    /// <summary>RTP version, always 2 (RFC 3550 §5.1).</summary>
    public byte Version { get; init; } = 2;

    /// <summary>
    /// Padding flag — when true, the packet contains one or more additional
    /// padding octets at the end; the last octet carries the total padding count
    /// (RFC 3550 §5.1).
    /// </summary>
    public bool Padding { get; init; }

    /// <summary>
    /// Marker bit — semantics defined by the payload format profile.
    /// For audio, typically set on the first packet after silence (RFC 3550 §5.1).
    /// </summary>
    public bool Marker { get; init; }

    /// <summary>Payload type — identifies the codec and clock-rate (RFC 3550 §5.1).</summary>
    public byte PayloadType { get; init; }

    /// <summary>
    /// Sequence number — incremented by one for each RTP packet sent.
    /// Wraps around from 65535 to 0 (RFC 3550 §5.1).
    /// </summary>
    public ushort SequenceNumber { get; init; }

    /// <summary>
    /// Sampling instant of the first octet of the payload.
    /// Clock rate depends on the payload type (RFC 3550 §5.1).
    /// </summary>
    public uint Timestamp { get; init; }

    /// <summary>Synchronization source identifier, chosen randomly (RFC 3550 §5.1).</summary>
    public uint Ssrc { get; init; }

    /// <summary>
    /// Contributing source identifiers — populated by mixers (RFC 3550 §5.1).
    /// Maximum 15 entries.
    /// </summary>
    public IReadOnlyList<uint> Csrc { get; init; } = [];

    /// <summary>Optional header extension (RFC 3550 §5.3.1). Null when not present.</summary>
    public RtpExtension? HeaderExtension { get; init; }

    /// <summary>
    /// Raw payload octets, with any padding already stripped.
    /// Empty when the packet carries no payload.
    /// </summary>
    public ReadOnlyMemory<byte> Payload { get; init; }
}
