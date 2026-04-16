namespace CalloraVoipSdk.Core.Infrastructure.Turn.Attributes;

/// <summary>
/// RFC 6062 CONNECTION-ID attribute model.
/// </summary>
internal sealed class TurnConnectionIdAttribute
{
    /// <summary>
    /// 32-bit connection identifier.
    /// </summary>
    public required uint ConnectionId { get; init; }
}
