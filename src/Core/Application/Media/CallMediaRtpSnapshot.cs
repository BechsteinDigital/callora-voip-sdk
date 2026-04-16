namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Internal RTP runtime snapshot used by the RTCP quality monitor.
/// Captures sender and receiver counters that are required to build SR/RR packets
/// and to compute call quality metrics.
/// </summary>
internal readonly record struct CallMediaRtpSnapshot(
    DateTimeOffset CapturedAtUtc,
    uint LocalSsrc,
    uint? RemoteSsrc,
    uint SenderPacketCount,
    uint SenderOctetCount,
    uint LastSentRtpTimestamp,
    bool HasSentRtpPackets,
    uint PacketsExpected,
    uint PacketsReceived,
    byte FractionLost,
    int CumulativePacketsLost,
    uint ExtendedHighestSequenceNumber,
    uint InterarrivalJitterRtpUnits,
    double LocalReceiveJitterMs,
    double LocalReceivePacketLossPercent,
    double LocalRoundTripTimeHintMs);
