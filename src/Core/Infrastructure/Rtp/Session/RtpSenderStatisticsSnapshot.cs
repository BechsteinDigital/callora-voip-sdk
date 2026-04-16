namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Session;

/// <summary>
/// Immutable RTP sender snapshot used by application orchestration and RTCP reporting.
/// </summary>
internal readonly record struct RtpSenderStatisticsSnapshot(
    uint LocalSsrc,
    uint SenderPacketCount,
    uint SenderOctetCount,
    uint LastSentRtpTimestamp,
    bool HasSentPackets);
