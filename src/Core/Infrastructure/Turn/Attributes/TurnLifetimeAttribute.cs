namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN LIFETIME attribute model (RFC 8656 §18.2).
/// </summary>
internal sealed class TurnLifetimeAttribute
{
    /// <summary>
    /// Lifetime in seconds.
    /// </summary>
    public required uint Seconds { get; init; }
}
