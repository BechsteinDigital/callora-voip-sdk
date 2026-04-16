using CalloraVoipSdk.Core.Infrastructure.Rtp.Packets;

namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Wire;

/// <summary>
/// Encodes and decodes RTP packets to and from their binary wire format (RFC 3550 §5).
/// </summary>
internal interface IRtpPacketCodec
{
    /// <summary>
    /// Decodes a raw UDP datagram into an <see cref="RtpPacket"/>.
    /// Throws <see cref="FormatException"/> when the datagram is shorter than the
    /// minimum 12-byte RTP header or carries an unsupported version.
    /// </summary>
    RtpPacket Decode(ReadOnlySpan<byte> datagram);

    /// <summary>
    /// Encodes an <see cref="RtpPacket"/> to its binary wire representation.
    /// </summary>
    byte[] Encode(RtpPacket packet);
}
