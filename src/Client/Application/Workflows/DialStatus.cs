namespace CalloraVoipSdk;

/// <summary>
/// Status for convenience outbound dial-and-wait operations.
/// </summary>
public enum DialStatus
{
    /// <summary>
    /// The call reached <c>Connected</c> (or <c>OnHold</c>) successfully.
    /// </summary>
    Connected,

    /// <summary>
    /// The operation timed out before connection establishment.
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
