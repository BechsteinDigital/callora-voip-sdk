using System.Buffers.Binary;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

/// <summary>
/// Codec for TURN ChannelData framing (RFC 8656 §11.6).
/// </summary>
internal static class TurnChannelDataCodec
{
    /// <summary>
    /// Tries to parse a datagram as ChannelData packet.
    /// </summary>
    public static bool TryParse(ReadOnlySpan<byte> packet, out ushort channelNumber, out byte[] data)
    {
        channelNumber = 0;
        data = Array.Empty<byte>();

        if (packet.Length < 4)
            return false;

        var channel = BinaryPrimitives.ReadUInt16BigEndian(packet);
        if (channel < 0x4000 || channel > 0x7FFF)
            return false;

        ushort length = BinaryPrimitives.ReadUInt16BigEndian(packet[2..]);
        if (packet.Length < 4 + length)
            return false;

        channelNumber = channel;
        data = packet.Slice(4, length).ToArray();
        return true;
    }

    /// <summary>
    /// Encodes a ChannelData packet for UDP transport.
    /// </summary>
    public static byte[] Encode(ushort channelNumber, ReadOnlySpan<byte> data)
    {
        if (channelNumber < 0x4000 || channelNumber > 0x7FFF)
            throw new ArgumentOutOfRangeException(nameof(channelNumber), "TURN channel number must be in range 0x4000..0x7FFF.");

        if (data.Length > ushort.MaxValue)
            throw new ArgumentOutOfRangeException(nameof(data), "TURN ChannelData payload exceeds 65535 bytes.");

        var packet = new byte[4 + data.Length];
        BinaryPrimitives.WriteUInt16BigEndian(packet, channelNumber);
        BinaryPrimitives.WriteUInt16BigEndian(packet.AsSpan(2), (ushort)data.Length);
        data.CopyTo(packet.AsSpan(4));
        return packet;
    }
}
