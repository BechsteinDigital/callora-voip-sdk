using System.Buffers.Binary;
using System.Net;
using System.Net.Sockets;
using CalloraVoipSdk.Core.Infrastructure.Stun.Wire;
using CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

namespace CalloraVoipSdk.Core.Infrastructure.Turn.Wire;

/// <summary>
/// Encodes and decodes TURN XOR-ADDRESS values used by XOR-PEER-ADDRESS and XOR-RELAYED-ADDRESS.
/// </summary>
internal static class TurnWireAddressCodec
{
    /// <summary>
    /// Encodes a TURN XOR address value (without STUN attribute header).
    /// </summary>
    public static byte[] EncodeXorAddressValue(IPEndPoint endPoint, ReadOnlySpan<byte> transactionId)
    {
        ArgumentNullException.ThrowIfNull(endPoint);

        bool isIpv6 = endPoint.AddressFamily == AddressFamily.InterNetworkV6;
        int addressLength = isIpv6 ? 16 : 4;
        var value = new byte[4 + addressLength];

        value[0] = 0x00;
        value[1] = (byte)(isIpv6 ? TurnAddressFamily.IPv6 : TurnAddressFamily.IPv4);

        ushort port = (ushort)(endPoint.Port ^ StunWireConstants.MagicCookieHighWord);
        BinaryPrimitives.WriteUInt16BigEndian(value.AsSpan(2), port);

        var addr = endPoint.Address.GetAddressBytes();

        Span<byte> cookie = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(cookie, StunWireConstants.MagicCookie);

        for (int i = 0; i < 4; i++)
            addr[i] ^= cookie[i];

        if (isIpv6)
        {
            if (transactionId.Length < StunWireConstants.TransactionIdLength)
                throw new ArgumentException("TURN XOR IPv6 encoding requires a 12-byte transaction ID.", nameof(transactionId));

            for (int i = 0; i < 12; i++)
                addr[4 + i] ^= transactionId[i];
        }

        addr.CopyTo(value.AsSpan(4));
        return value;
    }

    /// <summary>
    /// Tries to decode a TURN XOR address value into an endpoint.
    /// </summary>
    public static bool TryDecodeXorAddressValue(
        ReadOnlySpan<byte> value,
        ReadOnlySpan<byte> transactionId,
        out IPEndPoint? endPoint)
    {
        endPoint = null;

        if (value.Length < 4)
            return false;

        var family = (TurnAddressFamily)value[1];
        bool isIpv4 = family == TurnAddressFamily.IPv4;
        bool isIpv6 = family == TurnAddressFamily.IPv6;
        if (!isIpv4 && !isIpv6)
            return false;

        int requiredLength = isIpv6 ? 20 : 8;
        if (value.Length < requiredLength)
            return false;

        ushort port = BinaryPrimitives.ReadUInt16BigEndian(value.Slice(2, 2));
        port ^= StunWireConstants.MagicCookieHighWord;

        var addr = value.Slice(4, isIpv6 ? 16 : 4).ToArray();

        Span<byte> cookie = stackalloc byte[4];
        BinaryPrimitives.WriteUInt32BigEndian(cookie, StunWireConstants.MagicCookie);
        for (int i = 0; i < 4; i++)
            addr[i] ^= cookie[i];

        if (isIpv6)
        {
            if (transactionId.Length < StunWireConstants.TransactionIdLength)
                return false;

            for (int i = 0; i < 12; i++)
                addr[4 + i] ^= transactionId[i];
        }

        endPoint = new IPEndPoint(new IPAddress(addr), port);
        return true;
    }
}
