namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Signaling state for a SIP dialog session.
/// </summary>
internal enum SipDialogState
{
    /// <summary>
    /// Session exists but no signaling action started yet.
    /// </summary>
    Idle,

    /// <summary>
    /// Outbound INVITE was sent and final answer is pending.
    /// </summary>
    Inviting,

    /// <summary>
    /// Call is alerting (ringing).
    /// </summary>
    Ringing,

    /// <summary>
    /// Dialog is established and media can flow.
    /// </summary>
    Established,

    /// <summary>
    /// Dialog is established but currently on hold.
    /// </summary>
    OnHold,

    /// <summary>
    /// Dialog is terminated and can no longer transition.
    /// </summary>
    Terminated
}

