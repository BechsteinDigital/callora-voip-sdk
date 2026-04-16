using CalloraVoipSdk.Core.Infrastructure.Rtp.Packets;

namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Session;

/// <summary>
/// Manages one RTP media stream: sending encoded frames and receiving inbound packets
/// from the remote peer (RFC 3550 §3).
/// </summary>
internal interface IRtpSession : IAsyncDisposable
{
    /// <summary>
    /// Raised when a valid RTP packet arrives from the remote endpoint.
    /// Subscribers must not block the event; offload heavy work to a background task.
    /// </summary>
    event EventHandler<RtpPacket> PacketReceived;

    /// <summary>
    /// Raised when an inbound packet carries the same SSRC as this session's own outgoing SSRC,
    /// indicating a collision (RFC 3550 §8.2).
    /// The caller should stop sending, choose a new SSRC, and restart the session.
    /// </summary>
    event EventHandler SsrcCollisionDetected;

    /// <summary>
    /// Sends one encoded audio/video frame as an RTP packet.
    /// The session manages sequence numbers and timestamps automatically.
    /// </summary>
    /// <param name="payload">Raw encoded frame bytes.</param>
    /// <param name="marker">
    /// Marker bit — set for the first packet after a silence period
    /// or the last packet of a video frame (RFC 3550 §5.1).
    /// </param>
    /// <param name="payloadTypeOverride">
    /// Optional payload type override for this packet. When null, the session default
    /// from <see cref="RtpSessionOptions.PayloadType"/> is used.
    /// </param>
    /// <param name="cancellationToken">Cancellation token.</param>
    ValueTask SendAsync(
        ReadOnlyMemory<byte> payload,
        bool marker = false,
        byte? payloadTypeOverride = null,
        CancellationToken cancellationToken = default);

    /// <summary>Starts the receive loop. Must be called once before any packets arrive.</summary>
    Task StartAsync(CancellationToken cancellationToken = default);
}
