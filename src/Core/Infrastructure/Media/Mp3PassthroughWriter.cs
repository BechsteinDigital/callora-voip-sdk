using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

internal sealed class Mp3PassthroughWriter : IAudioFileWriter
{
    private readonly FileStream _stream;
    private bool _validatedFirstFrame;
    private long _bytesWritten;
    private bool _disposed;

    public Mp3PassthroughWriter(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required.", nameof(filePath));

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
    }

    public long BytesWritten => _bytesWritten;

    public async ValueTask WriteFrameAsync(MediaFrame frame, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Mp3PassthroughWriter));

        if (frame.Payload.IsEmpty)
            return;

        if (!_validatedFirstFrame)
        {
            if (!Mp3FrameParser.TryReadHeader(frame.Payload.Span, out _))
            {
                throw new InvalidOperationException(
                    "MP3 passthrough expects MPEG frame payload with a valid frame header.");
            }

            _validatedFirstFrame = true;
        }

        await _stream.WriteAsync(frame.Payload, ct).ConfigureAwait(false);
        _bytesWritten += frame.Payload.Length;
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        await _stream.FlushAsync().ConfigureAwait(false);
        await _stream.DisposeAsync().ConfigureAwait(false);
    }
}
