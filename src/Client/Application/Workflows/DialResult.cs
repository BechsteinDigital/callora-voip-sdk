using CalloraVoipSdk.Core.Domain.Calls;

namespace CalloraVoipSdk;

/// <summary>
/// Result model for convenience outbound dial-and-wait operations.
/// </summary>
public sealed class DialResult
{
    private DialResult(
        DialStatus status,
        ICall? call,
        CallState? finalCallState,
        Exception? error)
    {
        Status = status;
        Call = call;
        FinalCallState = finalCallState;
        Error = error;
    }

    /// <summary>
    /// Result status.
    /// </summary>
    public DialStatus Status { get; }

    /// <summary>
    /// Call created by the dial operation when available.
    /// </summary>
    public ICall? Call { get; }

    /// <summary>
    /// Last observed call state when the operation completed.
    /// </summary>
    public CallState? FinalCallState { get; }

    /// <summary>
    /// Captured exception when <see cref="Status"/> is <see cref="DialStatus.Failed"/>.
    /// </summary>
    public Exception? Error { get; }

    /// <summary>
    /// True when <see cref="Status"/> is <see cref="DialStatus.Connected"/>.
    /// </summary>
    public bool IsSuccess => Status == DialStatus.Connected;

    /// <summary>
    /// Creates a successful connected dial result.
    /// </summary>
    public static DialResult Connected(ICall call) =>
        new(DialStatus.Connected, call, call.State, error: null);

    /// <summary>
    /// Creates a timeout dial result.
    /// </summary>
    public static DialResult Timeout(ICall? call, CallState? finalState = null) =>
        new(DialStatus.Timeout, call, finalState ?? call?.State, error: null);

    /// <summary>
    /// Creates a canceled dial result.
    /// </summary>
    public static DialResult Canceled(ICall? call, CallState? finalState = null) =>
        new(DialStatus.Canceled, call, finalState ?? call?.State, error: null);

    /// <summary>
    /// Creates a failed dial result.
    /// </summary>
    public static DialResult Failed(ICall? call, Exception? error = null, CallState? finalState = null) =>
        new(DialStatus.Failed, call, finalState ?? call?.State, error);
}
