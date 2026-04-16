using NAudio.Codecs;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Minimal G.722 RTP payload codec helpers for WAV/MP3 transcoding paths.
/// </summary>
internal static class PcmG722Codec
{
    /// <summary>
    /// Decodes G.722 payload bytes into PCM16 little-endian bytes.
    /// </summary>
    public static byte[] Decode(ReadOnlySpan<byte> payload)
    {
        if (payload.Length == 0)
            return Array.Empty<byte>();

        var state = new G722CodecState(64000, G722Flags.None);
        var input = payload.ToArray();
        var samples = new short[input.Length * 2];
        new G722Codec().Decode(state, samples, input, input.Length);

        var pcm = new byte[samples.Length * 2];
        Buffer.BlockCopy(samples, 0, pcm, 0, pcm.Length);
        return pcm;
    }

    /// <summary>
    /// Encodes PCM16 little-endian bytes into G.722 payload bytes.
    /// </summary>
    public static byte[] Encode(ReadOnlySpan<byte> pcm16)
    {
        if (pcm16.Length == 0)
            return Array.Empty<byte>();
        if ((pcm16.Length & 1) != 0)
            throw new InvalidOperationException("G.722 encoder expects PCM16 payload with even byte count.");

        var state = new G722CodecState(64000, G722Flags.None);
        var sampleCount = pcm16.Length / 2;
        var samples = new short[sampleCount];
        Buffer.BlockCopy(pcm16.ToArray(), 0, samples, 0, pcm16.Length);

        var encoded = new byte[Math.Max(1, sampleCount / 2)];
        new G722Codec().Encode(state, encoded, samples, sampleCount);
        return encoded;
    }
}
