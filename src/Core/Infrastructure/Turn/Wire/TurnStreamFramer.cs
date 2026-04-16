using System.Buffers.Binary;
using CalloraVoipSdk.Core.Infrastructure.Stun.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

/// <summary>
/// Reads TURN-over-TCP/TLS frames, supporting both STUN messages and ChannelData packets.
/// </summary>
internal static class TurnStreamFramer
{
    /// <summary>
    /// Reads one full frame from the stream.
    /// Returns null on clean EOF.
    /// </summary>
    public static async Task<TurnStreamFrame?> ReadFrameAsync(Stream stream, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var header = new byte[4];
        int headerRead = await ReadExactAsync(stream, header, ct).ConfigureAwait(false);
        if (headerRead == 0)
            return null;
        if (headerRead < 4)
            throw new InvalidDataException($"TURN stream closed after {headerRead} header bytes; expected 4.");

        ushort first = BinaryPrimitives.ReadUInt16BigEndian(header);

        if (first >= 0x4000 && first <= 0x7FFF)
        {
            ushort channelLength = BinaryPrimitives.ReadUInt16BigEndian(header.AsSpan(2));
            var payload = new byte[channelLength];
            int payloadRead = await ReadExactAsync(stream, payload, ct).ConfigureAwait(false);
            if (payloadRead < channelLength)
                throw new InvalidDataException($"TURN stream closed after {payloadRead} channel bytes; expected {channelLength}.");

            return new TurnStreamFrame
            {
                IsChannelData = true,
                ChannelNumber = first,
                Payload = payload
            };
        }

        ushort stunLength = BinaryPrimitives.ReadUInt16BigEndian(header.AsSpan(2));
        var packet = new byte[StunWireConstants.HeaderSize + stunLength];
        header.CopyTo(packet, 0);

        int remainder = packet.Length - 4;
        int remainderRead = await ReadExactAsync(stream, packet.AsMemory(4), ct).ConfigureAwait(false);
        if (remainderRead < remainder)
            throw new InvalidDataException($"TURN stream closed after {remainderRead} STUN bytes; expected {remainder}.");

        return new TurnStreamFrame
        {
            IsChannelData = false,
            Payload = packet
        };
    }

    private static async Task<int> ReadExactAsync(Stream stream, Memory<byte> buffer, CancellationToken ct)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer[totalRead..], ct).ConfigureAwait(false);
            if (read == 0)
                break;
            totalRead += read;
        }

        return totalRead;
    }
}
