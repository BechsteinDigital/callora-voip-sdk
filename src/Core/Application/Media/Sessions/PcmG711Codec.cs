namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Minimal G.711 A-law/µ-law codec helpers for WAV transcoding.
/// </summary>
internal static class PcmG711Codec
{
    private static readonly int[] MuLawExponentLookup =
    [
        0,0,1,1,2,2,2,2,3,3,3,3,3,3,3,3,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,4,
        5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,5,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,6,
        7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
        7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
        7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
        7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,7,
    ];

    /// <summary>
    /// Decodes µ-law RTP payload into PCM16 little-endian bytes.
    /// </summary>
    public static byte[] DecodeMuLaw(ReadOnlySpan<byte> payload)
    {
        var pcm = new byte[payload.Length * 2];
        for (var i = 0; i < payload.Length; i++)
        {
            var sample = MuLawDecode(payload[i]);
            pcm[i * 2] = (byte)(sample & 0xFF);
            pcm[(i * 2) + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return pcm;
    }

    /// <summary>
    /// Decodes A-law RTP payload into PCM16 little-endian bytes.
    /// </summary>
    public static byte[] DecodeALaw(ReadOnlySpan<byte> payload)
    {
        var pcm = new byte[payload.Length * 2];
        for (var i = 0; i < payload.Length; i++)
        {
            var sample = ALawDecode(payload[i]);
            pcm[i * 2] = (byte)(sample & 0xFF);
            pcm[(i * 2) + 1] = (byte)((sample >> 8) & 0xFF);
        }

        return pcm;
    }

    /// <summary>
    /// Encodes PCM16 little-endian samples into µ-law payload.
    /// </summary>
    public static byte[] EncodeMuLaw(ReadOnlySpan<byte> pcm16)
    {
        var count = pcm16.Length / 2;
        var encoded = new byte[count];
        for (var i = 0; i < count; i++)
        {
            var sample = ReadSample(pcm16, i);
            encoded[i] = MuLawEncode(sample);
        }

        return encoded;
    }

    /// <summary>
    /// Encodes PCM16 little-endian samples into A-law payload.
    /// </summary>
    public static byte[] EncodeALaw(ReadOnlySpan<byte> pcm16)
    {
        var count = pcm16.Length / 2;
        var encoded = new byte[count];
        for (var i = 0; i < count; i++)
        {
            var sample = ReadSample(pcm16, i);
            encoded[i] = ALawEncode(sample);
        }

        return encoded;
    }

    private static short ReadSample(ReadOnlySpan<byte> pcm16, int index)
    {
        var offset = index * 2;
        return (short)(pcm16[offset] | (pcm16[offset + 1] << 8));
    }

    private static byte MuLawEncode(short pcm)
    {
        const int bias = 0x84;
        const int clip = 32635;

        var sign = (pcm >> 8) & 0x80;
        if (sign != 0)
            pcm = (short)-pcm;

        if (pcm > clip)
            pcm = clip;

        pcm += bias;
        var exponent = MuLawExponentLookup[(pcm >> 7) & 0xFF];
        var mantissa = (pcm >> (exponent + 3)) & 0x0F;
        return (byte)~(sign | (exponent << 4) | mantissa);
    }

    private static short MuLawDecode(byte muLaw)
    {
        var transformed = (byte)~muLaw;
        var sign = transformed & 0x80;
        var exponent = (transformed >> 4) & 0x07;
        var mantissa = transformed & 0x0F;
        var value = ((mantissa << 3) + 0x84) << exponent;
        return (short)(sign != 0 ? -(value - 0x84) : value - 0x84);
    }

    private static byte ALawEncode(short pcm)
    {
        var sign = (pcm & 0x8000) != 0 ? 0 : 0x80;
        if (sign == 0)
            pcm = (short)-pcm;

        if (pcm > 32767)
            pcm = 32767;

        var exponent = 7;
        for (var mask = 0x4000; (pcm & mask) == 0 && exponent > 0; exponent--, mask >>= 1)
        {
        }

        var mantissa = exponent == 0
            ? pcm >> 1
            : (pcm >> (exponent + 3)) & 0x0F;

        return (byte)((sign | (exponent << 4) | mantissa) ^ 0x55);
    }

    private static short ALawDecode(byte aLaw)
    {
        var transformed = (byte)(aLaw ^ 0x55);
        var sign = transformed & 0x80;
        var exponent = (transformed >> 4) & 0x07;
        var mantissa = transformed & 0x0F;

        var value = exponent == 0
            ? (mantissa << 1) | 1
            : (((mantissa | 0x10) << 1) | 1) << (exponent - 1);

        value <<= 3;
        return (short)(sign != 0 ? value : -value);
    }
}
