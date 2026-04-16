namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Event payload for inbound REFER transfer requests.
/// </summary>
internal sealed class SipTransferRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Creates transfer-request event payload.
    /// </summary>
    public SipTransferRequestedEventArgs(string referTo, string referredBy)
    {
        ReferTo = referTo;
        ReferredBy = referredBy;
    }

    /// <summary>
    /// Target URI requested by remote REFER.
    /// </summary>
    public string ReferTo { get; }

    /// <summary>
    /// Identity string for transfer initiator.
    /// </summary>
    public string ReferredBy { get; }

    /// <summary>
    /// Application decision whether transfer request should be accepted.
    /// </summary>
    public bool Accept { get; set; }
}

