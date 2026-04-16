using System.Buffers.Binary;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

internal static class Mp3FrameParser
{
    private static readonly int[] SampleRatesMpeg1 = [44100, 48000, 32000];
    private static readonly int[] SampleRatesMpeg2 = [22050, 24000, 16000];
    private static readonly int[] SampleRatesMpeg25 = [11025, 12000, 8000];

    private static readonly int[] BitratesMpeg1Layer3 =
        [0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0];

    private static readonly int[] BitratesMpeg2Layer3 =
        [0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0];

    public static bool TryReadHeader(ReadOnlySpan<byte> payload, out Mp3FrameHeader header)
    {
        header = default;
        if (payload.Length < 4)
            return false;

        var raw = BinaryPrimitives.ReadUInt32BigEndian(payload[..4]);
        var sync = (raw >> 21) & 0x7FF;
        if (sync != 0x7FF)
            return false;

        var versionBits = (int)((raw >> 19) & 0x3);
        var layerBits = (int)((raw >> 17) & 0x3);
        var bitrateIndex = (int)((raw >> 12) & 0xF);
        var sampleRateIndex = (int)((raw >> 10) & 0x3);
        var padding = (int)((raw >> 9) & 0x1);

        if (layerBits != 0x1)
            return false;

        if (sampleRateIndex == 0x3)
            return false;

        if (bitrateIndex == 0 || bitrateIndex == 0xF)
            return false;

        var sampleRate = ResolveSampleRate(versionBits, sampleRateIndex);
        if (sampleRate <= 0)
            return false;

        var bitrateKbps = ResolveBitrateKbps(versionBits, bitrateIndex);
        if (bitrateKbps <= 0)
            return false;

        var isMpeg1 = versionBits == 0x3;
        var samplesPerFrame = isMpeg1 ? 1152 : 576;
        var frameLength = isMpeg1
            ? ((144 * bitrateKbps * 1000) / sampleRate) + padding
            : ((72 * bitrateKbps * 1000) / sampleRate) + padding;

        if (frameLength <= 4)
            return false;

        header = new Mp3FrameHeader(frameLength, sampleRate, samplesPerFrame);
        return true;
    }

    private static int ResolveSampleRate(int versionBits, int sampleRateIndex)
    {
        return versionBits switch
        {
            0x3 => SampleRatesMpeg1[sampleRateIndex],
            0x2 => SampleRatesMpeg2[sampleRateIndex],
            0x0 => SampleRatesMpeg25[sampleRateIndex],
            _ => 0,
        };
    }

    private static int ResolveBitrateKbps(int versionBits, int bitrateIndex)
    {
        return versionBits == 0x3
            ? BitratesMpeg1Layer3[bitrateIndex]
            : BitratesMpeg2Layer3[bitrateIndex];
    }
}
