namespace CalloraVoipSdk.Core.Infrastructure.Stun.Wire;

/// <summary>
/// Wire-level constants for STUN protocol framing (RFC 5389 §6).
/// </summary>
internal static class StunWireConstants
{
    /// <summary>Fixed magic cookie value placed in bytes 4–7 of every STUN message header.</summary>
    public const uint MagicCookie = 0x2112A442;

    /// <summary>Fixed STUN header size in bytes (type + length + magic cookie + transaction ID).</summary>
    public const int HeaderSize = 20;

    /// <summary>Transaction ID byte length (bytes 8–19 of the header, after magic cookie).</summary>
    public const int TransactionIdLength = 12;

    /// <summary>Per-attribute overhead: 2-byte type field + 2-byte length field.</summary>
    public const int AttributeHeaderSize = 4;

    /// <summary>
    /// XOR mask applied to FINGERPRINT CRC32 to distinguish it from MESSAGE-INTEGRITY.
    /// Spells "STUN" in ASCII (RFC 5389 §15.5).
    /// </summary>
    public const uint FingerprintXorMask = 0x5354554E;

    /// <summary>Upper 16 bits of the magic cookie, used to XOR ports in XOR-MAPPED-ADDRESS.</summary>
    public const ushort MagicCookieHighWord = 0x2112;
}
