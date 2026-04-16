namespace CalloraVoipSdk.Core.Domain.Calls;

/// <summary>
/// Represents one ICE candidate associated with a call media leg.
/// </summary>
public sealed class CallIceCandidate
{
    /// <summary>
    /// Candidate foundation identifier.
    /// </summary>
    public required string Foundation { get; init; }

    /// <summary>
    /// Candidate component identifier (1 = RTP, 2 = RTCP).
    /// </summary>
    public required int Component { get; init; }

    /// <summary>
    /// Candidate transport token (for example UDP or TCP).
    /// </summary>
    public required string Transport { get; init; }

    /// <summary>
    /// Candidate priority value.
    /// </summary>
    public required long Priority { get; init; }

    /// <summary>
    /// Candidate address.
    /// </summary>
    public required string Address { get; init; }

    /// <summary>
    /// Candidate port.
    /// </summary>
    public required int Port { get; init; }

    /// <summary>
    /// Candidate type token (host, srflx, prflx, relay).
    /// </summary>
    public required string Type { get; init; }

    /// <summary>
    /// Related address (raddr) when present.
    /// </summary>
    public string? RelatedAddress { get; init; }

    /// <summary>
    /// Related port (rport) when present.
    /// </summary>
    public int? RelatedPort { get; init; }

    /// <summary>
    /// ICE generation value when present.
    /// </summary>
    public int? Generation { get; init; }

    /// <summary>
    /// Per-candidate ICE ufrag extension when present.
    /// </summary>
    public string? Ufrag { get; init; }

    /// <summary>
    /// Network-ID extension when present.
    /// </summary>
    public int? NetworkId { get; init; }
}
