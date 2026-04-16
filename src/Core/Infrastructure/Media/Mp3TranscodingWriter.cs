using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

internal sealed class Mp3TranscodingWriter : IAudioFileWriter
{
    private readonly string _outputPath;
    private readonly string _tempWavPath;
    private readonly int _sampleRate;
    private readonly IAudioFileWriter _wavWriter;
    private bool _disposed;

    public Mp3TranscodingWriter(string filePath, AudioFileCodecContext context, WavAudioFileCodec wavCodec)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path is required.", nameof(filePath));
        ArgumentNullException.ThrowIfNull(wavCodec);

        _outputPath = filePath;
        _sampleRate = context.SampleRate > 0 ? context.SampleRate : 8000;

        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory))
            Directory.CreateDirectory(directory);

        _tempWavPath = Path.Combine(
            Path.GetTempPath(),
            $"voipsdk-mp3-encode-{Guid.NewGuid():N}.wav");

        var wavContext = new AudioFileCodecContext(
            PayloadType: context.PayloadType,
            ClockRate: context.ClockRate,
            SampleRate: _sampleRate,
            SamplesPerFrame: Math.Max(1, context.SamplesPerFrame),
            CodecName: "L16");

        _wavWriter = wavCodec
            .CreateWriterAsync(_tempWavPath, wavContext, CancellationToken.None)
            .AsTask()
            .GetAwaiter()
            .GetResult();
    }

    public long BytesWritten => _wavWriter.BytesWritten;

    public async ValueTask WriteFrameAsync(MediaFrame frame, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(Mp3TranscodingWriter));

        if ((frame.Payload.Length & 1) != 0)
            throw new InvalidOperationException("MP3 transcode writer expects PCM16 payload with even byte length.");

        await _wavWriter.WriteFrameAsync(frame, ct).ConfigureAwait(false);
    }

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;

        Exception? error = null;
        try
        {
            await _wavWriter.DisposeAsync().ConfigureAwait(false);
            await FfmpegProcessRunner.RunAsync(psi =>
            {
                psi.ArgumentList.Add("-y");
                psi.ArgumentList.Add("-hide_banner");
                psi.ArgumentList.Add("-loglevel");
                psi.ArgumentList.Add("error");
                psi.ArgumentList.Add("-i");
                psi.ArgumentList.Add(_tempWavPath);
                psi.ArgumentList.Add("-vn");
                psi.ArgumentList.Add("-ac");
                psi.ArgumentList.Add("1");
                psi.ArgumentList.Add("-ar");
                psi.ArgumentList.Add(_sampleRate.ToString());
                psi.ArgumentList.Add("-codec:a");
                psi.ArgumentList.Add("libmp3lame");
                psi.ArgumentList.Add("-q:a");
                psi.ArgumentList.Add("4");
                psi.ArgumentList.Add(_outputPath);
            }).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            error = ex;
        }
        finally
        {
            Mp3AudioFileCodec.TryDeleteFile(_tempWavPath);
        }

        if (error is not null)
            throw new InvalidOperationException("Encoding MP3 output failed.", error);
    }
}
