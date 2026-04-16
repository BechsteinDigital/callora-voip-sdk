using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Infrastructure.Media;

internal sealed class Mp3TranscodingReader : IAudioFileReader
{
    private readonly string _tempWavPath;
    private readonly IAudioFileReader _wavReader;
    private bool _disposed;

    private Mp3TranscodingReader(string tempWavPath, IAudioFileReader wavReader)
    {
        _tempWavPath = tempWavPath;
        _wavReader = wavReader;
    }

    public static async Task<Mp3TranscodingReader> CreateAsync(
        string sourceMp3Path,
        AudioFileCodecContext context,
        WavAudioFileCodec wavCodec,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sourceMp3Path))
            throw new ArgumentException("File path is required.", nameof(sourceMp3Path));

        ArgumentNullException.ThrowIfNull(wavCodec);

        var tempWavPath = Path.Combine(Path.GetTempPath(), $"voipsdk-mp3-decode-{Guid.NewGuid():N}.wav");
        await FfmpegProcessRunner.RunAsync(psi =>
        {
            psi.ArgumentList.Add("-y");
            psi.ArgumentList.Add("-hide_banner");
            psi.ArgumentList.Add("-loglevel");
            psi.ArgumentList.Add("error");
            psi.ArgumentList.Add("-i");
            psi.ArgumentList.Add(sourceMp3Path);
            psi.ArgumentList.Add("-vn");
            psi.ArgumentList.Add("-ac");
            psi.ArgumentList.Add("1");
            psi.ArgumentList.Add("-f");
            psi.ArgumentList.Add("wav");
            psi.ArgumentList.Add(tempWavPath);
        }, ct).ConfigureAwait(false);

        try
        {
            var wavContext = new AudioFileCodecContext(
                PayloadType: context.PayloadType,
                ClockRate: context.ClockRate,
                SampleRate: context.SampleRate,
                SamplesPerFrame: Math.Max(1, context.SamplesPerFrame),
                CodecName: "L16");

            var reader = await wavCodec
                .CreateReaderAsync(tempWavPath, wavContext, ct)
                .ConfigureAwait(false);

            return new Mp3TranscodingReader(tempWavPath, reader);
        }
        catch
        {
            Mp3AudioFileCodec.TryDeleteFile(tempWavPath);
            throw;
        }
    }

    public ValueTask<AudioFileFrame?> ReadNextFrameAsync(CancellationToken ct = default)
        => _wavReader.ReadNextFrameAsync(ct);

    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        _disposed = true;
        await _wavReader.DisposeAsync().ConfigureAwait(false);
        Mp3AudioFileCodec.TryDeleteFile(_tempWavPath);
    }
}
