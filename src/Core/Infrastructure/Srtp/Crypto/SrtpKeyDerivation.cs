using System.Security.Cryptography;

namespace CalloraVoipSdk.Core.Infrastructure.Srtp.Crypto;

/// <summary>
/// SRTP key derivation function (RFC 3711 §4.3).
/// Derives cipher key, salt, and auth key from master key + master salt
/// using AES-128/256 Counter Mode as a PRF (PRF-128 / PRF-256).
/// </summary>
internal static class SrtpKeyDerivation
{
    // Label constants (RFC 3711 §4.3.1)
    private const byte LabelCipherKey = 0x00;
    private const byte LabelSalt      = 0x02;
    private const byte LabelAuthKey   = 0x01;

    /// <summary>
    /// Derives all session keys for the given master key material.
    /// Key derivation rate r = 0 (default — keys derived once per session).
    /// </summary>
    public static SrtpSessionKeys Derive(SrtpKeyMaterial material)
    {
        ArgumentNullException.ThrowIfNull(material);

        var keyLength  = material.MasterKey.Length; // 16 or 32
        var authLength = 20; // HMAC-SHA1 key = 20 bytes

        return new SrtpSessionKeys
        {
            CipherKey = DeriveKey(material, LabelCipherKey, keyLength),
            Salt      = DeriveKey(material, LabelSalt,      14),
            AuthKey   = DeriveKey(material, LabelAuthKey,   authLength),
        };
    }

    // -------------------------------------------------------------------------
    // PRF (AES Counter Mode as pseudo-random function, RFC 3711 §4.3.1)
    // -------------------------------------------------------------------------

    private static byte[] DeriveKey(SrtpKeyMaterial material, byte label, int outputLength)
    {
        // x = label * 2^48 (label in bit position 48..55 of 128-bit IV)
        // IV = (master_salt XOR x) padded to 128 bits, with r=0 and index=0
        // i.e. IV[7] = salt[7] XOR label (for r=0, index=0)
        var iv = new byte[16];
        var salt = material.MasterSalt.Span;

        // Copy 14-byte salt into bytes 2..15 of 16-byte IV (left-justified in 128 bits)
        salt.CopyTo(iv.AsSpan(2));

        // XOR label into byte at position 2 + (14 - 7) = 9 (label occupies bits 48..55)
        iv[2 + 7] ^= label;

        return AesCmPrf(material.MasterKey.Span, iv, outputLength);
    }

    /// <summary>
    /// AES Counter Mode PRF — generates <paramref name="outputLength"/> bytes of keystream
    /// by encrypting successive counter blocks (IV, IV+1, IV+2, …) with AES-ECB.
    /// </summary>
    private static byte[] AesCmPrf(ReadOnlySpan<byte> key, byte[] iv, int outputLength)
    {
        var output  = new byte[outputLength];
        var written = 0;
        var counter = (ulong)0;

        // Use AES-ECB to generate keystream blocks
        using var aes = Aes.Create();
        aes.Key     = key.ToArray();
        aes.Mode    = CipherMode.ECB;
        aes.Padding = PaddingMode.None;

        var block = new byte[16];
        var counterIv = (byte[])iv.Clone();

        while (written < outputLength)
        {
            // Encrypt current counter block
            using var encryptor = aes.CreateEncryptor();
            encryptor.TransformBlock(counterIv, 0, 16, block, 0);

            var toCopy = Math.Min(16, outputLength - written);
            block.AsSpan(0, toCopy).CopyTo(output.AsSpan(written));
            written += toCopy;

            // Increment counter in the last 4 bytes (big-endian, RFC 3711 §4.1.1)
            counter++;
            counterIv[12] = (byte)(counter >> 24);
            counterIv[13] = (byte)(counter >> 16);
            counterIv[14] = (byte)(counter >>  8);
            counterIv[15] = (byte)(counter);
        }

        return output;
    }
}
