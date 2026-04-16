using CalloraVoipSdk.Core.Infrastructure.Stun.Messages;

namespace CalloraVoipSdk.Core.Infrastructure.Stun.Wire;

/// <summary>
/// Encodes and decodes STUN messages to and from wire-format byte arrays (RFC 5389).
/// Handles MESSAGE-INTEGRITY (HMAC-SHA1) and FINGERPRINT (CRC32) computation on encoding.
/// All multi-byte integer fields are big-endian on the wire.
/// </summary>
internal interface IStunMessageCodec
{
    /// <summary>
    /// Encodes a STUN message without MESSAGE-INTEGRITY or FINGERPRINT.
    /// Suitable for basic unauthenticated Binding Requests.
    /// </summary>
    byte[] Encode(StunMessage message);

    /// <summary>
    /// Encodes a STUN message and appends a computed MESSAGE-INTEGRITY attribute.
    /// Optionally appends a FINGERPRINT attribute after MESSAGE-INTEGRITY.
    /// Any pre-existing MESSAGE-INTEGRITY or FINGERPRINT in the attribute list are ignored.
    /// </summary>
    /// <param name="message">Message to encode.</param>
    /// <param name="hmacKey">
    /// HMAC-SHA1 key bytes. For ICE short-term credentials this is the remote ICE password
    /// processed through SASLprep. For long-term credentials use MD5(user:realm:pass).
    /// </param>
    /// <param name="addFingerprint">When true, appends a FINGERPRINT after MESSAGE-INTEGRITY.</param>
    byte[] EncodeWithIntegrity(
        StunMessage message,
        ReadOnlySpan<byte> hmacKey,
        bool addFingerprint = true);

    /// <summary>
    /// Attempts to decode wire bytes into a STUN message model.
    /// Returns null when the input is not a valid STUN message
    /// (missing magic cookie, truncated header, declared length exceeds data, etc.).
    /// </summary>
    StunMessage? Decode(ReadOnlySpan<byte> data);

    /// <summary>
    /// Returns true when the buffer begins with a valid STUN magic cookie (0x2112A442 at offset 4).
    /// Used by multiplexed receivers to distinguish STUN from RTP, DTLS, and other protocols.
    /// </summary>
    bool IsStunPacket(ReadOnlySpan<byte> data);

    /// <summary>
    /// Verifies the MESSAGE-INTEGRITY attribute in a raw STUN message (RFC 5389 §15.4).
    /// Scans the raw buffer for the MESSAGE-INTEGRITY attribute, adjusts the length field
    /// as required by the spec, and computes HMAC-SHA1 over the prefix bytes.
    /// Returns false when no MESSAGE-INTEGRITY attribute is present or the HMAC does not match.
    /// </summary>
    /// <param name="rawMessage">The complete raw STUN message bytes as received.</param>
    /// <param name="hmacKey">
    /// The HMAC-SHA1 key. Derive with <see cref="Auth.StunKeyDerivation.ShortTermKey"/>
    /// or <see cref="Auth.StunKeyDerivation.LongTermKey"/> before calling.
    /// </param>
    bool VerifyIntegrity(ReadOnlySpan<byte> rawMessage, ReadOnlySpan<byte> hmacKey);

    /// <summary>
    /// Array overload of <see cref="VerifyIntegrity(ReadOnlySpan{byte}, ReadOnlySpan{byte})"/>.
    /// </summary>
    /// <param name="rawMessage">The complete raw STUN message bytes as received.</param>
    /// <param name="hmacKey">The HMAC-SHA1 key bytes.</param>
    bool VerifyIntegrity(byte[] rawMessage, byte[] hmacKey);

    /// <summary>
    /// Verifies the FINGERPRINT attribute in a raw STUN message (RFC 5389 §15.5).
    /// Scans the raw buffer for the FINGERPRINT attribute, adjusts the length field,
    /// computes CRC32 XOR 0x5354554E over the prefix bytes, and compares with the stored value.
    /// Returns false when no FINGERPRINT attribute is present or the CRC does not match.
    /// </summary>
    /// <param name="rawMessage">The complete raw STUN message bytes as received.</param>
    bool VerifyFingerprint(ReadOnlySpan<byte> rawMessage);

    /// <summary>
    /// Array overload of <see cref="VerifyFingerprint(ReadOnlySpan{byte})"/>.
    /// </summary>
    /// <param name="rawMessage">The complete raw STUN message bytes as received.</param>
    bool VerifyFingerprint(byte[] rawMessage);
}
