namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Event payload for inbound SIP NOTIFY requests (RFC 6665 §6.1.1).
/// </summary>
internal sealed class SipNotifyReceivedEventArgs : EventArgs
{
    /// <summary>
    /// Creates inbound NOTIFY event payload.
    /// </summary>
    public SipNotifyReceivedEventArgs(
        string eventType,
        string subscriptionState,
        bool isTerminated,
        string? contentType,
        string? body)
    {
        EventType = eventType;
        SubscriptionState = subscriptionState;
        IsTerminated = isTerminated;
        ContentType = contentType;
        Body = body;
    }

    /// <summary>
    /// Event package name from the <c>Event</c> header (e.g. "refer", "presence").
    /// </summary>
    public string EventType { get; }

    /// <summary>
    /// Raw value of the <c>Subscription-State</c> header (e.g. "active;expires=60", "terminated;reason=noresource").
    /// </summary>
    public string SubscriptionState { get; }

    /// <summary>
    /// True when <c>Subscription-State</c> starts with "terminated", meaning the subscription has ended.
    /// </summary>
    public bool IsTerminated { get; }

    /// <summary>
    /// <c>Content-Type</c> header value, or <c>null</c> when the NOTIFY has no body.
    /// </summary>
    public string? ContentType { get; }

    /// <summary>
    /// Notification body, or <c>null</c> when empty.
    /// </summary>
    public string? Body { get; }
}
