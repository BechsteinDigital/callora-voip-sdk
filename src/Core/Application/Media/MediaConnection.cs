using System.Threading.Channels;

namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// One-way media forwarding link between a receiver and sender.
/// </summary>
internal sealed class MediaConnection : IDisposable
{
    private readonly IMediaReceiver _receiver;
    private readonly IMediaSender _sender;
    private readonly Channel<MediaFrame> _queue;
    private readonly CancellationTokenSource _shutdownCts = new();
    private readonly Task _pumpTask;
    private int _disposed;

    /// <summary>
    /// Creates a buffered media forwarding link.
    /// </summary>
    internal MediaConnection(
        IMediaReceiver receiver,
        IMediaSender sender,
        int queueCapacity)
    {
        _receiver = receiver ?? throw new ArgumentNullException(nameof(receiver));
        _sender = sender ?? throw new ArgumentNullException(nameof(sender));

        _queue = Channel.CreateBounded<MediaFrame>(new BoundedChannelOptions(Math.Max(1, queueCapacity))
        {
            SingleReader = true,
            SingleWriter = false,
            FullMode = BoundedChannelFullMode.DropOldest,
            AllowSynchronousContinuations = false
        });

        _receiver.FrameReceived += OnFrameReceived;
        _pumpTask = Task.Run(PumpAsync);
    }

    /// <summary>
    /// Buffers one inbound frame for asynchronous forwarding.
    /// </summary>
    private void OnFrameReceived(object? _, MediaFrameReceivedEventArgs e)
    {
        if (Volatile.Read(ref _disposed) != 0) return;
        _queue.Writer.TryWrite(e.Frame);
    }

    /// <summary>
    /// Pumps buffered frames to the sender until shutdown.
    /// </summary>
    private async Task PumpAsync()
    {
        try
        {
            while (await _queue.Reader.WaitToReadAsync(_shutdownCts.Token).ConfigureAwait(false))
            {
                while (_queue.Reader.TryRead(out var frame))
                    await ForwardAsync(frame).ConfigureAwait(false);
            }
        }
        catch (OperationCanceledException) when (_shutdownCts.IsCancellationRequested) { }
        catch (ChannelClosedException) { }
    }

    /// <summary>
    /// Forwards one frame to the destination sender.
    /// </summary>
    private async Task ForwardAsync(MediaFrame frame)
    {
        try
        {
            await _sender.SendAsync(frame, _shutdownCts.Token).ConfigureAwait(false);
        }
        catch (OperationCanceledException) when (_shutdownCts.IsCancellationRequested) { }
        catch (ObjectDisposedException)
        {
            // Link is shutting down; ignore.
        }
        catch (InvalidOperationException)
        {
            // Sender is detached; ignore.
        }
        catch
        {
            // Isolate media forwarding faults from RTP/callback threads.
        }
    }

    /// <summary>
    /// Stops the forwarding loop and detaches frame subscriptions.
    /// </summary>
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0) return;

        _receiver.FrameReceived -= OnFrameReceived;
        _queue.Writer.TryComplete();
        _shutdownCts.Cancel();
        try
        {
            _pumpTask.GetAwaiter().GetResult();
        }
        catch
        {
            // Best effort shutdown.
        }
        _shutdownCts.Dispose();
    }
}
