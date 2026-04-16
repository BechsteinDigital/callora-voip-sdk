using System.Buffers.Binary;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Wire;

/// <summary>
/// Helper for reading STUN messages from TCP streams (RFC 5389 §7.2.2).
/// <para>
/// STUN messages over TCP/TLS are not framed with an explicit length prefix — they are
/// self-delimiting via the 2-byte length field in the STUN header (bytes 2–3).
/// This framer reads the 20-byte STUN header first, extracts the declared attribute length,
/// then reads the remaining bytes to return a single complete STUN message buffer.
/// </para>
/// <para>
/// TURN ChannelData framing (RFC 5766 §11) uses a different, 4-byte channel-number prefix
/// and is handled separately by the TURN layer.
/// </para>
/// </summary>
internal static class StunTcpFramer
{
    /// <summary>
    /// Asynchronously reads one complete STUN message from the given stream.
    /// </summary>
    /// <param name="stream">Connected TCP or TLS stream.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// A byte array containing the complete STUN message (header + attributes),
    /// or null when the stream has reached end-of-file cleanly.
    /// </returns>
    /// <exception cref="InvalidDataException">
    /// Thrown when the stream closes mid-message or the declared length is unreasonably large.
    /// </exception>
    public static async Task<byte[]?> ReadMessageAsync(Stream stream, CancellationToken ct = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        // Read the 20-byte STUN header.
        var header = new byte[StunWireConstants.HeaderSize];
        int headerRead = await ReadExactAsync(stream, header, ct).ConfigureAwait(false);

        if (headerRead == 0)
            return null; // Clean end-of-stream.

        if (headerRead < StunWireConstants.HeaderSize)
            throw new InvalidDataException(
                $"TCP stream closed after {headerRead} bytes; expected {StunWireConstants.HeaderSize}-byte STUN header.");

        ushort attrLength = BinaryPrimitives.ReadUInt16BigEndian(header.AsSpan(2));

        // Guard against absurdly large declared lengths (e.g. corrupt packets).
        const int maxStunAttributeBytes = 65535;
        if (attrLength > maxStunAttributeBytes)
            throw new InvalidDataException(
                $"STUN declared attribute length {attrLength} exceeds safety limit of {maxStunAttributeBytes}.");

        if (attrLength == 0)
            return header; // Header-only message (e.g. basic Binding Request).

        var full = new byte[StunWireConstants.HeaderSize + attrLength];
        header.CopyTo(full, 0);

        int attrRead = await ReadExactAsync(stream, full.AsMemory(StunWireConstants.HeaderSize), ct)
            .ConfigureAwait(false);

        if (attrRead < attrLength)
            throw new InvalidDataException(
                $"TCP stream closed after {attrRead} attribute bytes; expected {attrLength}.");

        return full;
    }

    /// <summary>
    /// Reads exactly <paramref name="buffer"/>.Length bytes from the stream, looping as needed.
    /// Returns the number of bytes actually read (less than buffer length only at clean EOF).
    /// </summary>
    private static async Task<int> ReadExactAsync(
        Stream            stream,
        Memory<byte>      buffer,
        CancellationToken ct)
    {
        int totalRead = 0;
        while (totalRead < buffer.Length)
        {
            int read = await stream.ReadAsync(buffer[totalRead..], ct).ConfigureAwait(false);
            if (read == 0)
                break; // EOF.
            totalRead += read;
        }

        return totalRead;
    }
}
