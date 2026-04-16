namespace CalloraVoipSdk.Core.Application.Ports.Connectivity;

/// <summary>
/// Port for publishing ICE telemetry events from application orchestration.
/// </summary>
internal interface IIceTelemetrySink
{
    /// <summary>
    /// Publishes one structured ICE telemetry event.
    /// </summary>
    void PublishEvent(IceTelemetryEvent record);
}
