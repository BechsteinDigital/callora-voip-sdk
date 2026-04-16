using System.Buffers.Binary;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

/// <summary>
/// WAV PCM16 mono codec for recording and playback.
/// </summary>
internal sealed class WavAudioFileCodec : IAudioFileCodec
{
    internal const int WaveHeaderSize = 44;
    internal const short PcmFormatTag = 1;
    internal const short ChannelsMono = 1;
    internal const short BitsPerSample16 = 16;

    /// <inheritdoc />
    public ValueTask<IAudioFileWriter> CreateWriterAsync(
        string filePath,
        AudioFileCodecContext context,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return ValueTask.FromResult<IAudioFileWriter>(new WavAudioFileWriter(filePath, context));
    }

    /// <inheritdoc />
    public ValueTask<IAudioFileReader> CreateReaderAsync(
        string filePath,
        AudioFileCodecContext context,
        CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        return ValueTask.FromResult<IAudioFileReader>(new WavAudioFileReader(filePath, context));
    }

    internal static void WriteWaveHeader(
        Span<byte> buffer,
        int sampleRate,
        uint dataLength)
    {
        if (buffer.Length < WaveHeaderSize)
            throw new ArgumentException("WAV header buffer is too small.", nameof(buffer));

        var byteRate = sampleRate * ChannelsMono * (BitsPerSample16 / 8);
        var blockAlign = (short)(ChannelsMono * (BitsPerSample16 / 8));

        buffer.Clear();
        buffer[0] = (byte)'R';
        buffer[1] = (byte)'I';
        buffer[2] = (byte)'F';
        buffer[3] = (byte)'F';

        BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(4, 4), dataLength + 36);

        buffer[8] = (byte)'W';
        buffer[9] = (byte)'A';
        buffer[10] = (byte)'V';
        buffer[11] = (byte)'E';

        buffer[12] = (byte)'f';
        buffer[13] = (byte)'m';
        buffer[14] = (byte)'t';
        buffer[15] = (byte)' ';
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(16, 4), 16);
        BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(20, 2), PcmFormatTag);
        BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(22, 2), ChannelsMono);
        BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(24, 4), sampleRate);
        BinaryPrimitives.WriteInt32LittleEndian(buffer.Slice(28, 4), byteRate);
        BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(32, 2), blockAlign);
        BinaryPrimitives.WriteInt16LittleEndian(buffer.Slice(34, 2), BitsPerSample16);

        buffer[36] = (byte)'d';
        buffer[37] = (byte)'a';
        buffer[38] = (byte)'t';
        buffer[39] = (byte)'a';
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.Slice(40, 4), dataLength);
    }
}
