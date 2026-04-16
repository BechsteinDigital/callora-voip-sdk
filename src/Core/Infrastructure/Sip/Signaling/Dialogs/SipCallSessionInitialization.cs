using CalloraVoipSdk.Core.Infrastructure.Sip.Wire;

namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Internal initialization state for constructing inbound or outbound dialog sessions.
/// </summary>
internal sealed class SipCallSessionInitialization
{
    /// <summary>
    /// True when the dialog originated from an inbound INVITE.
    /// </summary>
    public required bool IsInbound { get; init; }

    /// <summary>
    /// Initial inbound INVITE request for inbound dialogs.
    /// Null for outbound dialogs.
    /// </summary>
    public SipRequest? InitialInvite { get; init; }

    /// <summary>
    /// Local SIP tag value to apply to dialog headers.
    /// </summary>
    public string? LocalTag { get; init; }

    /// <summary>
    /// Remote SIP tag learned from peer headers.
    /// </summary>
    public string? RemoteTag { get; init; }

    /// <summary>
    /// Initial signaling state assigned at session creation.
    /// </summary>
    public required SipDialogState InitialState { get; init; }

    /// <summary>
    /// Creates initialization state for outbound dialogs.
    /// </summary>
    public static SipCallSessionInitialization CreateOutbound() =>
        new()
        {
            IsInbound = false,
            InitialState = SipDialogState.Idle
        };

    /// <summary>
    /// Creates initialization state for inbound dialogs.
    /// </summary>
    public static SipCallSessionInitialization CreateInbound(
        SipRequest initialInvite,
        string localTag,
        string? remoteTag) =>
        new()
        {
            IsInbound = true,
            InitialInvite = initialInvite ?? throw new ArgumentNullException(nameof(initialInvite)),
            LocalTag = string.IsNullOrWhiteSpace(localTag)
                ? throw new ArgumentException("Local tag is required.", nameof(localTag))
                : localTag,
            RemoteTag = remoteTag,
            InitialState = SipDialogState.Ringing
        };
}
