namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Event payload for inbound SIP SUBSCRIBE requests.
/// </summary>
internal sealed class SipSubscriptionRequestedEventArgs : EventArgs
{
    /// <summary>
    /// Creates subscription-request event payload.
    /// </summary>
    public SipSubscriptionRequestedEventArgs(
        string eventType,
        int expiresSeconds,
        string? acceptHeader)
    {
        EventType = eventType;
        ExpiresSeconds = expiresSeconds;
        AcceptHeader = acceptHeader;
    }

    /// <summary>
    /// Event package name requested by the subscriber.
    /// </summary>
    public string EventType { get; }

    /// <summary>
    /// Requested subscription duration in seconds.
    /// </summary>
    public int ExpiresSeconds { get; }

    /// <summary>
    /// Accept header value sent by subscriber.
    /// </summary>
    public string? AcceptHeader { get; }

    /// <summary>
    /// Application decision whether subscription should be accepted.
    /// </summary>
    public bool Accept { get; set; }
}
