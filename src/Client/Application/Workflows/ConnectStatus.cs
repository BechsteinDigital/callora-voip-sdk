namespace CalloraVoipSdk;

/// <summary>
/// Status for convenience registration/connect operations.
/// </summary>
public enum ConnectStatus
{
    /// <summary>
    /// The line reached the registered state.
    /// </summary>
    Registered,

    /// <summary>
    /// The operation timed out before registration completed.
    /// </summary>
    Timeout,

    /// <summary>
    /// The operation was canceled by the caller.
    /// </summary>
    Canceled,

    /// <summary>
    /// The operation failed.
    /// </summary>
    Failed
}
