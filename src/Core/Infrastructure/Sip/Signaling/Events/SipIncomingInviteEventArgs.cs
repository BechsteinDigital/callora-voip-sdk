namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Event data for inbound SIP INVITE notifications.
/// </summary>
internal sealed class SipIncomingInviteEventArgs : EventArgs
{
    /// <summary>
    /// Creates inbound invite event data.
    /// </summary>
    public SipIncomingInviteEventArgs(ISipCallSession session) => Session = session;

    /// <summary>
    /// Session object representing the inbound ringing dialog.
    /// </summary>
    public ISipCallSession Session { get; }
}

