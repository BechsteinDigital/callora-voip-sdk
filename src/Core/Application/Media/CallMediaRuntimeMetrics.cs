namespace CalloraVoipSdk.Core.Application.Media;

/// <summary>
/// Runtime snapshot for one active call media receive pipeline.
/// Used for operational troubleshooting and quality debugging.
/// </summary>
internal readonly record struct CallMediaRuntimeMetrics
{
    /// <summary>
    /// Creates a metrics snapshot value.
    /// </summary>
    public CallMediaRuntimeMetrics(
        DateTimeOffset capturedAtUtc,
        long packetsReceived,
        long packetsQueued,
        long packetsDelivered,
        long packetsDroppedLate,
        long packetsDroppedOverflow,
        long packetsDroppedDuplicate,
        long packetsConcealed,
        long packetsUnrecoverableLoss,
        int bufferedPackets,
        double estimatedJitterMs,
        double adaptiveDelayMs,
        double estimatedRoundTripTimeMs)
    {
        CapturedAtUtc = capturedAtUtc;
        PacketsReceived = packetsReceived;
        PacketsQueued = packetsQueued;
        PacketsDelivered = packetsDelivered;
        PacketsDroppedLate = packetsDroppedLate;
        PacketsDroppedOverflow = packetsDroppedOverflow;
        PacketsDroppedDuplicate = packetsDroppedDuplicate;
        PacketsConcealed = packetsConcealed;
        PacketsUnrecoverableLoss = packetsUnrecoverableLoss;
        BufferedPackets = bufferedPackets;
        EstimatedJitterMs = estimatedJitterMs;
        AdaptiveDelayMs = adaptiveDelayMs;
        EstimatedRoundTripTimeMs = estimatedRoundTripTimeMs;
    }

    /// <summary>Timestamp when the snapshot was captured (UTC).</summary>
    public DateTimeOffset CapturedAtUtc { get; }

    /// <summary>Total count of RTP packets observed from the network.</summary>
    public long PacketsReceived { get; }

    /// <summary>Total count of packets accepted by the jitter buffer queue.</summary>
    public long PacketsQueued { get; }

    /// <summary>Total count of original RTP packets delivered to the consumer.</summary>
    public long PacketsDelivered { get; }

    /// <summary>Total count of packets dropped because their playout deadline already passed.</summary>
    public long PacketsDroppedLate { get; }

    /// <summary>Total count of packets dropped because jitter-buffer capacity was exhausted.</summary>
    public long PacketsDroppedOverflow { get; }

    /// <summary>Total count of packets dropped as duplicates.</summary>
    public long PacketsDroppedDuplicate { get; }

    /// <summary>Total count of PLC-generated concealment packets emitted for gaps.</summary>
    public long PacketsConcealed { get; }

    /// <summary>
    /// Total count of missing packets that could not be concealed because the configured
    /// concealment burst limit was exceeded.
    /// </summary>
    public long PacketsUnrecoverableLoss { get; }

    /// <summary>Current number of packets buffered and waiting for playout.</summary>
    public int BufferedPackets { get; }

    /// <summary>Estimated inter-arrival jitter in milliseconds.</summary>
    public double EstimatedJitterMs { get; }

    /// <summary>Current adaptive playout delay in milliseconds.</summary>
    public double AdaptiveDelayMs { get; }

    /// <summary>Smoothed RTT estimate in milliseconds.</summary>
    public double EstimatedRoundTripTimeMs { get; }
}
