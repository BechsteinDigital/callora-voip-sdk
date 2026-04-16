using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Inbound INVITE-specific context needed to bootstrap one dialog session.
/// </summary>
internal sealed class SipInboundSessionContext
{
    /// <summary>
    /// Initial inbound INVITE request that created the session.
    /// </summary>
    public required SipRequest InitialInvite { get; init; }

    /// <summary>
    /// Local To-tag generated for the dialog during INVITE processing.
    /// </summary>
    public required string LocalTag { get; init; }
}
