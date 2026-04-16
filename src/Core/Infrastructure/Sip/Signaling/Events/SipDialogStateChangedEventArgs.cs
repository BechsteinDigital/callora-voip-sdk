namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Event data emitted when a SIP dialog session changes state.
/// </summary>
internal sealed class SipDialogStateChangedEventArgs : EventArgs
{
    /// <summary>
    /// Creates state change event data.
    /// </summary>
    public SipDialogStateChangedEventArgs(
        SipDialogState oldState,
        SipDialogState newState,
        SipDialogTerminationReason? terminationReason = null)
    {
        OldState = oldState;
        NewState = newState;
        TerminationReason = terminationReason;
    }

    /// <summary>
    /// Previous dialog state.
    /// </summary>
    public SipDialogState OldState { get; }

    /// <summary>
    /// New dialog state.
    /// </summary>
    public SipDialogState NewState { get; }

    /// <summary>
    /// Optional RFC3326 Reason value when transition ends in <see cref="SipDialogState.Terminated"/>.
    /// </summary>
    public SipDialogTerminationReason? TerminationReason { get; }
}
