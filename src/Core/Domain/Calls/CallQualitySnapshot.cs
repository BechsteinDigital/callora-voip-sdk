namespace CalloraVoipSdk.Core.Domain.Calls;

/// <summary>
/// Public quality snapshot for one call media leg.
/// Values are updated continuously while the call is active and can be used
/// by SDK consumers for routing, alerting, or UI quality indicators.
/// </summary>
public readonly record struct CallQualitySnapshot(
    DateTimeOffset CapturedAtUtc,
    bool RtcpActive,
    bool RtcpMux,
    double LocalReceiveJitterMs,
    double LocalReceivePacketLossPercent,
    double? RemoteReportJitterMs,
    double? RemoteReportPacketLossPercent,
    double? RoundTripTimeMs,
    long RtcpPacketsSent,
    long RtcpPacketsReceived)
{
    /// <summary>
    /// Creates an empty quality snapshot used before media is negotiated.
    /// </summary>
    public static CallQualitySnapshot CreateEmpty(DateTimeOffset capturedAtUtc, bool rtcpMux = false)
        => new(
            capturedAtUtc,
            RtcpActive: false,
            RtcpMux: rtcpMux,
            LocalReceiveJitterMs: 0,
            LocalReceivePacketLossPercent: 0,
            RemoteReportJitterMs: null,
            RemoteReportPacketLossPercent: null,
            RoundTripTimeMs: null,
            RtcpPacketsSent: 0,
            RtcpPacketsReceived: 0);
}
