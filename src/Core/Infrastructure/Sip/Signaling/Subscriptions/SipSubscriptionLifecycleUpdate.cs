namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Represents one lifecycle transition result for inbound SIP subscriptions.
/// </summary>
internal sealed class SipSubscriptionLifecycleUpdate
{
    /// <summary>
    /// Normalized subscription expires value used in SUBSCRIBE response.
    /// </summary>
    public required int EffectiveExpiresSeconds { get; init; }

    /// <summary>
    /// Subscription-State header value for follow-up NOTIFY.
    /// </summary>
    public required string SubscriptionStateHeader { get; init; }
}
