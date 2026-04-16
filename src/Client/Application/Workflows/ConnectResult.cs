using CalloraVoipSdk.Core.Domain.Lines;

namespace CalloraVoipSdk;

/// <summary>
/// Result model for convenience registration/connect operations.
/// </summary>
public sealed class ConnectResult
{
    private ConnectResult(
        ConnectStatus status,
        IPhoneLine? line,
        LineState? finalLineState,
        Exception? error)
    {
        Status = status;
        Line = line;
        FinalLineState = finalLineState;
        Error = error;
    }

    /// <summary>
    /// Result status.
    /// </summary>
    public ConnectStatus Status { get; }

    /// <summary>
    /// Line instance created by the registration call when available.
    /// </summary>
    public IPhoneLine? Line { get; }

    /// <summary>
    /// Last observed line state when the operation completed.
    /// </summary>
    public LineState? FinalLineState { get; }

    /// <summary>
    /// Captured exception when <see cref="Status"/> is <see cref="ConnectStatus.Failed"/>.
    /// </summary>
    public Exception? Error { get; }

    /// <summary>
    /// True when <see cref="Status"/> is <see cref="ConnectStatus.Registered"/>.
    /// </summary>
    public bool IsSuccess => Status == ConnectStatus.Registered;

    /// <summary>
    /// Creates a successful registration result.
    /// </summary>
    public static ConnectResult Registered(IPhoneLine line) =>
        new(ConnectStatus.Registered, line, line.State, error: null);

    /// <summary>
    /// Creates a timeout registration result.
    /// </summary>
    public static ConnectResult Timeout(IPhoneLine? line, LineState? finalState = null) =>
        new(ConnectStatus.Timeout, line, finalState ?? line?.State, error: null);

    /// <summary>
    /// Creates a canceled registration result.
    /// </summary>
    public static ConnectResult Canceled(IPhoneLine? line, LineState? finalState = null) =>
        new(ConnectStatus.Canceled, line, finalState ?? line?.State, error: null);

    /// <summary>
    /// Creates a failed registration result.
    /// </summary>
    public static ConnectResult Failed(
        IPhoneLine? line,
        Exception? error = null,
        LineState? finalState = null) =>
        new(ConnectStatus.Failed, line, finalState ?? line?.State, error);
}
