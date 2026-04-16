namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// TURN DATA attribute model (RFC 8656 §18.4).
/// </summary>
internal sealed class TurnDataAttribute
{
    /// <summary>
    /// Opaque payload bytes.
    /// </summary>
    public required ReadOnlyMemory<byte> Value { get; init; }
}
