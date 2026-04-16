using CalloraVoipSdk.Core.Application.Media;
using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk.Modules;

/// <summary>
/// SDK facade for realtime call-audio bridging.
/// </summary>
public interface IRealtimeModule
{
    /// <summary>True when this module can be used in the current runtime context.</summary>
    bool IsAvailable { get; }

    /// <summary>Active realtime bridges.</summary>
    IReadOnlyCollection<ICallRealtimeBridge> Active { get; }

    /// <summary>
    /// Starts a bidirectional realtime bridge for one active call.
    /// </summary>
    Task<ICallRealtimeBridge> StartCallBridgeAsync(
        ICall call,
        IAudioFrameStreamTransport transport,
        RealtimeBridgeOptions? options = null,
        CancellationToken ct = default);
}

/// <summary>
/// Realtime bridge lifecycle state.
/// </summary>
public enum RealtimeBridgeState
{
    Created = 0,
    Running = 1,
    Reconnecting = 2,
    Stopped = 3,
    Faulted = 4,
}

/// <summary>
/// Realtime bridge buffering policy when queues are full.
/// </summary>
public enum RealtimeBridgeBackpressureStrategy
{
    DropNewest = 0,
    DropOldest = 1,
}

/// <summary>
/// Realtime bridge media direction.
/// </summary>
public enum RealtimeBridgeDirection
{
    FullDuplex = 0,
    CallToTransportOnly = 1,
    TransportToCallOnly = 2,
}

/// <summary>
/// Realtime bridge fault code.
/// </summary>
public enum RealtimeBridgeFaultCode
{
    Unknown = 0,
    TransportReadFailed = 1,
    TransportWriteFailed = 2,
    TransportReconnectExhausted = 3,
    CallSenderFailed = 4,
}

/// <summary>
/// Runtime options for one realtime bridge.
/// </summary>
public sealed class RealtimeBridgeOptions
{
    /// <summary>
    /// Max buffered frames from call to transport.
    /// </summary>
    public int MaxBufferedCallToTransportFrames { get; set; } = 256;

    /// <summary>
    /// Max buffered frames from transport to call.
    /// </summary>
    public int MaxBufferedTransportToCallFrames { get; set; } = 256;

    /// <summary>
    /// Overflow strategy for both queues.
    /// </summary>
    public RealtimeBridgeBackpressureStrategy BackpressureStrategy { get; set; } = RealtimeBridgeBackpressureStrategy.DropOldest;

    /// <summary>
    /// Active media direction for this bridge.
    /// </summary>
    public RealtimeBridgeDirection Direction { get; set; } = RealtimeBridgeDirection.FullDuplex;

    /// <summary>
    /// Enables automatic reconnect attempts after transport failures.
    /// </summary>
    public bool EnableReconnect { get; set; } = true;

    /// <summary>
    /// Max reconnect attempts after one transport failure.
    /// </summary>
    public int MaxReconnectAttempts { get; set; } = 5;

    /// <summary>
    /// Initial reconnect delay.
    /// </summary>
    public TimeSpan InitialReconnectDelay { get; set; } = TimeSpan.FromMilliseconds(250);

    /// <summary>
    /// Max reconnect delay.
    /// </summary>
    public TimeSpan MaxReconnectDelay { get; set; } = TimeSpan.FromSeconds(5);

    /// <summary>
    /// Exponential reconnect backoff multiplier.
    /// </summary>
    public double ReconnectBackoffFactor { get; set; } = 2.0;

    /// <summary>
    /// Automatically stops the bridge when the call reaches terminated state.
    /// </summary>
    public bool AutoStopOnCallTerminated { get; set; } = true;

    /// <summary>
    /// Optional timeout used while draining during stop.
    /// </summary>
    public TimeSpan StopDrainTimeout { get; set; } = TimeSpan.FromSeconds(1);
}

/// <summary>
/// Active realtime bridge contract.
/// </summary>
public interface ICallRealtimeBridge : IAsyncDisposable
{
    /// <summary>Stable bridge identifier.</summary>
    Guid BridgeId { get; }

    /// <summary>Call associated with this bridge.</summary>
    ICall Call { get; }

    /// <summary>Current bridge state.</summary>
    RealtimeBridgeState State { get; }

    /// <summary>Current metrics snapshot.</summary>
    RealtimeBridgeMetricsSnapshot Metrics { get; }

    /// <summary>Raised when bridge state changes.</summary>
    event EventHandler<RealtimeBridgeStateChangedEventArgs>? StateChanged;

    /// <summary>Raised when bridge transitions to faulted state.</summary>
    event EventHandler<RealtimeBridgeFaultedEventArgs>? Faulted;

    /// <summary>
    /// Stops the bridge and releases media and transport resources.
    /// </summary>
    Task StopAsync(CancellationToken ct = default);
}

/// <summary>
/// State transition event args for realtime bridge.
/// </summary>
public sealed class RealtimeBridgeStateChangedEventArgs(
    RealtimeBridgeState oldState,
    RealtimeBridgeState newState,
    string reason,
    DateTimeOffset occurredAtUtc) : EventArgs
{
    public RealtimeBridgeState OldState { get; } = oldState;

    public RealtimeBridgeState NewState { get; } = newState;

    public string Reason { get; } = reason;

    public DateTimeOffset OccurredAtUtc { get; } = occurredAtUtc;
}

/// <summary>
/// Fault event args for realtime bridge.
/// </summary>
public sealed class RealtimeBridgeFaultedEventArgs(
    RealtimeBridgeFaultCode code,
    Exception exception,
    string message,
    DateTimeOffset occurredAtUtc) : EventArgs
{
    public RealtimeBridgeFaultCode Code { get; } = code;

    public Exception Exception { get; } = exception;

    public string Message { get; } = message;

    public DateTimeOffset OccurredAtUtc { get; } = occurredAtUtc;
}

/// <summary>
/// Runtime metrics snapshot for one realtime bridge.
/// </summary>
public sealed class RealtimeBridgeMetricsSnapshot
{
    public long FramesCallToTransportSent { get; init; }

    public long FramesTransportToCallSent { get; init; }

    public long FramesCallToTransportDropped { get; init; }

    public long FramesTransportToCallDropped { get; init; }

    public long ReconnectAttempts { get; init; }

    public long ReconnectSuccesses { get; init; }

    public long ReconnectFailures { get; init; }
}

/// <summary>
/// Transport contract for audio-frame streaming.
/// </summary>
public interface IAudioFrameStreamTransport : IAsyncDisposable
{
    /// <summary>True while transport is connected.</summary>
    bool IsConnected { get; }

    /// <summary>Connects transport resources.</summary>
    Task ConnectAsync(CancellationToken ct = default);

    /// <summary>Closes transport resources.</summary>
    Task CloseAsync(CancellationToken ct = default);

    /// <summary>
    /// Reads incoming media frames from transport.
    /// </summary>
    IAsyncEnumerable<MediaFrame> ReadFramesAsync(CancellationToken ct = default);

    /// <summary>
    /// Sends one media frame over transport.
    /// </summary>
    ValueTask SendFrameAsync(MediaFrame frame, CancellationToken ct = default);
}
