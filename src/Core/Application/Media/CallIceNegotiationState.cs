namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Lifecycle states of the application-layer ICE negotiation flow.
/// </summary>
internal enum CallIceNegotiationState
{
    /// <summary>
    /// ICE is disabled or no ICE metadata is available.
    /// </summary>
    Disabled,

    /// <summary>
    /// Local candidate gathering is in progress.
    /// </summary>
    Gathering,

    /// <summary>
    /// Candidate gathering completed.
    /// </summary>
    Gathered,

    /// <summary>
    /// Connectivity checks are running.
    /// </summary>
    Checking,

    /// <summary>
    /// One successful pair is being nominated.
    /// </summary>
    Nominating,

    /// <summary>
    /// A candidate pair was selected successfully.
    /// </summary>
    Connected,

    /// <summary>
    /// ICE checks failed and fallback handling is required.
    /// </summary>
    Failed
}
