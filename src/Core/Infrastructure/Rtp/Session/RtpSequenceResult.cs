namespace CalloraVoipSdk.Core.Infrastructure.Rtp.Session;

/// <summary>
/// Classification of an incoming RTP sequence number (RFC 3550 §A.1).
/// </summary>
internal enum RtpSequenceResult
{
    /// <summary>Packet is in sequence — deliver to application.</summary>
    Valid,

    /// <summary>
    /// Source is on probation; packet is counted but not yet delivered
    /// until <see cref="RtpSequenceValidator.MinSequential"/> consecutive
    /// packets confirm the source (RFC 3550 §A.1).
    /// </summary>
    Probation,

    /// <summary>Sequence number is a duplicate of one already received — drop.</summary>
    Duplicate,

    /// <summary>Packet arrived too late (beyond MAX_MISORDER behind) — drop.</summary>
    TooLate,

    /// <summary>
    /// Sequence number is far ahead of expected, suggesting a source restart.
    /// Source returns to probation (RFC 3550 §A.1).
    /// </summary>
    SequenceJump,
}
