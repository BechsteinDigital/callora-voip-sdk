using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Application.Ports.Media;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Default playback session implementation used by <see cref="MediaManager"/>.
/// </summary>
internal sealed class PlaybackSession : IPlaybackSession
{
    private readonly object _sync = new();
    private readonly IPlaybackFrameSink _sink;
    private readonly IAudioFileCodec _codec;
    private readonly AudioPayloadTranscodingPlan _transcodingPlan;
    private readonly PlaybackRequest _request;
    private readonly PlaybackOptions _options;
    private readonly ILogger _logger;
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _playbackLoop;

    private MediaSessionState _state;
    private bool _disposed;

    /// <summary>
    /// Creates and starts one playback session.
    /// </summary>
    public PlaybackSession(
        IPlaybackFrameSink sink,
        IAudioFileCodec codec,
        AudioPayloadTranscodingPlan transcodingPlan,
        PlaybackRequest request,
        AudioFileFormat format,
        ILogger logger)
    {
        _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        _codec = codec ?? throw new ArgumentNullException(nameof(codec));
        _transcodingPlan = transcodingPlan ?? throw new ArgumentNullException(nameof(transcodingPlan));
        _request = request ?? throw new ArgumentNullException(nameof(request));
        _options = _request.Options ?? new PlaybackOptions();
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        SessionId = Guid.NewGuid();
        SourceFilePath = _request.FilePath;
        Format = format;
        _state = _options.StartPaused ? MediaSessionState.Paused : MediaSessionState.Running;

        _playbackLoop = Task.Run(PlaybackLoopAsync);
    }

    /// <inheritdoc />
    public Guid SessionId { get; }

    /// <inheritdoc />
    public string SourceFilePath { get; }

    /// <inheritdoc />
    public AudioFileFormat Format { get; }

    /// <inheritdoc />
    public MediaSessionState State
    {
        get
        {
            lock (_sync)
                return _state;
        }
    }

    /// <inheritdoc />
    public event EventHandler<MediaSessionStateChangedEventArgs>? StateChanged;

    /// <inheritdoc />
    public event EventHandler<MediaSessionErrorEventArgs>? Error;

    /// <inheritdoc />
    public Task PauseAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        TransitionTo(MediaSessionState.Paused, "Playback paused.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public Task ResumeAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        EnsureNotDisposed();
        TransitionTo(MediaSessionState.Running, "Playback resumed.");
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task StopAsync(CancellationToken ct = default)
    {
        EnsureNotDisposed();

        if (State == MediaSessionState.Stopped)
            return;

        _cts.Cancel();
        try
        {
            await _playbackLoop.WaitAsync(ct).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
        {
            // normal stop
        }
        finally
        {
            await _sink.DisposeAsync().ConfigureAwait(false);
            if (State != MediaSessionState.Faulted)
                TransitionTo(MediaSessionState.Stopped, "Playback stopped.");
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (_disposed)
            return;

        try
        {
            await StopAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "Dispose observed an exception while stopping playback session {SessionId}.", SessionId);
        }
        finally
        {
            _disposed = true;
            _cts.Dispose();
        }
    }

    private async Task PlaybackLoopAsync()
    {
        try
        {
            do
            {
                await using var reader = await _codec
                    .CreateReaderAsync(SourceFilePath, _transcodingPlan.CodecContext, _cts.Token)
                    .ConfigureAwait(false);

                while (!_cts.IsCancellationRequested)
                {
                    await WaitWhilePausedAsync(_cts.Token).ConfigureAwait(false);

                    var frame = await reader.ReadNextFrameAsync(_cts.Token).ConfigureAwait(false);
                    if (frame is null)
                        break;

                    var outbound = _transcodingPlan.FromFileFrame(frame.Value.Frame);
                    await _sink.SendAsync(outbound, _cts.Token).ConfigureAwait(false);

                    var delay = _options.FixedFrameDelay ?? frame.Value.Delay;
                    if (delay > TimeSpan.Zero)
                        await Task.Delay(delay, _cts.Token).ConfigureAwait(false);
                }
            }
            while (_options.Loop && !_cts.IsCancellationRequested);

            if (State != MediaSessionState.Faulted)
                TransitionTo(MediaSessionState.Stopped, "Playback completed.");
        }
        catch (OperationCanceledException) when (_cts.IsCancellationRequested)
        {
            // Expected on stop.
        }
        catch (Exception ex)
        {
            TransitionTo(MediaSessionState.Faulted, "Playback failed.");
            RaiseError("playback-loop", "Playback loop failed.", ex);
            _logger.LogError(ex, "Playback session {SessionId} failed for {SourceFilePath}.", SessionId, SourceFilePath);
        }
    }

    private async Task WaitWhilePausedAsync(CancellationToken ct)
    {
        while (State == MediaSessionState.Paused)
        {
            await Task.Delay(25, ct).ConfigureAwait(false);
        }
    }

    private void TransitionTo(MediaSessionState newState, string reason)
    {
        MediaSessionState oldState;
        lock (_sync)
        {
            oldState = _state;
            if (oldState == newState)
                return;

            if (oldState == MediaSessionState.Stopped || oldState == MediaSessionState.Faulted)
                return;

            _state = newState;
        }

        StateChanged?.Invoke(this, new MediaSessionStateChangedEventArgs(oldState, newState, reason, DateTimeOffset.UtcNow));
    }

    private void RaiseError(string operation, string message, Exception exception)
    {
        Error?.Invoke(this, new MediaSessionErrorEventArgs(operation, message, exception));
    }

    private void EnsureNotDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(PlaybackSession));
    }
}
