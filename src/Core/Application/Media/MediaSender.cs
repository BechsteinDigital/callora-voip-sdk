using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Runtime.InteropServices;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Core.Application.Media;

public sealed class MediaSender : IMediaSender
{
    private readonly object _sync = new();
    private readonly ILogger<MediaSender> _logger;
    private Call? _call;
    private bool _disposed;

    internal MediaSender(ILogger<MediaSender>? logger = null)
    {
        _logger = logger ?? NullLogger<MediaSender>.Instance;
    }

    public void AttachToCall(ICall call)
    {
        if (call is not Call sdkCall)
            throw new ArgumentException("Call must be created by this SDK.", nameof(call));

        lock (_sync)
        {
            ThrowIfDisposed();
            _call = sdkCall;
        }
    }

    public void Detach()
    {
        lock (_sync) _call = null;
    }

    public Task SendAsync(MediaFrame frame, CancellationToken ct = default)
    {
        Call? call;
        lock (_sync)
        {
            ThrowIfDisposed();
            call = _call;
        }

        if (call == null)
            throw new InvalidOperationException("Media sender is not attached to a call.");

        // Audio callbacks can still race with call teardown; drop late frames instead
        // of surfacing process-terminating exceptions from real-time threads.
        var state = call.State;
        if (state is not (CallState.Connected or CallState.OnHold))
            return Task.CompletedTask;

        var payload = GetPayloadArray(frame.Payload);
        var outbound = new CallAudioFrame(payload, frame.PayloadType, frame.DurationRtpUnits);
        try
        {
            return call.SendAudioFrameAsync(outbound, ct);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogDebug(
                ex,
                "Dropping outbound media frame because call {CallId} is no longer send-capable (state={State}).",
                call.CallId,
                call.State);
            return Task.CompletedTask;
        }
    }

    public void Dispose()
    {
        lock (_sync)
        {
            if (_disposed) return;
            _disposed = true;
            _call = null;
        }
    }

    private static byte[] GetPayloadArray(ReadOnlyMemory<byte> payload)
    {
        if (MemoryMarshal.TryGetArray(payload, out ArraySegment<byte> segment) &&
            segment.Array != null)
        {
            if (segment.Offset == 0 && segment.Count == segment.Array.Length)
                return segment.Array;

            var copy = new byte[segment.Count];
            Buffer.BlockCopy(segment.Array, segment.Offset, copy, 0, segment.Count);
            return copy;
        }

        return payload.ToArray();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(MediaSender));
    }
}
