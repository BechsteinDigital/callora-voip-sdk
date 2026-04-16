namespace CalloraVoipSdk.Core.Infrastructure.Turn.Server;

/// <summary>
/// Behavior when stream connection cap is reached.
/// </summary>
internal enum TurnConnectionCapPolicy
{
    /// <summary>Wait for a free slot before accepting more connections.</summary>
    Backpressure,

    /// <summary>Accept and immediately reject connections above the cap.</summary>
    RejectNew
}
