using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Playback sink that injects frames into one call.
/// </summary>
internal sealed class CallPlaybackFrameSink : IPlaybackFrameSink
{
    private readonly IMediaSender _sender;
    private readonly ICall _call;
    private readonly ILogger _logger;
    private bool _disposed;

    /// <summary>
    /// Creates a call playback sink.
    /// </summary>
    public CallPlaybackFrameSink(MediaManager mediaManager, ICall call, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(mediaManager);
        _call = call ?? throw new ArgumentNullException(nameof(call));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _sender = mediaManager.CreateSender();
        _sender.AttachToCall(call);
    }

    /// <inheritdoc />
    public string TargetToken => $"call-{_call.CallId}";

    /// <inheritdoc />
    public async ValueTask SendAsync(MediaFrame frame, CancellationToken ct = default)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(CallPlaybackFrameSink));

        try
        {
            await _sender.SendAsync(frame, ct).ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Sending playback frame to call {CallId} failed.", _call.CallId);
            throw;
        }
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (_disposed)
            return ValueTask.CompletedTask;

        _disposed = true;
        _sender.Detach();
        _sender.Dispose();
        return ValueTask.CompletedTask;
    }
}
