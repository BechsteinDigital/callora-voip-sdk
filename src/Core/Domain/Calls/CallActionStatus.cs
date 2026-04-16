namespace CalloraVoipSdk.Core.Domain.Calls;

/// <summary>
/// Outcome category for extended call control operations.
/// </summary>
public enum CallActionStatus
{
    /// <summary>
    /// Operation completed successfully.
    /// </summary>
    Succeeded = 0,

    /// <summary>
    /// Operation was sent but rejected by the remote endpoint.
    /// </summary>
    Rejected = 1,

    /// <summary>
    /// Operation is invalid for the current call direction or state.
    /// </summary>
    InvalidState = 2,

    /// <summary>
    /// Input data for the operation is invalid.
    /// </summary>
    InvalidRequest = 3,

    /// <summary>
    /// Operation was canceled.
    /// </summary>
    Canceled = 4,

    /// <summary>
    /// Operation failed unexpectedly.
    /// </summary>
    Failed = 5
}
