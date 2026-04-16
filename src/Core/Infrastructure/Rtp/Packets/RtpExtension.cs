namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Packets;

/// <summary>
/// RTP header extension (RFC 3550 §5.3.1).
/// The profile field identifies the extension format; data is the raw extension body
/// excluding the 4-byte profile+length prefix.
/// </summary>
internal sealed class RtpExtension
{
    /// <summary>
    /// Profile-defined extension identifier (the 16-bit value in the extension header).
    /// Common profiles: 0xBEDE (RFC 5285 one-byte), 0x1000 (RFC 5285 two-byte).
    /// </summary>
    public ushort Profile { get; init; }

    /// <summary>
    /// Extension data, without the 4-byte profile+length prefix.
    /// Length is always a multiple of 4 bytes (padded with zeros if necessary per RFC 3550 §5.3.1).
    /// </summary>
    public ReadOnlyMemory<byte> Data { get; init; }
}
