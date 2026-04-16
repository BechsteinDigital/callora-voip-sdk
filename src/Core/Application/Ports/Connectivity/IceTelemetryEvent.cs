namespace CalloraVoipSdk.Core.Application.Ports.Connectivity;

/// <summary>
/// Structured ICE telemetry event produced by application-layer ICE orchestration.
/// </summary>
internal sealed class IceTelemetryEvent
{
    /// <summary>
    /// Event type identifier.
    /// </summary>
    public required string EventType { get; init; }

    /// <summary>
    /// Correlated call identifier when available.
    /// </summary>
    public string? CallId { get; init; }

    /// <summary>
    /// Correlation token for end-to-end tracing.
    /// </summary>
    public string? CorrelationId { get; init; }

    /// <summary>
    /// Event timestamp in UTC.
    /// </summary>
    public DateTimeOffset Timestamp { get; init; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Event attributes for diagnostics and analytics.
    /// </summary>
    public IReadOnlyDictionary<string, string> Attributes { get; init; }
        = new Dictionary<string, string>();
}
