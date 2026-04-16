namespace CalloraVoipSdk.Core.Infrastructure.Rtp.JitterBuffer;

/// <summary>
/// Result of adding a packet to the jitter buffer.
/// </summary>
internal enum JitterBufferAddResult
{
    /// <summary>Packet accepted and queued for playout.</summary>
    Queued,

    /// <summary>Packet arrived too late — its playout time has already passed.</summary>
    Late,

    /// <summary>Packet is a duplicate (same extended sequence number already buffered or played).</summary>
    Duplicate,

    /// <summary>Buffer is full; packet was discarded to make room is not attempted.</summary>
    Overflow,
}
