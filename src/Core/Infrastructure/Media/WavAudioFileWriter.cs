using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

internal sealed class WavAudioFileWriter : IAudioFileWriter
{
    private readonly FileStream _stream;
    private readonly int _sampleRate;
    private uint _dataLength;
    private bool _disposed;

    public WavAudioFileWriter(string filePath, AudioFileCodecContext context)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required.", nameof(filePath));

        _sampleRate = context.SampleRate > 0 ? context.SampleRate : 8000;
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        _stream = new FileStream(
            filePath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.Asynchronous | FileOptions.SequentialScan);

        Span<byte> header = stackalloc byte[WavAudioFileCodec.WaveHeaderSize];
        WavAudioFileCodec.WriteWaveHeader(header, _sampleRate, 0);
        _stream.Write(header);
    }

    public long BytesWritten => _dataLength;

    public async ValueTask WriteFrameAsync(MediaFrame frame, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(WavAudioFileWriter));

        if ((frame.Payload.Length & 1) != 0)
            throw new InvalidOperationException("WAV PCM16 frames must have even payload size.");

        var payload = frame.Payload;
        if (payload.Length == 0)
            return;

        await _stream.WriteAsync(payload, ct).ConfigureAwait(false);
        _dataLength += (uint)payload.Length;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        var position = _stream.Position;
        _stream.Seek(0, SeekOrigin.Begin);
        var header = new byte[WavAudioFileCodec.WaveHeaderSize];
        WavAudioFileCodec.WriteWaveHeader(header, _sampleRate, _dataLength);
        await _stream.WriteAsync(header, CancellationToken.None).ConfigureAwait(false);
        _stream.Seek(position, SeekOrigin.Begin);
        await _stream.FlushAsync().ConfigureAwait(false);
        await _stream.DisposeAsync().ConfigureAwait(false);
    }
}
