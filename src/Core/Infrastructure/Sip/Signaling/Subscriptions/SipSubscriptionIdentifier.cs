namespace CalloraVoipSdk.Core.Infrastructure.Sip.Signaling;

/// <summary>
/// Identifies one SIP event subscription by event package and optional id parameter.
/// </summary>
internal sealed class SipSubscriptionIdentifier
{
    /// <summary>
    /// Creates one normalized subscription identifier.
    /// </summary>
    public SipSubscriptionIdentifier(
        string eventPackage,
        string? eventId)
    {
        if (string.IsNullOrWhiteSpace(eventPackage))
            throw new ArgumentException("eventPackage is required.", nameof(eventPackage));

        EventPackage = eventPackage.Trim();
        EventId = string.IsNullOrWhiteSpace(eventId) ? null : eventId.Trim();
        Key = $"{EventPackage.ToUpperInvariant()}|{EventId?.ToUpperInvariant() ?? string.Empty}";
    }

    /// <summary>
    /// Event package token (for example presence, refer, message-summary).
    /// </summary>
    public string EventPackage { get; }

    /// <summary>
    /// Optional event id parameter.
    /// </summary>
    public string? EventId { get; }

    /// <summary>
    /// Canonical key for dictionary indexing.
    /// </summary>
    public string Key { get; }

    /// <summary>
    /// Formats this identifier for SIP Event header value.
    /// </summary>
    public string ToEventHeaderValue() =>
        string.IsNullOrWhiteSpace(EventId)
            ? EventPackage
            : $"{EventPackage};id={EventId}";

    /// <summary>
    /// Parses Event header into subscription identifier.
    /// </summary>
    public static bool TryParse(
        string? eventHeader,
        out SipSubscriptionIdentifier identifier)
    {
        identifier = null!;
        if (string.IsNullOrWhiteSpace(eventHeader))
            return false;

        var parts = eventHeader
            .Split(';', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return false;

        var package = parts[0].Trim();
        if (package.Length == 0)
            return false;

        string? eventId = null;
        for (var i = 1; i < parts.Length; i++)
        {
            var segment = parts[i];
            var equalsIndex = segment.IndexOf('=');
            if (equalsIndex <= 0)
                continue;

            var key = segment[..equalsIndex].Trim();
            if (!key.Equals("id", StringComparison.OrdinalIgnoreCase))
                continue;

            eventId = segment[(equalsIndex + 1)..].Trim().Trim('"');
            break;
        }

        identifier = new SipSubscriptionIdentifier(package, eventId);
        return true;
    }
}
