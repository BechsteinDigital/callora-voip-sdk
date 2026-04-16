using Microsoft.Extensions.Logging;
using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media.Sessions;

/// <summary>
/// Recording frame source that mirrors inbound audio from one call.
/// </summary>
internal sealed class CallRecordingFrameSource : IRecordingFrameSource
{
    private readonly IMediaReceiver _receiver;
    private readonly ICall _call;
    private readonly ILogger _logger;
    private bool _started;
    private bool _disposed;

    /// <summary>
    /// Creates a call-bound recording frame source.
    /// </summary>
    public CallRecordingFrameSource(MediaManager mediaManager, ICall call, ILogger logger)
    {
        ArgumentNullException.ThrowIfNull(mediaManager);
        _call = call ?? throw new ArgumentNullException(nameof(call));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _receiver = mediaManager.CreateReceiver();
        _receiver.FrameReceived += OnReceiverFrameReceived;
    }

    /// <inheritdoc />
    public string SourceToken => $"call-{_call.CallId}";

    /// <inheritdoc />
    public event Action<MediaFrame>? FrameReceived;

    /// <inheritdoc />
    public ValueTask StartAsync(CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        if (_disposed)
            throw new ObjectDisposedException(nameof(CallRecordingFrameSource));

        if (_started)
            return ValueTask.CompletedTask;

        _receiver.AttachToCall(_call);
        _started = true;
        return ValueTask.CompletedTask;
    }

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (_disposed)
            return ValueTask.CompletedTask;

        _disposed = true;
        _receiver.FrameReceived -= OnReceiverFrameReceived;
        _receiver.Detach();
        _receiver.Dispose();
        return ValueTask.CompletedTask;
    }

    private void OnReceiverFrameReceived(object? sender, MediaFrameReceivedEventArgs args)
    {
        try
        {
            FrameReceived?.Invoke(args.Frame);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Recording frame callback failed for call {CallId}.", _call.CallId);
        }
    }
}
