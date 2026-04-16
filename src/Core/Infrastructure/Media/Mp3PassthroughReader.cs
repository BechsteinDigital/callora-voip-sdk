using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

internal sealed class Mp3PassthroughReader : IAudioFileReader
{
    private readonly FileStream _stream;
    private readonly int _payloadType;
    private readonly int _clockRate;
    private bool _disposed;

    public Mp3PassthroughReader(string filePath, AudioFileCodecContext context)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required.", nameof(filePath));

        _payloadType = context.PayloadType;
        _clockRate = context.ClockRate > 0 ? context.ClockRate : 90000;

        _stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);
    }

    public async ValueTask<AudioFileFrame?> ReadNextFrameAsync(CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Mp3PassthroughReader));

        var headerBytes = new byte[4];
        var headerRead = await ReadExactAsync(headerBytes, ct).ConfigureAwait(false);
        if (headerRead == 0)
            return null;

        if (headerRead < headerBytes.Length)
            throw new InvalidOperationException("Unexpected end-of-file while reading MP3 frame header.");

        if (!Mp3FrameParser.TryReadHeader(headerBytes, out var header))
            throw new InvalidOperationException("Invalid MP3 frame header.");

        var frameLength = header.FrameLengthBytes;
        if (frameLength < 4)
            throw new InvalidOperationException("Invalid MP3 frame length.");

        var payload = new byte[frameLength];
        Buffer.BlockCopy(headerBytes, 0, payload, 0, 4);

        var remaining = frameLength - 4;
        if (remaining > 0)
        {
            var bodyRead = await ReadExactAsync(payload.AsMemory(4, remaining), ct).ConfigureAwait(false);
            if (bodyRead < remaining)
                throw new InvalidOperationException("Unexpected end-of-file while reading MP3 frame payload.");
        }

        var delay = TimeSpan.FromSeconds(header.SamplesPerFrame / (double)header.SampleRateHz);
        var durationRtpUnits = (uint)Math.Max(
            1,
            (int)Math.Round(header.SamplesPerFrame * (_clockRate / (double)header.SampleRateHz)));

        return new AudioFileFrame(new MediaFrame(payload, _payloadType, durationRtpUnits), delay);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        await _stream.DisposeAsync().ConfigureAwait(false);
    }

    private async ValueTask<int> ReadExactAsync(Memory<byte> buffer, CancellationToken ct)
    {
        var total = 0;
        while (total < buffer.Length)
        {
            var read = await _stream.ReadAsync(buffer[total..], ct).ConfigureAwait(false);
            if (read == 0)
                break;

            total += read;
        }

        return total;
    }

    private async ValueTask<int> ReadExactAsync(byte[] buffer, CancellationToken ct)
    {
        var total = 0;
        while (total < buffer.Length)
        {
            var read = await _stream.ReadAsync(buffer.AsMemory(total, buffer.Length - total), ct).ConfigureAwait(false);
            if (read == 0)
                break;

            total += read;
        }

        return total;
    }
}
